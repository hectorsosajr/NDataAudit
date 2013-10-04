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

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Audit()
        {
            EmailSubscribers = new ArrayList();
            Tests = new AuditTestCollection();

            ShowQueryMessage = true;
            ShowThresholdMessage = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The connection string needed to connect to the server that contains the needed test data.
        /// </summary>
        [Description("The connection string needed to connect to the server that contains the needed test data."), Category("Database")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// The emails of the people who will receive notifications.
        /// </summary>
        [Description("The emails of the people who will receive notifications.")]
        public ArrayList EmailSubscribers { get; set; }

        /// <summary>
        /// Whether or not this audit has been tested in this run.
        /// </summary>
        [Description("Whether or not this audit has been tested in this run.")]
        public bool HasRun { get; set; }

        /// <summary>
        /// This is the name of this <see cref="Audit"/>.
        /// </summary>
        [Description("This is the name of this Audit.")]
        public string Name { get; set; }

        /// <summary>
        /// The result of this <see cref="Audit"/> tests is stored here.
        /// </summary>
        [Description("The result of this Audit tests is stored here.")]
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
        [Description("The tests that will be run against the SQLStatement property.")]
        public AuditTestCollection Tests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the result data into the email.
        /// </summary>
        /// <value>
        ///   <c>true</c> if include the result data into the email; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether to include the result data into the email.")]
        public bool IncludeDataInEmail { get; set; }

        /// <summary>
        /// Gets or sets the test server where the test is being run.
        /// </summary>
        /// <value>
        /// The test server.
        /// </value>
        [Description("Gets or sets the test server where the test is being run.")]
        public string TestServer { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        /// <value>
        /// The email subject for this audit test.
        /// </value>
        [Description("Gets or sets the email subject for this test.")]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show threshold message].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show threshold message]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowThresholdMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show query message].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show query message]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowQueryMessage { get; set; }

        public bool FailIfTrue { get; set; }

        #endregion
    }
}
