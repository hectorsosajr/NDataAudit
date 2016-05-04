using System;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace NAudit.Data.MySql
{
    /// <summary>
    /// Class AuditMySqlProvider.
    /// </summary>
    /// <seealso cref="NAudit.Data.IAuditDbProvider" />
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditMySqlProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AuditMySqlProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

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
                //for (int i = 0; i < ex.Errors.Count; i++)
                //{
                //    errorMessages.Append("Index #" + i + "\n" +
                //                         "Message: " + ex.Errors[i].Message + "\n" +
                //                         "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                //                         "Source: " + ex.Errors[i].Source + "\n" +
                //                         "Procedure: " + ex.Errors[i].Procedure + "\n");
                //}

                errorMessages.Append(ex.Message);

                Console.WriteLine(errorMessages.ToString());

                string fileName = "Logs\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

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
            IDbCommand retval = new MySqlCommand(commandText);
            retval.Connection = CurrentConnection;
            retval.CommandTimeout = commandTimeOut;

            _currentDbCommand = retval;

            return retval;
        }
    }
}
