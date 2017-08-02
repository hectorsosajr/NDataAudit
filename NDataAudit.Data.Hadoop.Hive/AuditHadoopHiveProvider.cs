using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace NDataAudit.Data.Hadoop.Hive
{
    /// <summary>
    /// Class AuditHadoopHiveProvider.
    /// </summary>
    /// <seealso cref="NAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditHadoopHiveProvider : IAuditDbProvider
    {
        private IDbConnection _currentDbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHadoopHiveProvider" /> class.
        /// </summary>
        public AuditHadoopHiveProvider()
        {}

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseEngineName => "Hadoop Hive";

        /// <summary>
        /// Gets the provider namespace.
        /// </summary>
        /// <value>The provider namespace.</value>
        public string ProviderNamespace => "hadoop.hive";

        /// <summary>
        /// Gets the current connection, is it has been set.
        /// </summary>
        /// <value>The current connection.</value>
        public IDbConnection CurrentConnection => _currentDbConnection;

        /// <summary>
        /// Gets the current command.
        /// </summary>
        /// <value>The current command.</value>
        public IDbCommand CurrentCommand { get; }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the database connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public string ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the database command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public string CommandTimeout { get; set; }

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
            IDbCommand retval = new OdbcCommand(commandText)
            {
                Connection = (OdbcConnection) CurrentConnection,
                CommandTimeout = commandTimeOut
            };
            return retval;
        }

        /// <summary>
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbConnection CreateDatabaseSession()
        {
            StringBuilder errorMessages = new StringBuilder();

            if (string.IsNullOrEmpty(ConnectionString))
            {
                return null;
            }

            OdbcConnection conn = new OdbcConnection(this.ConnectionString)
            {
                ConnectionTimeout = int.Parse(ConnectionTimeout)
            };


            try
            {
                conn.Open();

                _currentDbConnection = conn;
            }
            catch (OdbcException ex)
            {
                errorMessages.Append(ex.Message);

                Errors.Add(ex.Message);

                Console.WriteLine(errorMessages.ToString());

                string fileName = "Logs\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_hive.log";

                string logPath = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                using (TextWriter writer = File.CreateText(fileName))
                {
                    writer.WriteLine(errorMessages.ToString());
                    writer.WriteLine(ex.StackTrace);
                }
            }

            return conn;
        }

        /// <summary>
        /// Creates the database data adapter for the specific database engine.
        /// </summary>
        /// <param name="currentDbCommand">The current database command.</param>
        /// <returns>IDbDataAdapter.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand)
        {
            OdbcCommand cmd = (OdbcCommand)currentDbCommand;
            IDbDataAdapter retval = new OdbcDataAdapter(cmd);

            return retval;
        }
    }
}
