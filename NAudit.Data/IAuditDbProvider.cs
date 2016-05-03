﻿using System.Data;

namespace NAudit.Data
{
    /// <summary>
    /// Interface IAuditDbProvider
    /// </summary>
    public interface IAuditDbProvider
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
        string DatabaseName { get; }

        /// <summary>
        /// Gets the provider namespace.
        /// </summary>
        /// <value>The provider namespace.</value>
        string ProviderNamespace { get; }

        /// <summary>
        /// Gets the current connection, is it has been set.
        /// </summary>
        /// <value>The current connection.</value>
        IDbConnection CurrentConnection { get; }

        /// <summary>
        /// Creates the command object for the specific database engine.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command, stored procedure or SQL text.</param>
        /// <param name="commandTimeOut">The command time out.</param>
        /// <returns>IDbCommand.</returns>
        IDbCommand CreateDbCommand(string commandText, CommandType commandType, int commandTimeOut);

        /// <summary>
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        IDbConnection CreateDatabaseSession();

        /// <summary>
        /// Creates the database data adapter for the specific database engine.
        /// </summary>
        /// <param name="currentDbCommand">The current database command.</param>
        /// <returns>IDbDataAdapter.</returns>
        IDbDataAdapter CreateDbDataAdapter(IDbCommand currentDbCommand);
    }
}