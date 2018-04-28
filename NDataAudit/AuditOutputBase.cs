//*********************************************************************
// File:       		AuditOutputBase.cs
// Author:  	    Hector Sosa, Jr
// Date:			4/26/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		4/26/2018	    Created
//*********************************************************************

namespace NDataAudit.Framework
{
    /// <summary>
    /// This is a base class for the different output types that NDataAudit can do.
    /// </summary>
    public abstract class AuditOutputBase
    {
        private readonly AuditCollection _audits;

        /// <summary>Initializes a new instance of the AuditOutputBase class.</summary>
        /// <param name="audits">The audits objects that will be used for the output.</param>
        protected AuditOutputBase(AuditCollection audits)
        {
            _audits = audits;
        }

        /// <summary>
        /// Gets the list of Audits.
        /// </summary>
        /// <value>A list of Audits from a AuditCollection object.</value>
        public AuditCollection Audits => _audits;

        /// <summary>Creates the output body.</summary>
        /// <returns>System.String.</returns>
        public abstract string CreateOutputBody();
    }
}
