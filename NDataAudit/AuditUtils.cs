using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NDataAudit.Framework
{
    /// <summary>
    /// Template used to color the HTML table in CreateHtmlData.
    /// </summary>
    public struct EmailTableTemplate
    {
        /// <summary>
        /// Gets or sets the name of this template. This is done so that it is human readable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color of the HTML header font.
        /// </summary>
        /// <value>
        /// The color of the HTML header font.
        /// </value>
        public string HtmlHeaderFontColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the HTML header background.
        /// </summary>
        /// <value>
        /// The color of the HTML header background.
        /// </value>
        public string HtmlHeaderBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use alternate row colors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if true use alternate row colors; otherwise, <c>false</c>.
        /// </value>
        public bool UseAlternateRowColors { get; set; }

        /// <summary>
        /// Gets or sets the HTML background color for the alternate row, the TR tags.
        /// </summary>
        /// <value>
        /// The color of the alternate row.
        /// </value>
        public string AlternateRowColor { get; set; }
    }

    internal static class AuditUtils
    {
        public static string HtmlBreak => "<br/>";

        public static string HtmlSpace => "&nbsp;";

        public static string HtmlTab => "&nbsp;&nbsp;&nbsp;&nbsp;";

        /// <summary>
        /// Extension method to convert regular strings into HTML text.
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns>Converted string with HTML tags</returns>
        public static string ToHtml(this string stringToConvert)
        {
            string retval = string.Empty;

            retval = stringToConvert.Replace(Environment.NewLine, HtmlBreak)
                .Replace(" ", HtmlSpace)
                .Replace("\t", HtmlTab);

            return retval;
        }

        public static string CreateHtmlData(Audit testedAudit, DataSet testData, EmailTableTemplate emailTableTemplate)
        {
            var sb = new StringBuilder();

            if (emailTableTemplate.Equals(null))
            {
                emailTableTemplate = GetDefaultTemplate();
            }

            int tableNamesCount = 0;

            foreach (DataTable currTable in testData.Tables)
            {
                if (testedAudit.Tests[0].MultipleResults)
                {
                    sb.Append("<B>");
                    sb.Append(testedAudit.Tests[0].TableNames[tableNamesCount]);
                    sb.Append("</B>");
                    sb.AppendLine("<br>");
                }

                sb.AppendFormat(@"<caption> Total Rows = ");
                sb.AppendFormat(currTable.Rows.Count.ToString(CultureInfo.InvariantCulture));
                sb.AppendFormat(@"  </caption>");

                sb.Append("<TABLE BORDER=1>");

                sb.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");

                // first append the column names.
                foreach (DataColumn column in currTable.Columns)
                {
                    sb.Append("<TD style='white-space: nowrap;' bgcolor=\"" + emailTableTemplate.HtmlHeaderBackgroundColor +
                              "\"><B>");
                    sb.Append("<font color=\"" + emailTableTemplate.HtmlHeaderFontColor + "\">" + column.ColumnName +
                              "</font>");
                    sb.Append("</B></TD>");
                }

                sb.Append("</TR>");

                int rowCounter = 1;

                // next, the column values.
                foreach (DataRow row in currTable.Rows)
                {
                    if (emailTableTemplate.UseAlternateRowColors)
                    {
                        if (rowCounter%2 == 0)
                        {
                            // Even numbered row, so tag it with a different background color.
                            sb.Append("<TR style='white-space: nowrap;' ALIGN='LEFT' bgcolor=\"" +
                                      emailTableTemplate.AlternateRowColor + "\">");
                        }
                        else
                        {
                            sb.Append("<TR style='white-space: nowrap;' ALIGN='LEFT'>");
                        }
                    }
                    else
                    {
                        sb.Append("<TR style='white-space: nowrap;' ALIGN='LEFT'>");
                    }

                    foreach (DataColumn column in currTable.Columns)
                    {
                        sb.Append("<TD style='white-space: nowrap;'>");
                        if (row[column].ToString().Trim().Length > 0)
                            sb.Append(row[column]);
                        else
                            sb.Append("&nbsp;");
                        sb.Append("</TD>");
                    }

                    sb.Append("</TR>");

                    rowCounter++;
                }

                sb.Append("</TABLE>");
                sb.Append("<br>");

                tableNamesCount++;
            }

            return sb.ToString();
        }

        public static List<EmailTableTemplate> GeTableTemplates()
        {
            var templates = new List<EmailTableTemplate>();

            string templateText = File.ReadAllText(@"TableTemplates.json");
            var results = JObject.Parse(templateText);

            foreach (var template in results["tabletemplates"])
            {
                EmailTableTemplate currTemplate = new EmailTableTemplate
                {
                    Name = (string) template["Name"],
                    AlternateRowColor = (string) template["AlternateRowColor"],
                    HtmlHeaderBackgroundColor = (string) template["HtmlHeaderBackgroundColor"],
                    HtmlHeaderFontColor = (string) template["HtmlHeaderFontColor"],
                    UseAlternateRowColors = Convert.ToBoolean(template["UseAlternateRowColors"])
                };

                templates.Add(currTemplate);
            }

            templates.Add(GetDefaultTemplate());
            
            return templates;
        }

        public static EmailTableTemplate GetDefaultTemplate()
        {
            var template = new EmailTableTemplate
            {
                Name = "Default",
                HtmlHeaderBackgroundColor = "FF0000",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = false
            };

            return template;
        }
    }
}