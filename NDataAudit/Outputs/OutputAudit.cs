//*********************************************************************
// File:       		OutputAudit.cs
// Author:  	    Hector Sosa, Jr
// Date:			5/4/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		5/4/2018	    Created
//*********************************************************************

using System;

namespace NDataAudit.Framework.Outputs
{
    public class OutputAudit : AuditOutputBase
    {
        public OutputAudit(AuditCollection audits) : base(audits)
        {}

        public override string CreateOutputBody()
        {
            throw new NotImplementedException();
        }
    }
}
