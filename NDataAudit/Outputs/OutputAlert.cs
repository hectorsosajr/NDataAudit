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
        public override string CreateOutputBody()
        {
            var body = new StringBuilder();
            string cleanBody = string.Empty;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

            foreach (Audit audit in base.Audits)
            {

                if (audit.EmailSubject != null)
                {
                    body.AppendLine("<h2>" + audit.EmailSubject + "</h2>");
                }

                body.Append("This audit ran at " +
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                            AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);

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
                    if (audit.ResultDataSet.Tables.Count > 0)
                    {
                        EmailTableTemplate currTemplate = audit.Test.TemplateColorScheme;

                        string htmlData = AuditUtils.CreateHtmlData(audit, audit.ResultDataSet, currTemplate);

                        body.Append(htmlData);
                    }
                }

                body.AppendLine(AuditUtils.HtmlBreak);

                body.Append("This alert ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
                body.Append("<b>This alert was run on: " + audit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);

                if (audit.ShowQueryMessage)
                {
                    body.Append(AuditUtils.HtmlBreak);
                    body.Append("The '" + audit.Name + "' audit has failed. The following SQL statement was used to test this alert :" + AuditUtils.HtmlBreak);
                    body.Append(audit.Test.SqlStatementToCheck.ToHtml() + AuditUtils.HtmlBreak);
                    body.Append("<b>This query was run on: " + audit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
                }

                cleanBody = body.ToString().Replace("\r\n", string.Empty);

            }

            return cleanBody;
        }
    }
}
