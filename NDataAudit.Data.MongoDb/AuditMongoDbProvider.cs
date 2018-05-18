//*********************************************************************
// File:       		AuditMongoDbProvider.cs
// Author:  	    Hector Sosa, Jr
// Date:			5/17/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		5/17/2018	    Created
//*********************************************************************

using System.Collections.Generic;

namespace NDataAudit.Data.Mongo
{
    /// <summary>
    /// Class AuditMongoProvider.
    /// </summary>
    /// <seealso cref="NDataAudit.Data.IAuditNoSqlProvider" />
    public class AuditMongoDbProvider : IAuditNoSqlProvider
    {
        public string ConnectionString { get; set; }

        public string DatabaseEngineName { get; }

        public string ProviderNamespace { get; }

        public List<string> Errors { get; set; }
    }
}
