//*********************************************************************
// File:       		OutputUnitTest.cs
// Author:  	    Hector Sosa, Jr
// Date:			4/26/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		4/26/2018	    Created
//*********************************************************************

using System;
using System.Globalization;
using System.Text;

namespace NDataAudit.Framework.Outputs
{
    /// <summary>Class OutputUnitTest.</summary>
    public class OutputUnitTest : AuditOutputBase
    {
        /// <summary>Initializes a new instance of the AuditOutputBase class.</summary>
        /// <param name="audits">The audits objects that will be used for the output.</param>
        public OutputUnitTest(AuditCollection audits) : base(audits)
        {}

        /// <summary>Creates the output body.</summary>
        /// <returns>System.String.</returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override string CreateOutputBody()
        {
            var body = new StringBuilder();

            body.AppendFormat("<h1>" + Audits.AuditGroupName + "</h1>");

            body.Append("These audits ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                        AuditUtils.HtmlBreak);

            body.Append("<b>This report was run on: " + Audits.ConnectionString.DatabaseServer + "</b>" + AuditUtils.HtmlBreak);

            StringBuilder database = new StringBuilder();

            switch (Audits.ConnectionString.DatabaseProviderName)
            {
                case "mysql.data.mysqlclient":
                    database.Append("<table><tr><td>");
                    database.Append(
                        "<img src=https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_MySQL.png>");
                    database.Append("</td><td>");
                    database.Append("Database Engine is Oracle MySQL");
                    database.Append("</td></tr></table>");
                    break;
                case "system.data.sqlclient":
                    database.Append("<table><tr><td>");
                    database.Append(
                        "<img src=https://cdn.rawgit.com/hectorsosajr/NDataAudit/87edd0dc/images/32_SQLServer.png>");
                    database.Append("</td><td>");
                    database.Append("Database Engine is Microsoft SQL Server");
                    database.Append("</td></tr></table>");
                    break;
                default:
                    database.Append("Database Engine is UNKNOWN");
                    break;
            }

            body.Append(database + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);

            if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
            {
                body.AppendLine("<style>");
                body.AppendLine(Audits.TemplateColorScheme.CssTableStyle);
                body.AppendLine("</style>");
                body.Append("<TABLE id=emailtable BORDER=1>");
                body.Append("<TR>");
            }
            else
            {
                body.Append("<TABLE BORDER=1 width=\"100%\">");
                body.Append("<TR ALIGN='LEFT' style='white-space: nowrap;'>");
            }

            if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
            {
                body.Append("<TH>Status</TH>");
            }
            else
            {
                body.Append("<TD style='white-space: nowrap;' bgcolor=\"" +
                            Audits.TemplateColorScheme.HtmlHeaderBackgroundColor +
                            "\"><B>");
                body.Append("<font color=\"" + Audits.TemplateColorScheme.HtmlHeaderFontColor + "\">Status</font>");

                body.Append("</B></TD>");
            }

            if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
            {
                body.Append("<TH>Audit Name</TH>");
            }
            else
            {
                body.Append("<TD style='white-space: nowrap;' bgcolor=\"" +
                            Audits.TemplateColorScheme.HtmlHeaderBackgroundColor +
                            "\"><B>");
                body.Append("<font color=\"" + Audits.TemplateColorScheme.HtmlHeaderFontColor + "\">Audit Name</font>");

                body.Append("</B></TD>");
            }

            body.Append("</TR>");

            foreach (Audit currentAudit in Audits)
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

                if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
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
                if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
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
                    if (!string.IsNullOrEmpty(Audits.TemplateColorScheme.CssTableStyle))
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
