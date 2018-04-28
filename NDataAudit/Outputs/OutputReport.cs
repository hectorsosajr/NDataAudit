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

namespace NDataAudit.Framework.Outputs
{
    /// <summary>
    /// Class OutputReport.
    /// </summary>
    /// <seealso cref="NDataAudit.Framework.AuditOutputBase" />
    public class OutputReport : AuditOutputBase
    {
        /// <summary>
        /// Initializes a new instance of the AuditOutputBase class.
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
            throw new NotImplementedException();
        }
    }
}
