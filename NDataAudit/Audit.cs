//*********************************************************************
// File:       		Audit.cs
// Author:  	    Hector Sosa, Jr
// Date:			2/16/2005
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE		COMMENTS
// Hector Sosa, Jr		2/16/2005	Created
// Hector Sosa, Jr      2/18/2005   Added properties
// Hector Sosa, Jr		3/21/2005	Changed the private variables
//									to be compliant with C# naming
//									conventions.
//*********************************************************************/

using System.Collections;
using System.ComponentModel;

namespace NDataAudit.Framework
{
    /// <summary>
    /// Summary description for Audit.
    /// </summary>
    public class Audit
    {
        #region Declarations

        /// <summary>
        ///  SQL type of command, either a SQL string or a stored procedure.
        /// </summary>
        public enum SqlStatementTypeEnum
        {
            /// <summary>
            /// A regular SQL Statement
            /// </summary>
            SqlText = 0,

            /// <summary>
            /// A stored procedure
            /// </summary>
            /// <remarks>Not implemented</remarks>
            StoredProcedure = 1
        }

        /// <summary>
        /// Type of email entry
        /// </summary>
        public enum EmailTypeEnum
        {
            /// <summary>
            /// Regular email recipient
            /// </summary>
            Recipient,

            /// <summary>
            /// Carbon Copy email recipient
            /// </summary>
            CarbonCopy,

            /// <summary>
            /// Blind carbon copy email recipient
            /// </summary>
            BlindCarbonCopy
        }

        /// <summary>
        /// Type of email priority
        /// </summary>
        public enum EmailPriorityEnum
        {
            /// <summary>
            /// The low email priority
            /// </summary>
            Low = 0,

            /// <summary>
            /// The normal email priority
            /// </summary>
            Normal = 1,

            /// <summary>
            /// The high email priority
            /// </summary>
            High = 2
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Audit()
        {
            EmailSubscribers = new ArrayList();
            EmailCarbonCopySubscribers = new ArrayList();
            EmailBlindCarbonCopySubscribers = new ArrayList();
            Tests = new AuditTestCollection();

            ShowQueryMessage = true;
            ShowThresholdMessage = true;
            ShowCommentMessage = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The connection string needed to connect to the server that contains the needed test data.
        /// </summary>
        [Description("The connection string needed to connect to the server that contains the needed test data."), Category("Database")]
        public AuditConnectionString ConnectionString { get; set; }

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
        /// Whether or not this audit has been tested in this run.
        /// </summary>
        [Description("Whether or not this audit has been tested in this run."), Category("Audit")]
        public bool HasRun { get; set; }

        /// <summary>
        /// This is the name of this <see cref="Audit"/>.
        /// </summary>
        [Description("This is the name of this Audit."), Category("Audit")]
        public string Name { get; set; }

        /// <summary>
        /// The result of this <see cref="Audit"/> tests is stored here.
        /// </summary>
        [Description("The result of this Audit tests is stored here."), Category("Audit")]
        public bool Result { get; set; }

        /// <summary>
        /// The SQL statement that this <see cref="Audit"/> will run.
        /// </summary>
        [Description("The SQL statement that this Audit will run."), Category("Database")]
        public string SqlStatement { get; set; }

        /// <summary>
        /// The Order By clause used in conjunction with the <see cref="SqlStatement"/> property.
        /// </summary>
        [Description("The Order By clause used in conjunction with the SQLStatement property."), Category("Database")]
        public string OrderByClause { get; set; }

        /// <summary>
        /// The type needed to be passed to the ADO.NET Command object.
        /// </summary>
        [Description("The type needed to be passed to the ADO.NET Command object."), Category("Database")]
        public SqlStatementTypeEnum SqlType { get; set; }

        /// <summary>
        /// The tests that will be run against the <see cref="SqlStatement"/> property.
        /// </summary>
        [Description("The tests that will be run against the SQLStatement property."), Category("Tests")]
        public AuditTestCollection Tests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the result data into the email.
        /// </summary>
        /// <value>
        ///   <c>true</c> if include the result data into the email; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether to include the result data into the email."), Category("Email")]
        public bool IncludeDataInEmail { get; set; }

        /// <summary>
        /// Gets or sets the test server where the test is being run.
        /// </summary>
        /// <value>
        /// The test server.
        /// </value>
        [Description("Gets or sets the test server where the test is being run."), Category("Report")]
        public string TestServer { get; set; }

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
        [Description("Gets or sets the email prioirty for this audit."), Category("Email")]
        public EmailPriorityEnum EmailPriority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show threshold message].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show threshold message]; otherwise, <c>false</c>.
        /// </value>
        [Description("A flag to determine whether to show the threshold message or not."), Category("Report")]
        public bool ShowThresholdMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [show query message].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show query message]; otherwise, <c>false</c>.
        /// </value>
        [Description("A flag to determine whether to show the query or not."), Category("Report")]
        public bool ShowQueryMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the comment and instructions message.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showComments]; otherwise, <c>false</c>.
        /// </value>
        [Description("A flag to determine whether to show the comments/instructions or not."), Category("Report")]
        public bool ShowCommentMessage { get; set; }


        /// <summary>
        /// Gets or sets whether to fail the audit if the threshold condition is true.
        /// </summary>
        /// <value><c>true</c> if [fail if true]; otherwise, <c>false</c>.</value>
        [Description("Gets or sets whether to fail the audit if the threshold condition is true."), Category("Tests")]
        public bool FailIfTrue { get; set; }

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

        #endregion
    }
}
