//*********************************************************************
// File:       		IAuditNoSqlProvider.cs
// Author:  	    Hector Sosa, Jr
// Date:			5/17/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		5/17/2018	    Created
//*********************************************************************

using System.Collections.Generic;

namespace NDataAudit.Data
{
    /// <summary>
    /// Interface IAuditNoSqlProvider
    /// </summary>
    public interface IAuditNoSqlProvider
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseEngineName { get; }

        /// <summary>
        /// Gets the provider namespace.
        /// </summary>
        /// <value>The provider namespace.</value>
        string ProviderNamespace { get; }

        /// <summary>
        /// Gets the errors from any internally thrown exceptions.
        /// </summary>
        /// <value>The errors.</value>
        List<string> Errors { get; set; }
    }
}
