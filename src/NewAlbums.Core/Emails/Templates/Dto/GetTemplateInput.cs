using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Emails.Templates.Dto
{
    public class GetTemplateInput
    {
        /// <summary>
        /// One of <see cref="TemplateTypes"/>
        /// </summary>
        public string TemplateType { get; set; }

        public Dictionary<string, string> SimpleVariableValues { get; set; }

        public IList<BodyParagraph> BodyParagraphs { get; set; }

        public GetTemplateInput()
        {
            BodyParagraphs = new List<BodyParagraph>();
        }
    }
}
