using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Emails.Templates.Dto
{
    public class BodyParagraph
    {
        public string Text { get; set; }

        /// <summary>
        /// If this is set, the Text is displayed as a button
        /// </summary>
        public string ButtonUrl { get; set; }
    }
}
