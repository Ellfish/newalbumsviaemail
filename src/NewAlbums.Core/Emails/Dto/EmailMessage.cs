using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Emails.Dto
{
    public class EmailMessage
    {
        public string Subject { get; set; }

        public IList<EmailAddress> ToAddresses { get; set; }

        /// <summary>
        /// Only set if overriding default set in EmailManager
        /// </summary>
        public EmailAddress FromAddress { get; set; }

        public EmailAddress ReplyToAddress { get; set; }

        public string BodyText { get; set; }

        public string BodyHtml { get; set; }

        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
        }
    }
}
