﻿//*********************************************************************
// File:       		OutputAlert.cs
// Author:  	    Hector Sosa, Jr
// Date:			4/26/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		4/26/2018	    Created
// Hector Sosa, Jr      6/5/2018        Changed the HTML for this output class.
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
            string rightNow = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);

            string cleanBody = string.Empty;

            foreach (Audit audit in base.Audits)
            {
                body.Append("<table border=\"1\" width=\"100%\" style=\"border-collapse: collapse;font-family: verdana; font-size: 10pt\">");
                body.AppendFormat("<tr><td><p><b><font size=\"4\">{0} :: <font color=\"#FF0000\">COMPLETED W/ ERRORS</font></font></b></td></tr>",audit.Name);
                body.Append("<tr><td width=\"100%\" bgcolor=\"#0B6BE1\"><font color=\"#FFFFFF\"><b>Alert Summary</b></font></td></tr>");
                body.Append("<tr><td width=\"100%\">");
                body.Append("<table border=\"1\" width=\"100%\" style=\"border-collapse: collapse;font-family: verdana; font-size: 10pt\">");
                body.Append("<tr><td width=\"189\" bgcolor=\"#F2F2F2\"><b>Execution Status</b></td><td> <font color=\"#FF0000\">COMPLETED W/ ERRORS</font></td></tr>");
                body.AppendFormat("<tr><td width=\"189\" bgcolor=\"#F2F2F2\"><b>Ran at </b></td><td>{0}</td></tr>",rightNow);
                body.AppendFormat("<tr><td width=\"189\" bgcolor=\"#F2F2F2\"><b>Machine Name</b></td><td>{0}</td></tr>",audit.TestServer);
                body.Append("</table></td></tr></table>");
                body.Append(AuditUtils.HtmlBreak);
                body.Append("<table border=\"1\" width=\"100%\" style=\"border-collapse: collapse;font-family: verdana; font-size: 10pt\">");
                body.Append("<tr><td width=\"100%\" bgcolor=\"#FF0000\"><font color=\"#FFFFFF\" size=\"4\"><b>ERRORS</b></font></td></tr>");
                body.AppendFormat("<tr><td width=\"100%\" bgcolor=\"#FFD5D5\"><b>Source :</b> {0}</td></tr>",audit.Name);
                body.AppendFormat("<tr><td width=\"100%\"><b>{0} : </b>{1}</td></tr>",rightNow,audit.Test.FailedMessage);
                body.Append("</table>");
                body.Append(AuditUtils.HtmlBreak);
                body.Append("<table border=\"1\" width=\"100%\" style=\"border-collapse: collapse;font-family: verdana; font-size: 10pt\">");
                body.Append("<tr><td width=\"100%\" bgcolor=\"#4a781c\"><font color=\"#FFFFFF\" size=\"4\"><b>QUERY</b></font></td></tr>");
                body.Append("<tr><td width=\"100%\" bgcolor=\"#8dfb9c\"><b>Tested With :</b> </td></tr>");
                body.AppendFormat("<tr><td width=\"100%\">{0}</td></tr>", audit.Test.SqlStatementToCheck.ToHtml());
                body.Append("</table>");
                body.Append(AuditUtils.HtmlBreak);

                if (audit.Test.Instructions != null)
                {
                    if (audit.Test.Instructions.Length > 0)
                    {
                        body.Append("<table border=\"1\" width=\"100%\" style=\"border-collapse: collapse;font-family: verdana; font-size: 10pt\">");
                        body.Append("<tr><td width=\"100%\" bgcolor=\"#6600FF\"><font color=\"#FFFFFF\" size=\"4\"><b>COMMENTS AND INSTRUCTIONS</b></font></td></tr>");
                        body.Append("<tr><td width=\"100%\" bgcolor=\"#CC99FF\"><b>====</b> </td></tr>");
                        body.AppendFormat("<tr><td width=\"100%\">{0}</td></tr>",audit.Test.Instructions.ToHtml());
                        body.Append("</table>");
                        body.Append(AuditUtils.HtmlBreak);
                    }
                }

                body.Append("<font face=\"Tahoma\" size=\"1\">This alert was generated by <a href=\"https://github.com/hectorsosajr/NDataAudit\">NDataAudit Framework</a></font>");
                body.Append(AuditUtils.HtmlBreak);
                body.Append(AuditUtils.HtmlBreak);

                cleanBody = body.ToString().Replace("\r\n", string.Empty);
            }

            return cleanBody;
        }
    }
}
