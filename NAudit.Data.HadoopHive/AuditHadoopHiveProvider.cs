using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Odbc;
using System.Data.Common;

namespace NAudit.Data.HadoopHive
{
    /// <summary>
    /// Class AuditHadoopHiveProvider.
    /// </summary>
    /// <seealso cref="NAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditHadoopHiveProvider : IAuditDbProvider
    {
        private IDbConnection _currentDbConnection;
        private IDbCommand _currentDbCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHadoopHiveProvider"/> class.
        /// </summary>
        public AuditHadoopHiveProvider()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHadoopHiveProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AuditHadoopHiveProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public string DatabaseEngineName => "Hadoop Hive";

        public string ProviderNamespace => "hive";

        /// <summary>
        /// Gets the current connection, is it has been set.
        /// </summary>
        /// <value>The current connection.</value>
        public IDbConnection CurrentConnection => _currentDbConnection;

        /// <summary>
        /// Gets the current command.
        /// </summary>
        /// <value>The current command.</value>
        public IDbCommand CurrentCommand => _currentDbCommand;

        /// <summary>
        /// Creates the command object for the specific database engine.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command, stored procedure or SQL text.</param>
        /// <param name="commandTimeOut">The command time out.</param>
        /// <returns>IDbCommand.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbCommand CreateDbCommand(string commandText, CommandType commandType, int commandTimeOut)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbConnection CreateDatabaseSession()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the database data adapter for the specific database engine.
        /// </summary>
        /// <param name="currentDbCommand">The current database command.</param>
        /// <returns>IDbDataAdapter.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand)
        {
            throw new NotImplementedException();
        }
    }
}
