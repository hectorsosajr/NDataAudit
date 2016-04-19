

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
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

            SqlConnection conn = new SqlConnection(this.ConnectionString);

            try
            {
                conn.Open();
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
    }
}
