using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace NAudit.Data.SqlServer
{
    /// <summary>
    /// Class AuditSqlServerProvider.
    /// </summary>
    /// <seealso cref="NAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditSqlServerProvider : IAuditDbProvider
    {
        private IDbConnection _currentDbConnection;
        private IDbCommand _currentDbCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditSqlServerProvider"/> class.
        /// </summary>
        public AuditSqlServerProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditSqlServerProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AuditSqlServerProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName => "Microsoft SQL Server";

        /// <summary>
        /// Gets the database provider namespace.
        /// </summary>
        /// <value>The database provider namespace.</value>
        public string ProviderNamespace => "system.data.sqlclient";

        /// <summary>
        /// Gets the current connection, if it has been set.
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
        public IDbCommand CreateDbCommand(string commandText, CommandType commandType, int commandTimeOut)
        {
            IDbCommand retval = new SqlCommand(commandText);
            retval.Connection = CurrentConnection;
            retval.CommandTimeout = commandTimeOut;

            return retval;
        }

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

            SqlConnection conn = new SqlConnection(this.ConnectionString);

            try
            {
                conn.Open();

                _currentDbConnection = conn;
            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                                         "Message: " + ex.Errors[i].Message + "\n" +
                                         "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                         "Source: " + ex.Errors[i].Source + "\n" +
                                         "Procedure: " + ex.Errors[i].Procedure + "\n");
                }

                Console.WriteLine(errorMessages.ToString());

                string fileName = "Logs\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

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
        public IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand)
        {
            SqlCommand cmd = (SqlCommand) currentDbCommand;
            IDbDataAdapter retval = new SqlDataAdapter(cmd);
            
            return retval;
        }
    }
}
