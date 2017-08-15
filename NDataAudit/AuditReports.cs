using System;
using System.Configuration;
using System.Data;
using System.Globalization;
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
        public string CreateUnitTestStyleReport(AuditCollection audits)
        {
            return BuildHtmlBodyForUnitTestReportEmail(audits);
        }

        /// <summary>
        /// Creates the failure report single audit.
        /// </summary>
        /// <param name="audit">The audit.</param>
        /// <param name="auditDataSet">The audit data set.</param>
        /// <returns>System.String.</returns>
        public string CreateFailureReportSingleAudit(Audit audit, DataSet auditDataSet)
        {
            return PrepareResultsSingleAudit(audit, auditDataSet);
        }

        private static string PrepareResultsSingleAudit(Audit testedAudit, DataSet testData)
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
                body.Append(testedAudit.Test.SqlStatementToCheck.ToHtml() + AuditUtils.HtmlBreak);
                body.Append("<b>This query was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            string cleanBody = body.ToString().Replace("\r\n", string.Empty);

            return cleanBody;
        }

        /// <summary>
        /// Builds the HTML body for unit test report email.
        /// </summary>
        /// <param name="auditGroup">The audit group.</param>
        /// <returns>System.String.</returns>
        public static string BuildHtmlBodyForUnitTestReportEmail(AuditCollection auditGroup)
        {
            var body = new StringBuilder();

            body.AppendFormat("<h1>" + auditGroup.AuditGroupName + "</h1>");

            body.Append("These audits ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                        AuditUtils.HtmlBreak);

            body.Append("<b>This report was run on: " + auditGroup.ConnectionString.DatabaseServer + "</b>" + AuditUtils.HtmlBreak);

            StringBuilder database = new StringBuilder();

            switch (auditGroup.ConnectionString.DatabaseProviderName)
            {
                case "mysql.data.mysqlclient":
                    database.Append("<table><tr><td>");
                    database.Append(
                        "<img src=https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_MySQL.png>");
                    database.Append("</td><td>");
                    database.Append("Database Engine is Oracle MySQL");
                    database.Append("</td></tr></table>");
                    break;
                default:
                    database.Append("Database Engine is UNKNOWN");
                    break;
            }

            body.Append(database + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);

            if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
            {
                body.AppendLine("<style>");
                body.AppendLine(auditGroup.TemplateColorScheme.CssTableStyle);
                body.AppendLine("</style>");
                body.Append("<TABLE id=emailtable>");
                body.Append("<TR>");
            }
            else
            {
                body.Append("<TABLE BORDER=1>");
                body.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
            }

            if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
            {
                body.Append("<TH>Status</TH>");
            }
            else
            {
                body.Append("<TD style='white-space: nowrap;' bgcolor=\"" +
                            auditGroup.TemplateColorScheme.HtmlHeaderBackgroundColor +
                            "\"><B>");
                body.Append("<font color=\"" + auditGroup.TemplateColorScheme.HtmlHeaderFontColor + "\">Status</font>");

                body.Append("</B></TD>");
            }

            if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
            {
                body.Append("<TH>Audit Name</TH>");
            }
            else
            {
                body.Append("<TD style='white-space: nowrap;' bgcolor=\"" +
                            auditGroup.TemplateColorScheme.HtmlHeaderBackgroundColor +
                            "\"><B>");
                body.Append("<font color=\"" + auditGroup.TemplateColorScheme.HtmlHeaderFontColor + "\">Audit Name</font>");

                body.Append("</B></TD>");
            }

            body.Append("</TR>");

            foreach (Audit currentAudit in auditGroup)
            {
                body.Append("<TR>");

                // Status Icon
                string testResult;
                string sql = string.Empty;
                string errorMessage = string.Empty;
                string help = string.Empty;

                if (currentAudit.Result)
                {
                    testResult = "<img src={pass}>";
                }
                else
                {
                    testResult = "<img src={fail}>";
                    sql = currentAudit.Test.SqlStatementToCheck.ToHtml();
                    errorMessage = currentAudit.Test.FailedMessage;
                    help = currentAudit.Test.Instructions;
                }

                if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
                {
                    body.Append("<TD>");
                    body.Append(testResult);
                }
                else
                {
                    body.Append("<TD style='white-space: nowrap;'>");
                    body.Append(testResult);
                }

                body.Append("</TD>");

                // Audit Name
                if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
                {
                    body.Append("<TD>");
                    body.Append(currentAudit.Name);
                }
                else
                {
                    body.Append("<TD style='white-space: nowrap;'>");
                    body.Append(currentAudit.Name);
                }

                body.Append("</TD>");

                body.Append("</TR>");

                // Failed Audit Information
                if (!currentAudit.Result)
                {
                    // Info icon
                    if (!string.IsNullOrEmpty(auditGroup.TemplateColorScheme.CssTableStyle))
                    {
                        body.Append("<TD>");
                        body.Append("<img src={info}>");
                    }
                    else
                    {
                        body.Append("<TD style='white-space: nowrap;'>");
                        body.Append("<img src={info}>");
                    }

                    body.Append("</TD>");

                    // Failure Information
                    body.Append("<TD>");
                    body.Append("<TABLE BORDER=1>");

                    // Error Message Icon
                    body.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
                    body.Append("<TD>");
                    body.Append("<img src={error}>");
                    body.Append("</TD>");

                    // Error Message Text
                    body.Append("<TD>");
                    body.Append(errorMessage);
                    body.Append("</TD>");

                    body.Append("</TR>");

                    // SQL Icon
                    body.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
                    body.Append("<TD>");
                    body.Append("<img src={sql}>");
                    body.Append("</TD>");

                    // SQL Text
                    body.Append("<TD>");
                    body.Append(sql);
                    body.Append("</TD>");
                    body.Append("</TR>");

                    // Help Icon
                    body.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
                    body.Append("<TD>");
                    body.Append("<img src={help}>");
                    body.Append("</TD>");

                    // Comment or Instruction Text
                    body.Append("<TD>");
                    body.Append(help);
                    body.Append("</TD>");
                    body.Append("</TR>");

                    body.Append("</TABLE>");
                    body.Append("</TD>");
                }
            }

            body.Append("</TABLE>");

            string htmlBody = body.ToString();
            htmlBody = htmlBody.Replace("{pass}", "https://cdn.rawgit.com/hectorsosajr/NDataAudit/c74445b1/images/32_database-check.png");
            htmlBody = htmlBody.Replace("{fail}", "https://cdn.rawgit.com/hectorsosajr/NDataAudit/c74445b1/images/32_database-fail.png");
            htmlBody = htmlBody.Replace("{sql}",
                "https://cdn.rawgit.com/hectorsosajr/NDataAudit/72a07bd7/images/32_database-sql.png");
            htmlBody = htmlBody.Replace("{info}",
                "https://cdn.rawgit.com/hectorsosajr/NDataAudit/72a07bd7/images/32_database-info.png");
            htmlBody = htmlBody.Replace("{error}", "https://cdn.rawgit.com/hectorsosajr/NDataAudit/72a07bd7/images/32_database-error.png");
            htmlBody = htmlBody.Replace("{help}", "https://cdn.rawgit.com/hectorsosajr/NDataAudit/72a07bd7/images/32_database-help.png");

            htmlBody = htmlBody.Replace("\r\n", string.Empty);
            htmlBody = htmlBody.Replace(@"\", string.Empty);

            return htmlBody;
        }
    }
}
