namespace NDataAudit.Framework
{
    /// <summary>
    /// Class AuditConnectionString.
    /// </summary>
    public class AuditConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConnectionString"/> class.
        /// </summary>
        public AuditConnectionString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AuditConnectionString(string connectionString)
        {
            // TODO: Change this once multi-database support is added
            string[] items = connectionString.Split(';');

            foreach (var item in items)
            {
                string[] currItem = item.Split('=');

                switch (currItem[0].ToLower())
                {
                    case "data source":
                        DatabaseServer = currItem[1];
                        break;
                    case "initial catalog":
                        DatabaseName = currItem[1];
                        break;
                    case "user id":
                        UserName = currItem[1];
                        break;
                    case "password":
                        Password = currItem[1];
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public string DatabaseServer { get; private set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; private set; }

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            // TODO: Change this once multi-database support is added
            return "Data Source=" + DatabaseServer + ";Initial Catalog=" + DatabaseName + ";User ID=" + UserName +
                   ";Password=" + Password + ";";
        }
    }
}
