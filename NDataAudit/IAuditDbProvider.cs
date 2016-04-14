using System.Data;

namespace NDataAudit.Framework
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
        /// Creates the database session.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        IDbConnection CreateDatabaseSession();
    }
}
