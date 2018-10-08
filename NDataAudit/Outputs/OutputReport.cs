//*********************************************************************
// File:       		OutputReport.cs
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
using System.Globalization;
using System.Text;

namespace NDataAudit.Framework.Outputs
{
    /// <summary>
    /// Class OutputReport.
    /// </summary>
    /// <seealso cref="NDataAudit.Framework.AuditOutputBase" />
    public class OutputReport : AuditOutputBase
    {
        /// <summary>
        /// Initializes a new instance of the OutputReport class.
        /// </summary>
        /// <param name="audits">The audits objects that will be used for the output.</param>
        public OutputReport(AuditCollection audits) : base(audits)
        {}

        /// <summary>
        /// Creates the body.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string CreateOutputBody()
        {
            var body = new StringBuilder();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

            Audits[0].ShowThresholdMessage = false;
            Audits[0].ShowQueryMessage = false;

            if (Audits[0].EmailSubject != null)
            {
                body.AppendLine("<h2>" + Audits[0].EmailSubject + "</h2>");
            }

            body.Append("This report ran at " +
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
                        AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);

            if (Audits[0].ShowThresholdMessage)
            {
                body.AppendLine("<h2>ERROR MESSAGE</h2>");
                body.Append(Audits[0].Test.FailedMessage + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (Audits[0].ShowCommentMessage)
            {
                body.AppendLine("COMMENTS AND INSTRUCTIONS" + AuditUtils.HtmlBreak);
                body.AppendLine("============================" + AuditUtils.HtmlBreak);

                if (Audits[0].Test.Instructions != null)
                {
                    if (Audits[0].Test.Instructions.Length > 0)
                    {
                        body.Append(Audits[0].Test.Instructions.ToHtml() + AuditUtils.HtmlBreak);
                        body.AppendLine(AuditUtils.HtmlBreak);
                    }
                }
            }

            if (Audits[0].IncludeDataInEmail)
            {
                if (Audits[0].ResultDataSet.Tables.Count > 0)
                {
                    EmailTableTemplate currTemplate;
                    
                    // Check for template info at the collection level
                    if (!Audits.TemplateColorScheme.Equals(null))
                    {
                        currTemplate = Audits.TemplateColorScheme;
                    }
                    else
                    {
                        // Check for template info at the test level 
                        if (!Audits[0].Test.TemplateColorScheme.Equals(null))
                        {
                            currTemplate = Audits[0].Test.TemplateColorScheme;
                        }
                        else
                        {
                            // We didn't find anything, so get the default template
                            currTemplate = AuditUtils.GetDefaultTemplate();
                        }
                    }

                    string htmlData = AuditUtils.CreateHtmlData(Audits[0], Audits[0].ResultDataSet, currTemplate);

                    body.Append(htmlData);
                }
            }

            body.AppendLine(AuditUtils.HtmlBreak);

            if (Audits[0].Test.SendReport)
            {
                body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
                body.Append("<b>This report was run on: " + Audits[0].TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }
            else
            {
                body.Append("This audit ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
            }

            if (Audits[0].ShowQueryMessage)
            {
                body.Append(AuditUtils.HtmlBreak);
                body.Append("The '" + Audits[0].Name + "' audit has failed. The following SQL statement was used to test this audit :" + AuditUtils.HtmlBreak);
                body.Append(Audits[0].Test.SqlStatementToCheck.ToHtml() + AuditUtils.HtmlBreak);
                body.Append("<b>This query was run on: " + Audits[0].TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            string cleanBody = body.ToString().Replace("\r\n", string.Empty);

            return cleanBody;
        }
    }
}
