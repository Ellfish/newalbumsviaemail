using Microsoft.Extensions.Configuration;
using NewAlbums.Configuration;
using NewAlbums.Emails.Templates;
using NewAlbums.Emails.Templates.Dto;
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

        private const string BodyRowHtml = "<tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">";
        private const string BodyCellHtml = "<td class=\"content-block\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box;" 
                                            + "font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;\" valign=\"top\">";

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
                    var template = new StringBuilder(Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3));

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
                bodyHtml.Append($"{BodyRowHtml}{BodyCellHtml}");

                if (String.IsNullOrWhiteSpace(paragraph.ButtonUrl))
                {
                    bodyHtml.Append($"{paragraph.Text}");
                }
                else
                {
                    bodyHtml.Append($"<a href=\"{paragraph.ButtonUrl}\" class=\"btn-primary\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; "
                        + $"box-sizing: border-box; font-size: 14px; color: {textColourOnPrimary}; text-decoration: none; line-height: 2em; font-weight: bold; " 
                        + $"text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize; background-color: {primaryColour}; "
                        + $"margin: 0; border-color: {primaryColour}; border-style: solid; border-width: 10px 20px;\">{paragraph.Text}</a>");
                }

                bodyHtml.Append("</td></tr>");
            }

            template.Replace($"{{{TemplateVariables.Body}}}", bodyHtml.ToString());
        }
    }
}
