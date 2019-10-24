using Microsoft.Extensions.Configuration;
using NewAlbums.Configuration;
using NewAlbums.Emails.Templates;
using NewAlbums.Emails.Templates.Dto;
using NewAlbums.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Emails.Templates
{
    public class TemplateManager : BaseManager
    {
        private readonly IConfiguration _configuration;

        private const string EmailFontFamily = "font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;";
        private string EmailRowHtml = $"<tr style=\"{EmailFontFamily} box-sizing: border-box; font-size: 14px; margin: 0;\">";
        private string BodyCellHtml = $"<td class=\"content-block\" style=\"{EmailFontFamily} box-sizing: border-box;" 
                                      + "font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px; text-align: center;\" align=\"center\" valign=\"top\">";

        private string FooterCellHtml = $"<td class=\"aligncenter content-block\" style=\"{EmailFontFamily} box-sizing: border-box; font-size: 12px; vertical-align: top; "
                                        + "color: #999; text-align: center; margin: 0; padding: 0;\" align=\"center\" valign=\"top\">";

        public TemplateManager(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<StringBuilder> GetHtmlEmailTemplate(GetTemplateInput input)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"NewAlbums.Emails.Templates.{input.TemplateType}.html"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    var template = new StringBuilder(Encoding.UTF8.GetString(bytes, 0, bytes.Length));

                    ReplaceVariables(ref template, input);
                    return template;
                }
            }
        }

        private void ReplaceVariables(ref StringBuilder template, GetTemplateInput input)
        {
            //Common variables
            string primaryColour = _configuration[AppSettingKeys.Style.PrimaryColour];
            string textColourOnPrimary = _configuration[AppSettingKeys.Style.TextColourOnPrimary];

            template.Replace($"{{{TemplateVariables.PrimaryColour}}}", primaryColour);
            template.Replace($"{{{TemplateVariables.BackgroundColour}}}", _configuration[AppSettingKeys.Style.BackgroundColour]);
            template.Replace($"{{{TemplateVariables.FrontEndRootUrl}}}", _configuration[AppSettingKeys.App.FrontEndRootUrl]);

            //Other variables specific to this email
            foreach (var key in input.SimpleVariableValues.Keys)
            {
                template.Replace($"{{{key}}}", input.SimpleVariableValues[key]);
            }

            //Body
            var bodyHtml = new StringBuilder();
            foreach (var paragraph in input.BodyParagraphs)
            {
                bodyHtml.Append($"{EmailRowHtml}{BodyCellHtml}");

                if (String.IsNullOrWhiteSpace(paragraph.ButtonUrl))
                {
                    bodyHtml.Append($"{paragraph.HtmlText}");
                }
                else
                {
                    bodyHtml.Append($"<a href=\"{paragraph.ButtonUrl}\" target=\"_blank\" rel=\"noopener\" class=\"btn-primary\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; "
                        + $"box-sizing: border-box; font-size: 14px; color: {textColourOnPrimary}; text-decoration: none; line-height: 2em; font-weight: bold; " 
                        + $"text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; background-color: {primaryColour}; "
                        + $"margin: 0; border-color: {primaryColour}; border-style: solid; border-width: 10px 20px;\">{paragraph.HtmlText}</a>");
                }

                bodyHtml.Append("</td></tr>");
            }

            template.Replace($"{{{TemplateVariables.Body}}}", bodyHtml.ToString());

            //Footer
            var footerHtml = new StringBuilder();

            //Append provided lines first
            foreach (var line in input.FooterLines)
            {
                footerHtml.Append($"{EmailRowHtml}{FooterCellHtml}{line.HtmlText}</td></tr>");
            }

            //Then append generic lines for all emails
            string frontEndRootUrl = _configuration[AppSettingKeys.App.FrontEndRootUrl];
            string contactEmailAddress = _configuration[AppSettingKeys.App.ContactEmailAddress];
            string spotifyLogoUrl = frontEndRootUrl.EnsureEndsWith('/') + "images/spotify-logo-white.png";

            footerHtml.Append($"{EmailRowHtml}{FooterCellHtml}{GetEmailLink(frontEndRootUrl, frontEndRootUrl, null, "12px")}</td></tr>");
            footerHtml.Append($"{EmailRowHtml}{FooterCellHtml}{GetEmailLink($"mailto:{contactEmailAddress}", contactEmailAddress, null, "12px")}</td></tr>");
            footerHtml.Append($"{EmailRowHtml}{FooterCellHtml}Artist and album content including cover art supplied by:</td></tr>");
            footerHtml.Append($"{EmailRowHtml}{FooterCellHtml}<img alt=\"Spotify logo\" src=\"{spotifyLogoUrl}\" width=\"80\" height=\"24\" style=\"width: 80px; height: 24px; margin: 0; outline: none;\" /></td></tr>");

            template.Replace($"{{{TemplateVariables.Footer}}}", footerHtml.ToString());
        }

        public string GetEmailLink(string url, string text, string colour = null, string fontSize = "14px")
        {
            if (String.IsNullOrWhiteSpace(colour))
            {
                colour = _configuration[AppSettingKeys.Style.PrimaryColour];
            }

            string target = "_blank";
            if (url.StartsWith("mailto:"))
            {
                target = "_self";
            }

            return $"<a href=\"{url}\" target=\"{target}\" rel=\"noopener\" style=\"{EmailFontFamily} "
                + $"font-size: {fontSize}; color: {colour}; text-decoration: underline; margin: 0; box-sizing: border-box; \">{text}</a>";
        }

        public string GetEmailTextFooter()
        {
            string appName = _configuration[AppSettingKeys.App.Name];
            string frontEndRootUrl = _configuration[AppSettingKeys.App.FrontEndRootUrl];
            string contactEmailAddress = _configuration[AppSettingKeys.App.ContactEmailAddress];

            return $"\r\n\r\n\r\n----------------Sent by {appName}\r\n{frontEndRootUrl}\r\nContact: {contactEmailAddress}";
        }
    }
}
