using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NDataAudit.Framework
{
    /// <summary>
    /// An enum to give the different hard-coded template easy to use
    /// names.
    /// </summary>
    public enum TableTemplateNames
    {
        /// <summary>
        /// Default template color scheme. RED header
        /// background with white font.
        /// </summary>
        Default,

        /// <summary>
        /// Red header with white text, and alternating row colors.
        /// </summary>
        RedReport,

        /// <summary>
        /// Yellow header with black font.
        /// </summary>
        Yellow,

        /// <summary>
        /// Yellow header with black font, and alternating row colors.
        /// </summary>
        YellowReport,

        /// <summary>
        /// Green header with white font.
        /// </summary>
        Green,

        /// <summary>
        /// Green header with white font, and alternating row colors.
        /// </summary>
        GreenReport,

        /// <summary>
        /// Blue header with white, and alternating row colors.
        /// </summary>
        BlueReport,
    }

    /// <summary>
    /// Template used to color the HTML table in CreateHtmlData.
    /// </summary>
    public struct TableTemplate
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
        public static string CreateHtmlData(Audit testedAudit, DataSet testData, TableTemplate tableTemplate)
        {
            var sb = new StringBuilder();

            if (tableTemplate.Equals(null))
            {
                tableTemplate = GetDefaultTemplate();
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
                    sb.Append("<TD style='white-space: nowrap;' bgcolor=\"" + tableTemplate.HtmlHeaderBackgroundColor +
                              "\"><B>");
                    sb.Append("<font color=\"" + tableTemplate.HtmlHeaderFontColor + "\">" + column.ColumnName +
                              "</font>");
                    sb.Append("</B></TD>");
                }

                sb.Append("</TR>");

                int rowCounter = 1;

                // next, the column values.
                foreach (DataRow row in currTable.Rows)
                {
                    if (tableTemplate.UseAlternateRowColors)
                    {
                        if (rowCounter%2 == 0)
                        {
                            // Even numbered row, so tag it with a different background color.
                            sb.Append("<TR style='white-space: nowrap;' ALIGN='LEFT' bgcolor=\"" +
                                      tableTemplate.AlternateRowColor + "\">");
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

        public static List<TableTemplate> GeTableTemplates()
        {
            var templates = new List<TableTemplate>();

            string templateText = File.ReadAllText(@"TableTemplates.json");
            var results = JObject.Parse(templateText);

            foreach (var template in results["tabletemplates"])
            {
                TableTemplate currTemplate = new TableTemplate
                {
                    Name = (string) template["Name"],
                    AlternateRowColor = (string) template["AlternateRowColor"],
                    HtmlHeaderBackgroundColor = (string) template["HtmlHeaderBackgroundColor"],
                    HtmlHeaderFontColor = (string) template["HtmlHeaderFontColor"]
                };

                currTemplate.UseAlternateRowColors = Convert.ToBoolean(template["UseAlternateRowColors"]);

                templates.Add(currTemplate);
            }

            templates.Add(GetDefaultTemplate());
            
            return templates;
        }

        public static TableTemplate GetDefaultTemplate()
        {
            var template = new TableTemplate
            {
                Name = "Default",
                HtmlHeaderBackgroundColor = "FF0000",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = false
            };

            return template;
        }

        public static TableTemplate GetRedReportTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "FF0000",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = true,
                AlternateRowColor = "F2F2F2"
            };

            return template;
        }

        public static TableTemplate GetYellowTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "FFFF00",
                HtmlHeaderFontColor = "black",
                UseAlternateRowColors = false
            }; 

            return template;
        }

        public static TableTemplate GetYellowReportTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "FFFF00",
                HtmlHeaderFontColor = "black",
                UseAlternateRowColors = true,
                AlternateRowColor = "F2F2F2"
            };

            return template;
        }

        public static TableTemplate GetGreenTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "13913F",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = false
            };

            return template;
        }

        public static TableTemplate GetGreenReportTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "70AD47",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = true,
                AlternateRowColor = "E2EFDA"
            };

            return template;
        }

        public static TableTemplate GetBlueReportTemplate()
        {
            var template = new TableTemplate
            {
                HtmlHeaderBackgroundColor = "5B9BD5",
                HtmlHeaderFontColor = "white",
                UseAlternateRowColors = true,
                AlternateRowColor = "DDEBF7"
            };

            return template;
        }
    }
}