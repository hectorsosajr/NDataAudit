

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NDataAudit.Data;

namespace NAudit.Data.SqlServer
{
    /// <summary>
    /// Class AuditSqlServerProvider.
    /// </summary>
    /// <seealso cref="NDataAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditSqlServerProvider : IAuditDbProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditSqlServerProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AuditSqlServerProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName
        {
            get
            {
                return "Microsoft SQL Server";
            }
        }

        /// <summary>
        /// Gets the database provider namespace.
        /// </summary>
        /// <value>The database provider namespace.</value>
        public string ProviderNamespace
        {
            get
            {
                return "system.data.sqlclient";
            }
        }

        /// <summary>
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbConnection CreateDatabaseSession()
        {
            var factory = DbProviderFactories.GetFactory(ProviderNamespace);
            var connection = factory.CreateConnection();

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
            }

            return connection;
        }
    }
}
