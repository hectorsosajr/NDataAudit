using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace NDataAudit.Data.MySql
{
    /// <summary>
    /// Class AuditMySqlProvider.
    /// </summary>
    /// <seealso cref="NDataAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditMySqlProvider : IAuditDbProvider
    {
        private IDbConnection _currentDbConnection;
        private IDbCommand _currentDbCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditMySqlProvider"/> class.
        /// </summary>
        public AuditMySqlProvider()
        {}

        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseEngineName => "Oracle MySQL";

        /// <summary>
        /// Gets the provider namespace.
        /// </summary>
        /// <value>The provider namespace.</value>
        public string ProviderNamespace => "mysql.data.mysqlclient";

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
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection CreateDatabaseSession()
        {
            StringBuilder errorMessages = new StringBuilder();

            if (string.IsNullOrEmpty(ConnectionString))
            {
                return null;
            }

            MySqlConnection conn = new MySqlConnection(this.ConnectionString);

            try
            {
                conn.Open();

                _currentDbConnection = conn;
            }
            catch (MySqlException ex)
            {
                errorMessages.Append(ex.Message);

                Console.WriteLine(errorMessages.ToString());

                string fileName = "Logs\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_mysql.log";

                using (TextWriter writer = File.CreateText(fileName))
                {
                    writer.WriteLine(errorMessages.ToString());
                    writer.WriteLine(ex.StackTrace);
                }
            }

            _currentDbConnection = conn;

            return conn;
        }

        public IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand)
        {
            MySqlCommand cmd = (MySqlCommand)currentDbCommand;
            IDbDataAdapter retval = new MySqlDataAdapter(cmd);

            return retval;
        }

        public IDbCommand CreateDbCommand(string commandText, CommandType commandType, int commandTimeOut)
        {
            IDbCommand retval = new MySqlCommand(commandText)
            {
                Connection = (MySqlConnection) CurrentConnection,
                CommandTimeout = commandTimeOut
            };
            _currentDbCommand = retval;

            return retval;
        }
    }
}
