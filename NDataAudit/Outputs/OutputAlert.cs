//*********************************************************************
// File:       		OutputAlert.cs
// Author:  	    Hector Sosa, Jr
// Date:			4/26/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		4/26/2018	    Created
//*********************************************************************

using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;

namespace NDataAudit.Framework.Outputs
{
    /// <summary>Class OutputAlert.</summary>
    public class OutputAlert : AuditOutputBase
    {
        /// <summary>Initializes a new instance of the AuditOutputBase class.</summary>
        /// <param name="audits">The audits objects that will be used for the output.</param>
        public OutputAlert(AuditCollection audits) : base(audits)
        {}

        /// <summary>
        /// Creates the output body.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string CreateOutputBody()
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates the output body.
        /// </summary>
        /// <param name="audit">The audit.</param>
        /// <param name="auditDataSet">The audit data set.</param>
        /// <returns>System.String.</returns>
        protected string CreateOutputBody(Audit audit, DataSet auditDataSet)
        {
            var body = new StringBuilder();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

            if (audit.Test.SendReport)
            {
                audit.ShowThresholdMessage = false;
                audit.ShowQueryMessage = false;

                if (audit.EmailSubject != null)
                {
                    body.AppendLine("<h2>" + audit.EmailSubject + "</h2>");
                }

                body.Append("This report ran at " +
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                            AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (audit.ShowThresholdMessage)
            {
                body.AppendLine("<h2>ERROR MESSAGE</h2>");
                body.Append(audit.Test.FailedMessage + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (audit.ShowCommentMessage)
            {
                body.AppendLine("COMMENTS AND INSTRUCTIONS" + AuditUtils.HtmlBreak);
                body.AppendLine("============================" + AuditUtils.HtmlBreak);

                if (audit.Test.Instructions != null)
                {
                    if (audit.Test.Instructions.Length > 0)
                    {
                        body.Append(audit.Test.Instructions.ToHtml() + AuditUtils.HtmlBreak);
                        body.AppendLine(AuditUtils.HtmlBreak);
                    }
                }
            }

            if (audit.IncludeDataInEmail)
            {
                if (auditDataSet.Tables.Count > 0)
                {
                    EmailTableTemplate currTemplate = audit.Test.TemplateColorScheme;

                    string htmlData = AuditUtils.CreateHtmlData(audit, auditDataSet, currTemplate);

                    body.Append(htmlData);
                }
            }

            body.AppendLine(AuditUtils.HtmlBreak);

            if (audit.Test.SendReport)
            {
                body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
                body.Append("<b>This report was run on: " + audit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }
            else
            {
                body.Append("This audit ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
            }

            if (audit.ShowQueryMessage)
            {
                body.Append(AuditUtils.HtmlBreak);
                body.Append("The '" + audit.Name + "' audit has failed. The following SQL statement was used to test this audit :" + AuditUtils.HtmlBreak);
                body.Append(audit.Test.SqlStatementToCheck.ToHtml() + AuditUtils.HtmlBreak);
                body.Append("<b>This query was run on: " + audit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            string cleanBody = body.ToString().Replace("\r\n", string.Empty);

            return cleanBody;
        }
    }
}
