using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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

         /// <summary>
         /// Gets or sets the CSS table style.
         /// </summary>
         /// <value>The CSS table style.</value>
         public string CssTableStyle { get; set; }
    }

    internal static class AuditUtils
    {
        /// <summary>
        /// Gets the HTML equivalent of line break.
        /// </summary>
        /// <value>The HTML break tag.</value>
        public static string HtmlBreak => "<br/>";

        /// <summary>
        /// Gets the HTML equivalent of space.
        /// </summary>
        /// <value>The HTML non-blanking space tag.</value>
        public static string HtmlSpace => "&nbsp;";

        /// <summary>
        /// Gets the HTML tab.
        /// </summary>
        /// <value>The HTML equivalent of a tab. Made it equal to 4 non-blanking space tags.</value>
        public static string HtmlTab => "&nbsp;&nbsp;&nbsp;&nbsp;";

        /// <summary>
        /// Extension method to convert regular strings into HTML text.
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns>Converted string with HTML tags</returns>
        public static string ToHtml(this string stringToConvert)
        {
            var retval = stringToConvert.Replace(Environment.NewLine, HtmlBreak)
                .Replace(" ", HtmlSpace)
                .Replace("\t", HtmlTab);

            return retval;
        }

        /// <summary>
        /// Creates the HTML content for the email.
        /// </summary>
        /// <param name="testedAudit">The tested audit.</param>
        /// <param name="testData">The test data.</param>
        /// <param name="emailTableTemplate">The email table template.</param>
        /// <returns>System.String.</returns>
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
                sb.AppendFormat(@"</caption>");

                if (!string.IsNullOrEmpty(emailTableTemplate.CssTableStyle))
                {
                    sb.AppendLine("<style>");
                    sb.AppendLine(emailTableTemplate.CssTableStyle);
                    sb.AppendLine("</style>");
                    sb.Append("<TABLE id=emailtable>");
                    sb.Append("<TR>");
                }
                else
                {
                sb.Append("<TABLE BORDER=1>");

                sb.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
                }

                // first append the column names.
                foreach (DataColumn column in currTable.Columns)
                {
                    if (!string.IsNullOrEmpty(emailTableTemplate.CssTableStyle))
                    {
                        sb.Append("<TH>" + column.ColumnName + "</TH>");
                    }
                    else
                    {
                        sb.Append("<TD style='white-space: nowrap;' bgcolor=\"" +
                                  emailTableTemplate.HtmlHeaderBackgroundColor +
                              "\"><B>");
                    sb.Append("<font color=\"" + emailTableTemplate.HtmlHeaderFontColor + "\">" + column.ColumnName +
                              "</font>");

                    sb.Append("</B></TD>");
                }
                }

                sb.Append("</TR>");

                int rowCounter = 1;

                // next, the column values.
                foreach (DataRow row in currTable.Rows)
                {
                    if (!string.IsNullOrEmpty(emailTableTemplate.CssTableStyle))
                    {
                        if (emailTableTemplate.UseAlternateRowColors)
                        {
                            if (rowCounter % 2 == 0)
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
                            sb.Append("<TR>");
                        }
                    }
                    else
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
                    }

                    foreach (DataColumn column in currTable.Columns)
                    {
                        if (!string.IsNullOrEmpty(emailTableTemplate.CssTableStyle))
                        {
                            sb.Append("<TD>");
                            if (row[column].ToString().Trim().Length > 0)
                                sb.Append(row[column]);
                            else
                                sb.Append("&nbsp;");
                        }
                        else
                        {
                        sb.Append("<TD style='white-space: nowrap;'>");
                        if (row[column].ToString().Trim().Length > 0)
                            sb.Append(row[column]);
                        else
                            sb.Append("&nbsp;");
                        }

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

        /// <summary>
        /// Gets the email table templates.
        /// </summary>
        /// <returns>List&lt;EmailTableTemplate&gt;.</returns>
        public static List<EmailTableTemplate> GeTableTemplates()
        {
            var templates = new List<EmailTableTemplate>();

            string templateText = File.ReadAllText(@"TableTemplates.json");
            var results = JObject.Parse(templateText);

            foreach (var template in results["tabletemplates"])
            {
                var currTemplate = new EmailTableTemplate
                {
                    Name = (string) template["Name"],
                    AlternateRowColor = (string) template["AlternateRowColor"],
                    HtmlHeaderBackgroundColor = (string) template["HtmlHeaderBackgroundColor"],
                    HtmlHeaderFontColor = (string) template["HtmlHeaderFontColor"],
                    UseAlternateRowColors = Convert.ToBoolean(template["UseAlternateRowColors"])
                };

                if (template["CssTableStyle"] != null)
                {
                    JArray styleArray = JArray.Parse(template["CssTableStyle"].ToString());
                    StringBuilder sb = new StringBuilder();

                    foreach (var line in styleArray)
                    {
                        sb.AppendLine(line.ToString());
                    }

                    string stageCss = sb.ToString();
                    stageCss = stageCss.Replace("\r\n", string.Empty).Replace(@"\", string.Empty);

                    currTemplate.CssTableStyle = stageCss;
                }

                templates.Add(currTemplate);
            }

            templates.Add(GetDefaultTemplate());
            
            return templates;
        }

        /// <summary>
        /// Gets the default email table template.
        /// </summary>
        /// <returns>EmailTableTemplate.</returns>
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