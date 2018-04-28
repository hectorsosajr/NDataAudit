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
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
// Hector Sosa, Jr      4/28/2018   Audits now store the result of the
//                                  dataset in the new ResultDataSet
//                                  property.
//*********************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

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
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        public Audit()
        {
            Test = new AuditTest();
            ErrorMessages = new List<string>();

            ShowQueryMessage = true;
            ShowThresholdMessage = true;
            ShowCommentMessage = true;
        }

        #endregion

        #region Properties

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
        /// The Order By clause used in conjunction with the SqlStatement property.
        /// </summary>
        [Description("The Order By clause used in conjunction with the SQLStatement property."), Category("Database")]
        public string OrderByClause { get; set; }

        /// <summary>
        /// The test that will be run in this audit.
        /// </summary>
        [Description("The test that will be run in this Audit. See the SQLStatement property."), Category("Test")]
        public AuditTest Test { get; set; }

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
        [Description("Gets or sets the email priority for this audit."), Category("Email")]
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
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>The error messages.</value>
        [Description("Gets or sets the error messages from exceptions happening inside the auditing engine."), Category("Audit")]
        public List<string> ErrorMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [was successful].
        /// </summary>
        /// <value><c>true</c> if [was successful]; otherwise, <c>false</c>.</value>
        [Description("Gets or sets whether the audit passed or failed."), Category("Audit")]
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the result data set.
        /// </summary>
        /// <value>The result data set.</value>
        [Description("Gets or sets the data set that contains the result of the <see cref=\"Audit\"/> query."), Category("Data")]
        public DataSet ResultDataSet { get; set; }

        #endregion
    }
}