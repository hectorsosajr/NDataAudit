using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace NDataAudit.Framework
{
    /// <summary>
    /// AuditReports is a class to generate a pass-fail report for an audit group.
    /// </summary>
    public class AuditReports
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditReports"/> class.
        /// </summary>
        public AuditReports()
        { }

        /// <summary>
        /// Creates the report.
        /// </summary>
        /// <param name="audits">The audits.</param>
        /// <returns>System.String.</returns>
        public string CreateReport(AuditCollection audits)
        {
            string retval = string.Empty;



            return retval;
        }

        /// <summary>
        /// Creates the failure report single audit.
        /// </summary>
        /// <param name="audit">The audits.</param>
        /// <returns>System.String.</returns>
        public string CreateFailureReportSingleAudit(Audit audit)
        {
            string retval = string.Empty;


            return retval;
        }

        private static string PrepareResultsSingleAudit(string sqlTested, Audit testedAudit, DataSet testData)
        {
            var body = new StringBuilder();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

            if (testedAudit.Test.SendReport)
            {
                testedAudit.ShowThresholdMessage = false;
                testedAudit.ShowQueryMessage = false;

                if (testedAudit.EmailSubject != null)
                {
                    body.AppendLine("<h2>" + testedAudit.EmailSubject + "</h2>");
                }

                body.Append("This report ran at " +
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                            AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowThresholdMessage)
            {
                body.AppendLine("<h2>ERROR MESSAGE</h2>");
                body.Append(testedAudit.Test.FailedMessage + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowCommentMessage)
            {
                body.AppendLine("COMMENTS AND INSTRUCTIONS" + AuditUtils.HtmlBreak);
                body.AppendLine("============================" + AuditUtils.HtmlBreak);

                if (testedAudit.Test.Instructions != null)
                {
                    if (testedAudit.Test.Instructions.Length > 0)
                    {
                        body.Append(testedAudit.Test.Instructions.ToHtml() + AuditUtils.HtmlBreak);
                        body.AppendLine(AuditUtils.HtmlBreak);
                    }
                }
            }

            if (testedAudit.IncludeDataInEmail)
            {
                if (testData.Tables.Count > 0)
                {
                    EmailTableTemplate currTemplate = testedAudit.Test.TemplateColorScheme;

                    string htmlData = AuditUtils.CreateHtmlData(testedAudit, testData, currTemplate);

                    body.Append(htmlData);
                }
            }

            body.AppendLine(AuditUtils.HtmlBreak);

            if (testedAudit.Test.SendReport)
            {
                body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
                body.Append("<b>This report was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }
            else
            {
                body.Append("This audit ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowQueryMessage)
            {
                body.Append(AuditUtils.HtmlBreak);
                body.Append("The '" + testedAudit.Name + "' audit has failed. The following SQL statement was used to test this audit :" + AuditUtils.HtmlBreak);
                body.Append(sqlTested.ToHtml() + AuditUtils.HtmlBreak);
                body.Append("<b>This query was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            string cleanBody = body.ToString().Replace("\r\n", string.Empty);

            //SendEmail(testedAudit, cleanBody, sourceEmailDescription);

            return cleanBody;
        }

        //private static void SendEmail(Audit testedAudit, string body, string sourceEmailDescription)
        //{
        //    var message = new MailMessage { IsBodyHtml = true };

        //    foreach (string recipient in _colAudits.EmailSubscribers)
        //    {
        //        message.To.Add(new MailAddress(recipient));
        //    }

        //    if (_colAudits.EmailCarbonCopySubscribers != null)
        //    {
        //        // Carbon Copies - CC
        //        foreach (string ccemail in _colAudits.EmailCarbonCopySubscribers)
        //        {
        //            message.CC.Add(new MailAddress(ccemail));
        //        }
        //    }

        //    if (_colAudits.EmailBlindCarbonCopySubscribers != null)
        //    {
        //        // Blind Carbon Copies - BCC
        //        foreach (string bccemail in _colAudits.EmailBlindCarbonCopySubscribers)
        //        {
        //            message.Bcc.Add(new MailAddress(bccemail));
        //        }
        //    }

        //    message.Body = body;

        //    switch (testedAudit.EmailPriority)
        //    {
        //        case EmailPriorityEnum.Low:
        //            message.Priority = MailPriority.Low;
        //            break;
        //        case EmailPriorityEnum.Normal:
        //            message.Priority = MailPriority.Normal;
        //            break;
        //        case EmailPriorityEnum.High:
        //            message.Priority = MailPriority.High;
        //            break;
        //        default:
        //            message.Priority = MailPriority.Normal;
        //            break;
        //    }

        //    if (!string.IsNullOrEmpty(testedAudit.EmailSubject))
        //    {
        //        message.Subject = testedAudit.EmailSubject;
        //    }
        //    else
        //    {
        //        message.Subject = "Audit Failure - " + testedAudit.Name;
        //    }

        //    message.From = new MailAddress(_colAudits.SmtpSourceEmail, sourceEmailDescription);

        //    var server = new SmtpClient();

        //    if (_colAudits.SmtpHasCredentials)
        //    {
        //        server.UseDefaultCredentials = false;
        //        server.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        server.Host = _colAudits.SmtpServerAddress;
        //        server.Port = _colAudits.SmtpPort;
        //        server.Credentials = new NetworkCredential(_colAudits.SmtpUserName, _colAudits.SmtpPassword);
        //        server.EnableSsl = _colAudits.SmtpUseSsl;
        //    }
        //    else
        //    {
        //        server.Host = _colAudits.SmtpServerAddress;
        //    }

        //    try
        //    {
        //        server.Send(message);
        //    }
        //    catch (SmtpException smtpEx)
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        sb.AppendLine(smtpEx.Message);

        //        if (smtpEx.InnerException != null)
        //        {
        //            sb.AppendLine(smtpEx.InnerException.Message);
        //        }

        //        throw;
        //    }
        //}
    }
}
