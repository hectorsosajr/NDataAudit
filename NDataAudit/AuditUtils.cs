//*********************************************************************
// File:       		AuditUtils.cs
// Author:  	    Hector Sosa, Jr
// Date:			1/31/2013
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE        COMMENTS
// Hector Sosa, Jr		1/31/2013	Created
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using NDataAudit.Framework.Outputs;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

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

    /// <summary>
    ///  The type of email client to target
    /// </summary>
    public enum EmailClientHtml
    {
        /// <summary>
        /// No email client targetted
        /// </summary>
        None = 0,

        /// <summary>
        /// Gmail Web client
        /// </summary>
        GmailWeb = 1,

        /// <summary>
        /// Gmail Android client
        /// </summary>
        GmailAndroid = 2,

        /// <summary>
        /// Microsoft Outlook
        /// </summary>
        Outlook = 3
    }

    /// <summary>
    /// Type of email priority
    /// </summary>
    public enum EmailPriorityEnum
    {
        /// <summary>
        /// The low email priority
        /// </summary>
        Low = 0,

        /// <summary>
        /// The normal email priority
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The high email priority
        /// </summary>
        High = 2
    }

    /// <summary>
    /// Type of email entry
    /// </summary>
    public enum EmailTypeEnum
    {
        /// <summary>
        /// Regular email recipient
        /// </summary>
        Recipient,

        /// <summary>
        /// Carbon Copy email recipient
        /// </summary>
        CarbonCopy,

        /// <summary>
        /// Blind carbon copy email recipient
        /// </summary>
        BlindCarbonCopy
    }

    /// <summary>
    /// What type of output do we want for our audits.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// Unit test style where you get pass-fail status for each audit in the audit group.
        /// </summary>
        UnitTest,

        /// <summary>
        /// Only report on failing audits.
        /// </summary>
        Alert,

        /// <summary>
        /// The original audit where a threshold is set. This is a pass or fail system.
        /// </summary>
        Audit,

        /// <summary>
        /// This is a simple report using the same output templates as unit tests and alerts.
        /// </summary>
        Report
    }

    /// <summary>
    /// Class AuditUtils.
    /// </summary>
    public static class AuditUtils
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
        /// Gets the quote character.
        /// </summary>
        /// <value>The quote.</value>
        public static string Quote => @"""";

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


            // Keep IMG tags intact.
            Regex regexObj = new Regex("<img.+?>", RegexOptions.IgnoreCase);
            Match matchResults = regexObj.Match(retval);
            while (matchResults.Success)
            {
                string currMatch = matchResults.Value.Replace(@"\", string.Empty)
                    .Replace(HtmlSpace, " ");
                retval = retval.Replace(matchResults.Value, currMatch);
                matchResults = matchResults.NextMatch();
            }

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
                if (testedAudit.Test.MultipleResults)
                {
                    sb.Append("<B>");
                    sb.Append(testedAudit.Test.TableNames[tableNamesCount]);
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

        /// <summary>
        /// Sends the output of the <see cref="Audit"/> result. Currently only through emails.
        /// </summary>
        /// <param name="auditGroup">The list of <see cref="Audit"/>s that were tested.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SendResult(AuditCollection auditGroup)
        {
            var succeed = false;

            try
            {
                string htmlBody;

                switch (auditGroup.AuditResultOutputType)
                {
                    case OutputType.UnitTest:
                        OutputUnitTest unitTest = new OutputUnitTest(auditGroup);
                        htmlBody = unitTest.CreateOutputBody();
                        break;
                    case OutputType.Alert:
                        OutputAlert alert = new OutputAlert(auditGroup);
                        htmlBody = alert.CreateOutputBody();
                        break;
                    case OutputType.Audit:
                        OutputAudit audit = new OutputAudit(auditGroup);
                        htmlBody = audit.CreateOutputBody();
                        break;
                    case OutputType.Report:
                        OutputReport report = new OutputReport(auditGroup);
                        htmlBody = report.CreateOutputBody();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var mailClient = CreateMailMessage(out var message, auditGroup, htmlBody);

                mailClient.Send(message);

                succeed = true;
            }
            catch (SmtpException smtpEx)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(smtpEx.Message);

                if (smtpEx.InnerException != null)
                {
                    sb.AppendLine(smtpEx.InnerException.Message);
                }

                succeed = false;

                var logger = GetFileLogger();
                logger.Error(smtpEx, sb.ToString());
            }
            catch (Exception ex)
            {
                succeed = false;

                var logger = GetFileLogger();
                logger.Error(ex, ex.Message);
            }

            return succeed;
        }

        private static SmtpClient CreateMailMessage(out MailMessage message, AuditCollection auditGroup, string body)
        {
            message = new MailMessage
            {
                IsBodyHtml = true,
                Subject = $"Audit Results for {auditGroup.AuditGroupName}"
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;


            foreach (string recipient in auditGroup.EmailSubscribers)
            {
                message.To.Add(new MailAddress(recipient));
            }

            if (auditGroup.EmailCarbonCopySubscribers != null)
            {
                // Carbon Copies - CC
                foreach (string ccemail in auditGroup.EmailCarbonCopySubscribers)
                {
                    message.CC.Add(new MailAddress(ccemail));
                }
            }

            if (auditGroup.EmailBlindCarbonCopySubscribers != null)
            {
                // Blind Carbon Copies - BCC
                foreach (string bccemail in auditGroup.EmailBlindCarbonCopySubscribers)
                {
                    message.Bcc.Add(new MailAddress(bccemail));
                }
            }

            message.Body = body;

            switch (auditGroup.EmailPriority)
            {
                case EmailPriorityEnum.Low:
                    message.Priority = MailPriority.Low;
                    break;
                case EmailPriorityEnum.Normal:
                    message.Priority = MailPriority.Normal;
                    break;
                case EmailPriorityEnum.High:
                    message.Priority = MailPriority.High;
                    break;
                default:
                    message.Priority = MailPriority.Normal;
                    break;
            }

            message.From = new MailAddress(auditGroup.SmtpSourceEmail, sourceEmailDescription);

            var smtpClient = new SmtpClient();

            if (auditGroup.SmtpHasCredentials)
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Host = auditGroup.SmtpServerAddress;
                smtpClient.Port = auditGroup.SmtpPort;
                smtpClient.Credentials = new NetworkCredential(auditGroup.SmtpUserName, auditGroup.SmtpPassword);
                smtpClient.EnableSsl = auditGroup.SmtpUseSsl;
            }
            else
            {
                smtpClient.Host = auditGroup.SmtpServerAddress;
            }

            return smtpClient;
        }

        /// <summary>
        /// Gets the file logger.
        /// </summary>
        /// <returns>Logger.</returns>
        public static Logger GetFileLogger()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            fileTarget.FileName = "${basedir}/logs/ndataaudit.${shortdate}.log";
            fileTarget.Layout = "[${shortdate}] ${level} ${logger} ${message}";
            fileTarget.ArchiveFileName = "${basedir}/logs/archives/logfile.{#}.txt";
            fileTarget.ArchiveEvery = FileArchivePeriod.Day;

            var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(fileRule);

            LogManager.Configuration = config;

            Logger logger = LogManager.GetLogger("FileLogger");

            return logger;
        }

        /// <summary>
        /// Gets the console logger.
        /// </summary>
        /// <returns>Logger.</returns>
        public static Logger GetConsoleLogger()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();

            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${level} ${logger} ${message}";

            var consoleRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            LogManager.Configuration = config;

            Logger logger = LogManager.GetLogger("ConsoleLogger");

            return logger;
        }
    }
}