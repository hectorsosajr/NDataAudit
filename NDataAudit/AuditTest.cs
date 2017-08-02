//*********************************************************************
// File:       		AuditTest.cs
// Author:  	    Hector Sosa, Jr
// Date:			3/1/2005
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		3/1/2005	    Created
// Hector Sosa, Jr		3/21/2005		Changed the private variables
//										to be compliant with C# naming
//										conventions.
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
//*********************************************************************

using System.Collections;

namespace NDataAudit.Framework
{
    /// <summary>
    /// Summary description for AuditTest.
    /// </summary>
    public class AuditTest
    {
        #region  Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public AuditTest()
        {
            SendReport = false;
            HasTestRun = false;
            TableNames = new ArrayList();
        }

        #endregion

        #region  Properties

        /// <summary>
        /// The name of the table column to test.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Text to use in the where clause that will be built from this test.
        /// </summary>
        /// <remarks>
        /// Check the <see cref="SqlStatementToCheck"/> property to see the full query used to validate this test.
        /// </remarks>
        public string Criteria { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [fail if condition is true].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [fail if condition is true]; otherwise, <c>false</c>.
        /// </value>
        public bool FailIfConditionIsTrue { get; set; }

        /// <summary>
        /// Gets or sets the instructions for the people receiving the email.
        /// </summary>
        /// <value>
        /// The instructions on what is actually happening. It may include how to fix the issue.
        /// </value>
        public string Instructions { get; set; }

        /// <summary>
        /// The operator symbol that will be used with the <see cref="Criteria"/> property.
        /// </summary>
        /// <remarks>
        /// This will be used in conjunction with the <see cref="Criteria"/>, <see cref="WhereClause"/>, and <see cref="ColumnName"/> properties.
        /// </remarks>
        public string Operator { get; set; }

        /// <summary>
        /// How many rows were returned from using the root SQL code.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send report].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send report]; otherwise, <c>false</c>.
        /// </value>
        public bool SendReport { get; set; }

        /// <summary>
        /// The SQL statement that gets built from the audit object properties.
        /// </summary>
        public string SqlStatementToCheck { get; set; }

        /// <summary>
        /// Gets or sets the template color scheme for the data HTML table in emails.
        /// </summary>
        /// <value>
        /// The template color scheme.
        /// </value>
        public EmailTableTemplate TemplateColorScheme { get; set; }

        /// <summary>
        /// Stores any error message from exceptions. This is mostly to get a verbose description of the failure.
        /// </summary>
        public string FailedMessage { get; set; }

        /// <summary>
        /// Whether or not to test the rows that may get returned.
        /// </summary>
        public bool TestReturnedRows { get; set; }

        /// <summary>
        /// Whether or not to use the criteria set for this audit test.
        /// </summary>
        public bool UseCriteria { get; set; }

        /// <summary>
        /// A where clause that will be added to the root SQL code. This is only done for this test.
        /// </summary>
        public string WhereClause { get; set; }

        /// <summary>
        /// A flag to indicate whether this test needs to process multiple results from the database.
        /// </summary>
        public bool MultipleResults { get; set; }

        /// <summary>
        /// A list of table names to be used in the email as headers.
        /// </summary>
        public ArrayList TableNames { get; set; }

        /// <summary>
        /// A flag to indicate the result of this test.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// A flag to indicate whether this test has run or not.
        /// </summary>
        public bool HasTestRun { get; set; }

        #endregion
    }
}
