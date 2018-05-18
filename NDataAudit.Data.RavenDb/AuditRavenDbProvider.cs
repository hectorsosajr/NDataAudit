﻿//*********************************************************************
// File:       		AuditRavenDbProvider.cs
// Author:  	    Hector Sosa, Jr
// Date:			5/17/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		5/17/2018	    Created
//*********************************************************************

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Raven.Client;

namespace NDataAudit.Data.RavenDb
{
    /// <summary>
    /// Class AuditRavenDbProvider.
    /// </summary>
    /// <seealso cref="NDataAudit.Data.IAuditNoSqlProvider" />
    [Export(typeof(IAuditNoSqlProvider))]
    public class AuditRavenDbProvider : IAuditNoSqlProvider
    {
        public string ConnectionString { get; set; }

        public string DatabaseEngineName => "Hybernating Rhinos RavenDb";

        public string ProviderNamespace { get; }

        public List<string> Errors { get; set; }
    }
}
