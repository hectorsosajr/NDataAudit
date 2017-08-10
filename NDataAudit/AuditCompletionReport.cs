using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDataAudit.Framework
{
    /// <summary>
    /// AuditCompletionReport is a class to generate a pass-fail report for each audit in an audit group.
    /// </summary>
    public class AuditCompletionReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditCompletionReport"/> class.
        /// </summary>
        public AuditCompletionReport()
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
    }
}
