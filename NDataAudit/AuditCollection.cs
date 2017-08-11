//*********************************************************************
// File:       		AuditCollection.cs
// Author:  	    Hector Sosa, Jr
// Date:			2/16/2005
//**************************************************************************************
// Change Log
//**************************************************************************************
// USER					DATE        COMMENTS
// Hector Sosa, Jr		2/16/2005	Created
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
//**************************************************************************************

using System.Collections;
using System.ComponentModel;

namespace NDataAudit.Framework
{
	/// <summary>
	/// Summary description for AuditCollection.
	/// </summary>
	public class AuditCollection : CollectionBase
	{
		#region Constructors

		/// <summary>
		/// Empty constructor
		/// </summary>
		public AuditCollection()
		{
            EmailSubscribers = new ArrayList();
            EmailCarbonCopySubscribers = new ArrayList();
            EmailBlindCarbonCopySubscribers = new ArrayList();
        }

        #endregion

        #region  Properties 

        /// <summary>
        /// Gets the <see cref="Audit"/> with the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Audit.</returns>
        public Audit this[int index] => ((Audit)(List[index]));

        /// <summary>
        /// Gets or sets the name of the audit group.
        /// </summary>
        /// <value>The name of the audit group.</value>
        public string AuditGroupName { get; set; }

	    /// <summary>
	    /// The connection string needed to connect to the server that contains the needed test data.
	    /// </summary>
	    [Description("The connection string needed to connect to the server that contains the needed test data."), Category("Database")]
	    public AuditConnectionString ConnectionString { get; set; }

	    /// <summary>
	    /// Gets or sets the database provider.
	    /// </summary>
	    /// <value>The database provider.</value>
	    public string DatabaseProvider { get; set; }

	    /// <summary>
	    /// The emails of the people who will receive notifications.
	    /// </summary>
	    [Description("The emails of the people who will receive notifications."), Category("Email")]
	    public ArrayList EmailSubscribers { get; set; }

	    /// <summary>
	    /// The emails of the people who will receive notifications as carbon copies.
	    /// </summary>
	    [Description("The emails of the people who will receive notifications as carbon copies."), Category("Email")]
	    public ArrayList EmailCarbonCopySubscribers { get; set; }

	    /// <summary>
	    /// The emails of the people who will receive notifications as blind carbon copies.
	    /// </summary>
	    [Description("The emails of the people who will receive notifications as blind carbon copies."), Category("Email")]
	    public ArrayList EmailBlindCarbonCopySubscribers { get; set; }

	    /// <summary>
	    /// The address for the SMTP server.
	    /// </summary>
	    [Description("Gets or sets the address for the SMTP Server."), Category("Email Server")]
	    public string SmtpServerAddress { get; set; }

	    /// <summary>
	    /// The port number for the SMTP server, if it uses something other than 25. This is mostly for TLS and SSL connections.
	    /// </summary>
	    [Description("Gets or sets the port for the SMTP Server."), Category("Email Server")]
	    public int SmtpPort { get; set; }

	    /// <summary>
	    /// This is the SMTP user name for servers that require authentication.
	    /// </summary>
	    [Description("Gets or sets the user name for the SMTP Server."), Category("Email Server")]
	    public string SmtpUserName { get; set; }

	    /// <summary>
	    /// This is the SMTP password for servers that require authentication.
	    /// </summary>
	    [Description("Gets or sets the password for the SMTP Server."), Category("Email Server")]
	    public string SmtpPassword { get; set; }

	    /// <summary>
	    /// A flag to indicate whether or not the SMTP server needs to connect using SSL or TSL.
	    /// </summary>
	    [Description("A flag to indicate whether or not the SMTP Server uses SSL or TSL."), Category("Email Server")]
	    public bool SmtpUseSsl { get; set; }

	    /// <summary>
	    /// A flag to indicate whether this audit group needs to connect to a SMTP server that requires network credentials.
	    /// </summary>
	    [Description("A flag to indicate whether or not the SMTP Server needs credentials."), Category("Email Server")]
	    public bool SmtpHasCredentials { get; set; }

	    /// <summary>
	    /// This is the SMTP FROM email address.
	    /// </summary>
	    [Description("Gets or sets the FROM email used for the SMTP Server."), Category("Email Server")]
	    public string SmtpSourceEmail { get; set; }

	    /// <summary>
	    /// Gets or sets the email subject.
	    /// </summary>
	    /// <value>
	    /// The email subject for this audit test.
	    /// </value>
	    [Description("Gets or sets the email subject for this audit."), Category("Email")]
	    public string EmailSubject { get; set; }

	    /// <summary>
	    /// Gets or sets the email priority.
	    /// </summary>
	    /// <value>The email priority.</value>
	    [Description("Gets or sets the email priority for this audit."), Category("Email")]
	    public EmailPriorityEnum EmailPriority { get; set; }

        #endregion

        #region  Public Members 

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Int32[].</returns>
        public int[] Add(AuditCollection items)
		{
			ArrayList indexes = new ArrayList();

			foreach (object item in items)
			{
				indexes.Add(this.List.Add(item));
			}

			return ((int[])(indexes.ToArray(typeof(int))));
		}

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int Add(Audit item)
		{
			return List.Add(item);
		}

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, Audit item)
		{
			List.Insert(index, item);
		}

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(Audit item)
		{
			List.Remove(item);
		}

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(Audit item)
		{
			return List.Contains(item);
		}

		#endregion
	}
}
