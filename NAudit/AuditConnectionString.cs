namespace NAudit.Framework
{
    /// <summary>
    /// Class AuditConnectionString.
    /// </summary>
    public class AuditConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConnectionString" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseProviderName">Name of the database provider.</param>
        public AuditConnectionString(string connectionString, string databaseProviderName)
        {
            string[] items = connectionString.Split(';');

            DatabaseProviderName = databaseProviderName;

            foreach (var item in items)
            {
                string[] currItem = item.Split('=');

                switch (currItem[0].ToLower())
                {
                    case "data source":
                    case "server":
                    case "host":
                        DatabaseServer = currItem[1];
                        break;
                    case "initial catalog":
                    case "database":
                    case "schema":
                        DatabaseName = currItem[1];
                        break;
                    case "user id":
                        UserName = currItem[1];
                        break;
                    case "password":
                        Password = currItem[1];
                        break;
                    case "port":
                        Port = currItem[1];
                        break;
                    case "defaulttable":
                        DatabaseTargetTable = currItem[1];
                        break;
                    case "driver":
                        DatabaseDriver = currItem[1];
                        break;
                    default:
                        if (!string.IsNullOrEmpty(currItem[0]))
                        {
                            ExtraSettings += currItem[0] + "=" + currItem[1] + ";";
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public string CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public string ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets the database driver.
        /// </summary>
        /// <value>The database driver.</value>
        public string DatabaseDriver { get; private set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public string DatabaseServer { get; private set; }

        /// <summary>
        /// Gets the database provider.
        /// </summary>
        /// <value>The database provider.</value>
        public string DatabaseProviderName { get; private set; }

        /// <summary>
        /// Gets the user identifier for this connection string, if any.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the password for this connection string, if any.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the port where the server will be listening on.
        /// </summary>
        /// <value>The port the database engine listens on.</value>
        public string Port { get; private set; }

        /// <summary>
        /// Gets the target table. Some database engines require that a 
        /// table be part of the connection string.
        /// </summary>
        /// <value>The target table.</value>
        public string DatabaseTargetTable { get; private set; }

        /// <summary>
        /// Gets the extra settings.
        /// </summary>
        /// <value>The extra settings.</value>
        public string ExtraSettings { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string retval = string.Empty;

            switch (DatabaseProviderName)
            {
                case "system.data.sqlclient":
                    retval = "Data Source=" + DatabaseServer + ";Initial Catalog=" + DatabaseName + ";User ID=" + UserName +
                   ";Password=" + Password + ";";
                    break;
                case "npgsql":
                    retval = "Server=" + DatabaseServer + ";Database=" + DatabaseName + ";User ID=" + UserName +
                             ";Password=" + Password;

                    if (!string.IsNullOrEmpty(Port))
                    {
                        retval += ";Port=" + Port;
                    }
                    break;
                case "hadoop.hive":
                    retval = "DRIVER=" + DatabaseDriver + ";Host=" + DatabaseServer + ";Port=" + Port + ";Schema=" +
                             DatabaseName + ";DefaultTable=" + DatabaseTargetTable + ";" + ExtraSettings;
                    break;
            }

            return retval;
        }
    }
}
