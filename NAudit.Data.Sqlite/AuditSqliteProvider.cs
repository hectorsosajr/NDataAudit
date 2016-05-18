using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace NAudit.Data.Sqlite
{
    /// <summary>
    /// Class AuditSqliteProvider.
    /// </summary>
    /// <seealso cref="NAudit.Data.IAuditDbProvider" />
    [Export(typeof(IAuditDbProvider))]
    public class AuditSqliteProvider : IAuditDbProvider
    {
        private IDbConnection _currentDbConnection;
        private IDbCommand _currentDbCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditSqliteProvider"/> class.
        /// </summary>
        public AuditSqliteProvider()
        { }

        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseEngineName => "SQLite";

        /// <summary>
        /// Gets the provider namespace.
        /// </summary>
        /// <value>The provider namespace.</value>
        public string ProviderNamespace => "system.data.sqlite";

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
            IDbCommand retval = new SQLiteCommand(commandText);
            retval.Connection = CurrentConnection;
            retval.CommandTimeout = commandTimeOut;

            _currentDbCommand = retval;

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

            SQLiteConnection conn = new SQLiteConnection(this.ConnectionString);

            try
            {
                conn.Open();

                _currentDbConnection = conn;
            }
            catch (SQLiteException ex)
            {
                //for (int i = 0; i < ex.Errors.Count; i++)
                //{
                //    errorMessages.Append("Index #" + i + "\n" +
                //                         "Message: " + ex.Errors[i].Message + "\n" +
                //                         "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                //                         "Source: " + ex.Errors[i].Source + "\n" +
                //                         "Procedure: " + ex.Errors[i].Procedure + "\n");
                //}

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

        /// <summary>
        /// Creates the database data adapter for the specific database engine.
        /// </summary>
        /// <param name="currentDbCommand">The current database command.</param>
        /// <returns>IDbDataAdapter.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand)
        {
            SQLiteCommand cmd = (SQLiteCommand)currentDbCommand;
            IDbDataAdapter retval = new SQLiteDataAdapter(cmd);

            return retval;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public List<string> Errors { get; set; }
    }
}
