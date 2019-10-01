using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Configuration;
using NewAlbums.Debugging;
using NewAlbums.Emails.Dto;
using NewAlbums.Paths;
using NewAlbums.Utils;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Emails
{
    public class EmailManager : BaseManager
    {
        private readonly IPathProvider _pathProvider;
        private readonly IConfiguration _configuration;

        public EmailManager(
            IPathProvider pathProvider,
            IConfiguration configuration)
        {
            _pathProvider = pathProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// Retries sending email up to 3 times if an error is encountered.
        /// Returns null if successful, otherwise the error text.
        /// </summary>
        public async Task<string> SendEmail(EmailMessage email)
        {
            SaveEmailToDisk(email);

            if (DebugHelper.IsDebug)
            {
                //Don't actually send the email in debug/development mode
                return null;
            }

            string error = null;

            for (int i = 0; i < 3; i++)
            {
                error = await SendMailgunEmail(email);
                if (String.IsNullOrWhiteSpace(error))
                    break;
            }

            return error;
        }

        private async Task<string> SendMailgunEmail(EmailMessage email)
        {
            string error = null;

            string info = String.Format("Sending email with subject: {0}, To: {1}", email.Subject, String.Join(",", email.ToAddresses));
            Logger.LogInformation(info);

            string mailgunApiKey = _configuration[AppSettingKeys.Mailgun.ApiKey];
            string mailgunApiUrl = _configuration[AppSettingKeys.Mailgun.ApiUrl];
            string mailgunDomain = _configuration[AppSettingKeys.Mailgun.Domain];

            if (String.IsNullOrWhiteSpace(mailgunApiKey) || String.IsNullOrWhiteSpace(mailgunApiUrl) || String.IsNullOrWhiteSpace(mailgunDomain))
            {
                Logger.LogError("Mailgun not configured, cannot send email.");
                return "Mailgun not configured, cannot send email.";
            }

            if (email.FromAddress == null)
            {
                //Set default from address
                email.FromAddress = new EmailAddress
                {
                    Address = _configuration[AppSettingKeys.App.SystemEmailAddress],
                    DisplayName = _configuration[AppSettingKeys.App.Name]
                };
            }

            var client = new RestClient();
            client.BaseUrl = new Uri(mailgunApiUrl);
            client.Authenticator = new HttpBasicAuthenticator("api", mailgunApiKey);

            var request = new RestRequest();
            request.AddParameter("domain", mailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", String.Format("{0} <{1}>", email.FromAddress.DisplayName, email.FromAddress.Address));

            foreach (var toAddress in email.ToAddresses)
            {
                request.AddParameter("to", String.Format("{0} <{1}>", toAddress.DisplayName, toAddress.Address));
            }

            request.AddParameter("subject", email.Subject);
            request.Method = Method.POST;

            if (!String.IsNullOrEmpty(email.BodyHtml))
                request.AddParameter("html", email.BodyHtml);

            if (!String.IsNullOrEmpty(email.BodyText))
                request.AddParameter("text", email.BodyText);

            if (email.ReplyToAddress != null)
                request.AddParameter("h:Reply-To", email.ReplyToAddress.Address);

            //Commented code not currently needed, but leave so it's easy to implement later if needed
            //Add CCs
            /*
            string ccString = "";
            if (email.CCs != null)
            {
                foreach (var cc in email.CCs)
                {
                    if (ShouldSendEmail(cc.ToAddress))
                    {
                        ccString += String.Format("{0} <{1}>,", cc.ToDisplay, cc.ToAddress);
                    }
                    else
                    {
                        string ccInfo = String.Format("Not ccing email to: {0}, ShouldSendEmail() returned false",
                            cc.ToAddress);
                        Logger.Info(ccInfo);
                        Console.Out.WriteLine(ccInfo);
                    }
                }

                if (!String.IsNullOrEmpty(ccString))
                {
                    ccString = ccString.Substring(0, ccString.Length - 1); //Remove the last comma
                    request.AddParameter("cc", ccString);
                }
            }
            
            //Attachments
            if (email.Attachments != null)
            {
                foreach (var attachment in email.Attachments)
                {
                    //Resolve that the attachments exist, skip email if not
                    var fileInfo = new FileInfo(attachment.FilePath);
                    if (!fileInfo.Exists)
                    {
                        string skipInfo = String.Format("Attachment Id: {0} didn't exist, skipping email", attachment.Id);
                        Logger.Info(skipInfo);
                        Console.Out.WriteLine(skipInfo);
                        return false;
                    }

                    byte[] fileData = File.ReadAllBytes(fileInfo.FullName);
                    request.AddFile("attachment", fileData, fileInfo.Name, FileUtils.GetMimeType(fileInfo.Extension));
                }
            }
            */

            var response = await client.ExecuteAsync(request);

            //Error checking
            if (!String.IsNullOrEmpty(response.ErrorMessage))
            {
                Logger.LogError("Received ErrorMessage from Mailgun: " + response.ErrorMessage);
                error = response.ErrorMessage;
            }

            if (response.ErrorException != null)
            {
                Logger.LogError("Received ErrorException from Mailgun", response.ErrorException);

                if (String.IsNullOrEmpty(error))
                    error = response.ErrorException.Message;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.LogError("Received non-OK status code from Mailgun: " + response.StatusCode.ToString());

                if (String.IsNullOrEmpty(error))
                    error = String.Format("Mailgun returned error status code: {0}", response.StatusCode.ToString());
            }

            if (response.ErrorException == null && String.IsNullOrEmpty(response.ErrorMessage) &&
                response.StatusCode == HttpStatusCode.OK)
            {
                //The email sent successfully.
                Logger.LogInformation("Email sent successfully");
                return null;
            }

            //If we get here, an error occurred.   
            return error;
        }

        /// <summary>
        /// Makes debugging emails easier, prevents debug emails from having to actually be sent
        /// </summary>
        private void SaveEmailToDisk(EmailMessage email)
        {
            string toAddress = email.ToAddresses?.FirstOrDefault()?.Address;

            try
            {
                string emailsFolderPath = _pathProvider.GetAbsoluteEmailsFolderPath();
                if (!String.IsNullOrWhiteSpace(emailsFolderPath))
                {
                    var info = new DirectoryInfo(emailsFolderPath);
                    if (!info.Exists)
                        info.Create();

                    if (!String.IsNullOrWhiteSpace(email.BodyHtml))
                    {
                        string htmlFilename = FileUtils.GetFileSafeName(String.Format("{0}-{1}-{2}.html",
                            DateTime.Now.ToString("yyyyMMdd-HHmmss-ffff"), toAddress, email.Subject));

                        string htmlFilePath = Path.Combine(emailsFolderPath, htmlFilename);
                        File.WriteAllText(htmlFilePath, email.BodyHtml);
                    }

                    if (!String.IsNullOrWhiteSpace(email.BodyText))
                    {
                        string textFilename = FileUtils.GetFileSafeName(String.Format("{0}-{1}-{2}.txt",
                            DateTime.Now.ToString("yyyyMMdd-HHmmss-ffff"), toAddress, email.Subject));

                        string textFilePath = Path.Combine(emailsFolderPath, textFilename);
                        File.WriteAllText(textFilePath, email.BodyText);
                    }
                }
            }
            catch (Exception ex)
            {
                //This is not a crucial function, don't throw exceptions on error, just log
                Logger.LogError(ex, "Caught exception saving email To: {0}, Subject: {1}", toAddress, email.Subject);
            }
        }
    }
}
