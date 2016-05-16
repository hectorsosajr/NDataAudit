//*********************************************************************
// File:       		AuditTesting.cs
// Author:  	    Hector Sosa, Jr
// Date:			3/1/2005
//*********************************************************************
//Change Log
//*********************************************************************
// USER					DATE        COMMENTS
// Hector Sosa, Jr		3/1/2005	Created
// Hector Sosa, Jr      3/7/2005    Changed the name from AuditDB
//                                  to AuditTesting.
// Hector Sosa, Jr      3/9/2005    Added code to handle COUNTROWS
//                                  custom criteria. Added code to
//                                  fill the TestFailureMessage in
//                                  the AuditTest object when there
//                                  are exceptions.
// Hector Sosa, Jr		3/21/2005	Added event handlers for running a
//									single audit, instead of all of them.
//*********************************************************************

using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using NAudit.Data;

namespace NAudit.Framework
{
    // Delegates for groups of Audits
    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.CurrentAuditRunning"/>AuditTesting.CurrentAuditRunning event.
    /// </summary>
    public delegate void CurrentAuditRunningEventHandler(int auditIndex, string auditName);

    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.CurrentAuditDone"/>AuditTesting.CurrentAuditDone event.
    /// </summary>
    public delegate void CurrentAuditDoneEventHandler(int auditIndex, string auditName);

    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.AuditTestingStarting"/>AuditTesting.AuditTestingStarting event.
    /// </summary>
    public delegate void AuditTestingStartingEventHandler();

    // Delegates for single Audits
    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.CurrentSingleAuditRunning"/>AuditTesting.CurrentSingleAuditRunning event.
    /// </summary>
    public delegate void CurrentSingleAuditRunningEventHandler(Audit currentAudit);

    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.CurrentSingleAuditDone"/>AuditTesting.CurrentSingleAuditDone event.
    /// </summary>
    public delegate void CurrentSingleAuditDoneEventHandler(Audit currentAudit);

    /// <summary>
    /// Delegate for the <seealso cref="AuditTesting.AuditTestingEnded"/>AuditTesting.AuditTestingEnded event.
    /// </summary>
    public delegate void AuditTestingEndedEventHandler();

    /// <summary>
    /// Summary description for AuditTesting.
    /// </summary>
    public class AuditTesting
    {
        #region  Declarations

        private AuditCollection _colAudits;

        private DbProviderCache _providers = null;

        #endregion

        #region Events

        /// <summary>
        /// This event fires when a new <see cref="Audit"/> starts running, and it's part of a group run.
        /// </summary>
        public event CurrentAuditRunningEventHandler CurrentAuditRunning;

        /// <summary>
        /// This event fires when the current <see cref="Audit"/> is done running its tests.
        /// </summary>
        public event CurrentAuditDoneEventHandler CurrentAuditDone;

        /// <summary>
        /// This event fires when <see cref="AuditTesting"/> object starts processing all of 
        /// the <see cref="Audit"/>s in the <see cref="AuditCollection"/> object. 
        /// </summary>
        public event AuditTestingStartingEventHandler AuditTestingStarting;

        /// <summary>
        /// This event fires when the current <see cref="Audit"/> starts running its tests.
        /// </summary>
        public event CurrentSingleAuditRunningEventHandler CurrentSingleAuditRunning;

        /// <summary>
        /// This event fires when a new <see cref="Audit"/> starts running.
        /// </summary>
        public event CurrentSingleAuditDoneEventHandler CurrentSingleAuditDone;

        /// <summary>
        /// This event fires when <see cref="AuditTesting"/> object ends processing all of 
        /// the <see cref="Audit"/>s in the <see cref="AuditCollection"/> object. 
        /// </summary>
        public event AuditTestingEndedEventHandler AuditTestingEnded;

        #endregion

        #region  Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public AuditTesting()
        {
            _providers = new DbProviderCache();
        }

        /// <summary>
        /// Constructor that lets you load an AuditCollection.
        /// </summary>
        /// <param name="colAudits">An AuditCollection object</param>
        public AuditTesting(AuditCollection colAudits)
        {
            _colAudits = colAudits;

            _providers = new DbProviderCache();
        }

        #endregion

        #region  Properties

        /// <summary>
        /// The internal <see cref="AuditCollection"/> object used in this class.
        /// </summary>
        public AuditCollection Audits
        {
            get
            {
                return _colAudits;
            }

            set
            {
                _colAudits = value;
            }
        }

        #endregion

        #region  Public Members

        /// <summary>
        /// Runs all of the audits in the collection.
        /// </summary>
        public void RunAudits()
        {
            if (_colAudits == null)
            {
                throw new NoAuditsLoadedException("No audits have been loaded. Please load some audits and try again.");
            }
                GetAudits();
            }

        /// <summary>
        /// Run a single audit.
        /// </summary>
        /// <param name="currentAudit">The Audit object to use</param>
        public void RunAudit(ref Audit currentAudit)
        {
            OnSingleAuditRunning(currentAudit);
            RunTests(ref currentAudit);			
            OnSingleAuditDone(currentAudit);
        }

        #endregion

        #region Private Members

        private void GetAudits()
        {
            int auditCount = 0;
            
            int tempFor1 = _colAudits.Count;

            OnAuditTestingStarting();
            
            for (auditCount = 0; auditCount < tempFor1; auditCount++)
            {
                Audit currAudit = null;

                currAudit = _colAudits[auditCount];
                
                OnCurrentAuditRunning(auditCount, currAudit);
                RunTests(ref currAudit);
                OnCurrentAuditDone(auditCount, currAudit);
            }
        }

        private void RunTests(ref Audit currentAudit)
        {
            int testCount;
            int tempFor1 = currentAudit.Tests.Count;
            
            for (testCount = 0; testCount < tempFor1; testCount++)
            {
                DataSet dsTest = GetTestDataSet(ref currentAudit, testCount);

                if (dsTest.Tables.Count == 0)
                {
                    AuditTest currTest = currentAudit.Tests[testCount];

                    if (!currTest.FailIfConditionIsTrue)
                    {
                        currentAudit.Result = true;
                    }
                    else
                    {
                        // TODO: This is a hack that needs to be fixed.
                        // I want the test to succeed, but not send
                        // any emails. When this app was first built,
                        // it always assumed that the audit would fail or
                        // succeed with no further processing. This is to handle
                        // the weird case where there are actually two thresholds;
                        // the first one is the usual one, and the second one
                        // is for the data itself.
                        // TODO: Think of a better way of doing this!
                        if (currTest.SendReport)
                        {
                            currentAudit.Result = true;
                        }
                        else
                        {
                            currentAudit.Result = false;
                            PrepareResultsEmailData(currentAudit.Tests[testCount].SqlStatementToCheck, testCount, currentAudit,
                                             dsTest);
                        }
                    }
                }
                else
                {
                    var currTest = currentAudit.Tests[testCount];

                    int rowCount = dsTest.Tables[0].Rows.Count;					
        
                    if (currTest.TestReturnedRows)
                    {
                        if (currTest.Criteria.ToUpper() == "COUNTROWS")
                        {
                            string threshold;
                            switch (currTest.Operator)
                            {
                                case ">":
                                    if (rowCount > currTest.RowCount)
                                    {
                                        if (!currTest.FailIfConditionIsTrue)
                                        {
                                            currentAudit.Result = true;
                                        }
                                        else
                                        {
                                            threshold = currentAudit.Tests[testCount].RowCount.ToString(CultureInfo.InvariantCulture);
                                            currentAudit.Tests[testCount].FailedMessage = "The failure threshold was greater than " + threshold + " rows. This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                        }
                                    }
                                    else
                                    {
                                        if (rowCount <= currTest.RowCount)
                                        {
                                            // Threshold was not broken, so the test passes.
                                            currentAudit.Result = true;
                                        }
                                        else
                                        {
                                            threshold =
                                                currentAudit.Tests[testCount].RowCount.ToString(
                                                    CultureInfo.InvariantCulture);
                                            currentAudit.Tests[testCount].FailedMessage =
                                                "The failure threshold was greater than " + threshold +
                                                " rows. This audit returned " +
                                                rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                        }
                                    }
                                    break;
                                case ">=":
                                case "=>":
                                    if (rowCount >= currTest.RowCount)
                                    {
                                        currentAudit.Result = true;
                                    }
                                    else
                                    {
                                        threshold = currentAudit.Tests[testCount].RowCount.ToString(CultureInfo.InvariantCulture);
                                        currentAudit.Tests[testCount].FailedMessage = "The failure threshold was greater than or equal to " + threshold + " rows. This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                    }
                                    break;
                                case "<":
                                    if (rowCount < currTest.RowCount)
                                    {
                                        currentAudit.Result = true;
                                    }
                                    else
                                    {
                                        threshold = currentAudit.Tests[testCount].RowCount.ToString(CultureInfo.InvariantCulture);
                                        currentAudit.Tests[testCount].FailedMessage = "The failure threshold was less than " + threshold + " rows. This audit returned " + rowCount + " rows.";
                                    }
                                    break;
                                case "<=":
                                case "=<":
                                    if (rowCount <= currTest.RowCount)
                                    {
                                        currentAudit.Result = true;
                                    }
                                    else
                                    {
                                        threshold = currentAudit.Tests[testCount].RowCount.ToString(CultureInfo.InvariantCulture);
                                        currentAudit.Tests[testCount].FailedMessage = "The failure threshold was less than or equal to " + threshold + " rows. This audit returned " + rowCount + " rows.";
                                    }
                                    break;
                                case "=":
                                    if (rowCount == currTest.RowCount)
                                    {
                                        if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                                        {
                                            currentAudit.Result = false;
                                        }
                                        else
                                        {
                                            currentAudit.Result = true;
                                        }
                                    }
                                    else
                                    {
                                        if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                                        {
                                            currentAudit.Result = false;
                                        }
                                        else
                                        {
                                            currentAudit.Result = true;
                                        }

                                        threshold = currentAudit.Tests[testCount].RowCount.ToString(CultureInfo.InvariantCulture);
                                        currentAudit.Tests[testCount].FailedMessage = "The failure threshold was equal to " + threshold + " rows. This audit returned " + rowCount + " rows.";
                                    }
                                    break;
                                case "<>":
                                case "!=":
                                    if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                                    {
                                        currentAudit.Result = false;
                                    }
                                    else
                                    {
                                        currentAudit.Result = true;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (rowCount > 0)
                            {
                                if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                                {
                                    currentAudit.Result = false;
                                }
                                else
                                {
                                    currentAudit.Result = true;
                                }
                            }
                            else
                            {
                                if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                                {
                                    currentAudit.Result = false;

                                    currentAudit.Tests[testCount].FailedMessage = "This audit was set to have more than zero rows returned. " + "This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                }
                                else
                                {
                                    currentAudit.Result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (rowCount == 0)
                        {
                            currentAudit.Result = true;
                        }
                        else
                        {
                            currentAudit.Tests[testCount].FailedMessage = "This audit was set to not return any rows. " + "This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                        }
                    }

                    if (currentAudit.Result == false)
                    {
                        if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                        {
                            PrepareResultsEmailData(currentAudit.Tests[testCount].SqlStatementToCheck, testCount, currentAudit, dsTest);
                        }
                    }
                    else
                    {
                        if (currentAudit.Tests[testCount].FailIfConditionIsTrue)
                        {
                            if (currentAudit.Tests[testCount].SendReport)
                            {
                                // It's not really a failure. Just want to send a report-like email.
                                PrepareResultsEmailData(currentAudit.Tests[testCount].SqlStatementToCheck, testCount, currentAudit, dsTest);
                            }
                        }
                    }

                    dsTest.Dispose();

                    }
                }

            currentAudit.HasRun = true;
        }

        private DataSet GetTestDataSet(ref Audit auditToRun, int testIndex)
        {
            IAuditDbProvider currDbProvider = _providers.Providers[auditToRun.DatabaseProvider];

            currDbProvider.ConnectionString = auditToRun.ConnectionString.ToString();

            currDbProvider.CreateDatabaseSession();

            var dsAudit = new DataSet();

            string sql = BuildSqlStatement(auditToRun, testIndex);

            CommandType commandType = (CommandType) 0;
            
            if (auditToRun.SqlType == Audit.SqlStatementTypeEnum.SqlText)
            {
                commandType = CommandType.Text;
            }
            else if (auditToRun.SqlType == Audit.SqlStatementTypeEnum.StoredProcedure)
            {
                commandType = CommandType.StoredProcedure;
            }

            IDbCommand cmdAudit = currDbProvider.CreateDbCommand(sql, commandType, int.Parse(auditToRun.ConnectionString.CommandTimeout));
            IDbDataAdapter daAudit = currDbProvider.CreateDbDataAdapter(cmdAudit);

            int intCommandTimeout = cmdAudit.CommandTimeout;
            int intConnectionTimeout = currDbProvider.CurrentConnection.ConnectionTimeout;

            try
            {
                daAudit.Fill(dsAudit);
            }
            catch (Exception ex)
            {
                int intFound = 0;
                string strMsg = null;

                strMsg = ex.Message;

                intFound = (strMsg.IndexOf("Timeout expired.", 0, StringComparison.Ordinal) + 1);

                if (intFound == 1)
                {
                    auditToRun.Tests[testIndex].FailedMessage = "Timeout expired while running this audit. The connection timeout was " + intConnectionTimeout.ToString(CultureInfo.InvariantCulture) + " seconds. The command timeout was " + intCommandTimeout + " seconds.";

                    auditToRun.ErrorMessages.Add(auditToRun.Tests[testIndex].FailedMessage);
                }
                else
                {
                    auditToRun.Tests[testIndex].FailedMessage = strMsg;
                    auditToRun.ErrorMessages.Add(strMsg);
                }

                auditToRun.WasSuccessful = false;
            }
            finally
            {
                cmdAudit.Dispose();
            }

            return dsAudit;
        }

        private string ParseCriteria(string criteriaToParse, string columnName)
        {
            string result = criteriaToParse.Replace("TODAY", Today(columnName));

            return result;
        }

        private string BuildSqlStatement(Audit auditToParse, int testIndex)
        {
            string result;
            StringBuilder sql = new StringBuilder();

            string sqlStatement = auditToParse.SqlStatement;
            string orderBy = auditToParse.OrderByClause;
            bool useCriteria = auditToParse.Tests[testIndex].UseCriteria;

            if (useCriteria)
            {
                sql.Append(sqlStatement);

                string whereClause = BuildWhereClause(auditToParse, testIndex);
                sql.Append(" WHERE " + whereClause);

                if (orderBy != null)
                {
                    if (orderBy.Length > 0)
                    {
                        sql.Append(" ORDER BY " + orderBy);
                    }
                }

                result = sql.ToString();
            }
            else
            {
                result = auditToParse.SqlStatement;
            }

            auditToParse.Tests[testIndex].SqlStatementToCheck = result;

            return result;
        }

        private string BuildWhereClause(Audit auditToParse, int testIndex)
        {
            string result;

            AuditTest currTest = auditToParse.Tests[testIndex];

            string criteria = currTest.Criteria;
            string columnName = currTest.ColumnName;
            string Operator = currTest.Operator;

            // Add additional custom criteria inside this select statement
            switch (criteria.ToUpper())
            {
                case "TODAY":
                    result = Today(columnName) + Operator + "0";
                    auditToParse.Tests[testIndex].WhereClause = result;
                    break;
                default:
                    result = auditToParse.Tests[testIndex].WhereClause;
                    break;
            }

            return result;
        }

        private static string Today(string columnName)
        {
            string result = "DATEDIFF(day, COLUMN, getdate())";
            result = result.Replace("COLUMN", columnName);

            return result;
        }

        private static void PrepareResultsEmailData(string sqlTested, int testIndex, Audit testedAudit, DataSet testData)
        {
            var body = new StringBuilder();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

            if (testedAudit.Tests[testIndex].SendReport)
            {
                if (testedAudit.EmailSubject != null)
            {
                    body.AppendLine("<h2>" + testedAudit.EmailSubject + "</h2>");
                }

                body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowThresholdMessage)
            {
                body.Append(testedAudit.Tests[testIndex].FailedMessage + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowCommentMessage)
            {
                body.AppendLine("COMMENTS AND INSTRUCTIONS" + AuditUtils.HtmlBreak);
                body.AppendLine("============================" + AuditUtils.HtmlBreak);

                if (testedAudit.Tests[testIndex].Instructions != null)
                {
                if (testedAudit.Tests[testIndex].Instructions.Length > 0)
                {
                    body.Append(testedAudit.Tests[testIndex].Instructions.ToHtml() + AuditUtils.HtmlBreak);
                    body.AppendLine(AuditUtils.HtmlBreak);
                }
            }
            }

            if (testedAudit.IncludeDataInEmail)
            {
                if (testData.Tables.Count > 0)
                {
                    EmailTableTemplate currTemplate = testedAudit.Tests[testIndex].TemplateColorScheme;

                    string htmlData = AuditUtils.CreateHtmlData(testedAudit, testData, currTemplate);

                    body.Append(htmlData);
                }
            }

            body.AppendLine(AuditUtils.HtmlBreak);

            if (testedAudit.Tests[testIndex].SendReport)
            {
                body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak); 
            }
            else
            {
                body.Append("This audit ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
            }

            if (testedAudit.ShowQueryMessage)
            {
                body.Append(AuditUtils.HtmlBreak);
                body.Append("The '" + testedAudit.Name + "' audit has failed. The following SQL statement was used to test this audit :" + AuditUtils.HtmlBreak);
                body.Append(sqlTested.ToHtml() + AuditUtils.HtmlBreak);
                body.Append("<b>This query was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
            }

            string cleanBody = body.ToString().Replace("\r\n", string.Empty);

            SendEmail(testedAudit, cleanBody, sourceEmailDescription);
        }

        private static void SendEmail(Audit testedAudit, string body, string sourceEmailDescription)
        {
            var message = new MailMessage {IsBodyHtml = true};

            foreach (string recipient in testedAudit.EmailSubscribers)
            {
                message.To.Add(new MailAddress(recipient));
            }

            if (testedAudit.EmailCarbonCopySubscribers != null)
            {
                // Carbon Copies - CC
                foreach (string ccemail in testedAudit.EmailCarbonCopySubscribers)
                {
                    message.CC.Add(new MailAddress(ccemail));
                } 
            }
            
            if (testedAudit.EmailBlindCarbonCopySubscribers != null)
            {
                // Blind Carbon Copies - BCC
                foreach (string bccemail in testedAudit.EmailBlindCarbonCopySubscribers)
                {
                    message.Bcc.Add(new MailAddress(bccemail));
                }
            }

            message.Body = body;

            switch (testedAudit.EmailPriority)
            {
                case Audit.EmailPriorityEnum.Low:
                    message.Priority = MailPriority.Low;
                    break;
                case Audit.EmailPriorityEnum.Normal:
                    message.Priority = MailPriority.Normal;
                    break;
                case Audit.EmailPriorityEnum.High:
                    message.Priority = MailPriority.High;
                    break;
                default:
                    message.Priority = MailPriority.Normal;
                    break;
            }

            if (!string.IsNullOrEmpty(testedAudit.EmailSubject))
            {
                message.Subject = testedAudit.EmailSubject;
            }
            else
            {
                message.Subject = "Audit Failure - " + testedAudit.Name;
            }

            message.From = new MailAddress(testedAudit.SmtpSourceEmail, sourceEmailDescription);

            var server = new SmtpClient();

            if (testedAudit.SmtpHasCredentials)
            {
                server.Host = testedAudit.SmtpServerAddress;
                server.Port = testedAudit.SmtpPort;
                server.Credentials = new NetworkCredential(testedAudit.SmtpUserName, testedAudit.SmtpPassword);
                server.EnableSsl = testedAudit.SmtpUseSsl;
            }
            else
            {
                server.Host = testedAudit.SmtpServerAddress;
            }

            try
            {
            server.Send(message);
        }
            catch (SmtpException smtpEx)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(smtpEx.Message);

                if (smtpEx.InnerException != null)
                {
                    sb.AppendLine(smtpEx.InnerException.Message);
                }

                throw;
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Used to fire the <see cref="AuditTestingStarting"/> event.
        /// </summary>
        protected virtual void OnAuditTestingStarting()
        {
            if (AuditTestingStarting != null)
                AuditTestingStarting();
        }

        /// <summary>
        /// Used to fire the <see cref="CurrentAuditRunning"/> event.
        /// </summary>
        /// <param name="auditIndex">The index of this audit in the collection</param>
        /// <param name="currentAudit">The actual Audit object that is currently running</param>
        protected virtual void OnCurrentAuditRunning(int auditIndex, Audit currentAudit)
        {
            if (CurrentAuditRunning != null)
                CurrentAuditRunning(auditIndex, currentAudit.Name);
        }

        /// <summary>
        /// Used to fire the <see cref="CurrentAuditDone"/> event.
        /// </summary>
        /// <param name="auditIndex">The index of this audit in the collection</param>
        /// <param name="currentAudit">The actual Audit object that just got done running</param>
        protected virtual void OnCurrentAuditDone(int auditIndex, Audit currentAudit)
        {
            if (CurrentAuditDone != null)
                CurrentAuditDone(auditIndex, currentAudit.Name);
        }

        /// <summary>
        /// Used to fire the <see cref="CurrentSingleAuditRunning"/> event.
        /// </summary>
        /// <param name="currentAudit">The audit object that is currently running</param>
        protected virtual void OnSingleAuditRunning(Audit currentAudit)
        {
            if (CurrentSingleAuditRunning != null)
                CurrentSingleAuditRunning(currentAudit);
        }

        /// <summary>
        /// Used to fire the <see cref="CurrentSingleAuditDone"/> event.
        /// </summary>
        /// <param name="currentAudit">The audit object that just got done running</param>
        protected virtual void OnSingleAuditDone(Audit currentAudit)
        {
            if (CurrentSingleAuditDone != null)
                CurrentSingleAuditDone(currentAudit);
        }

        #endregion
    }	
    
    /// <summary>
    /// Custom <see cref="Exception"/> to alert users that no Audits have been loaded for testing.
    /// </summary>
    public class NoAuditsLoadedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">The message that will be associated with this <see cref="Exception"/> and that will be shown to users.</param>
        public NoAuditsLoadedException(string message) : base(message)
        {}
    }

    /// <summary>
    /// Class MissingRequiredConfigurations.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MissingRequiredConfigurations : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingRequiredConfigurations"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingRequiredConfigurations(string message) : base(message)
        {}
    }
}
