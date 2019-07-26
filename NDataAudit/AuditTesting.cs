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
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
// Hector Sosa, Jr      4/28/2018   Audits now store the result of the
//                                  dataset in the new ResultDataSet
//                                  property.
//*********************************************************************

using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using NDataAudit.Data;

namespace NDataAudit.Framework
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

        private static AuditCollection _colAudits;

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
        public void RunDataAudit(ref Audit currentAudit)
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

            ONDataAuditTestingStarting();

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
            int tempFor1 = 1;

            for (testCount = 0; testCount < tempFor1; testCount++)
            {
                DataSet dsTest = GetTestDataSet(ref currentAudit, testCount);
                currentAudit.ResultDataSet = dsTest;

                if (dsTest.Tables.Count == 0)
                {
                    AuditTest currTest = currentAudit.Test;

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
                            //PrepareResultsEmailData(currentAudit.Test.SqlStatementToCheck, currentAudit, dsTest);
                        }
                    }
                }
                else
                {
                    var currTest = currentAudit.Test;

                    int rowCount = dsTest.Tables[0].Rows.Count;

                    if (currTest.TestReturnedRows)
                    {
                        switch (currTest.Criteria.ToUpper())
                        {
                            case "COUNTROWS":
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
                                                threshold = currentAudit.Test.RowCount.ToString(CultureInfo.InvariantCulture);
                                                currentAudit.Test.FailedMessage = "The failure threshold was greater than " + threshold + " rows. This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
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
                                                    currentAudit.Test.RowCount.ToString(
                                                        CultureInfo.InvariantCulture);
                                                currentAudit.Test.FailedMessage =
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
                                            threshold = currentAudit.Test.RowCount.ToString(CultureInfo.InvariantCulture);
                                            currentAudit.Test.FailedMessage = "The failure threshold was greater than or equal to " + threshold + " rows. This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                        }
                                        break;
                                    case "<":
                                        if (rowCount < currTest.RowCount)
                                        {
                                            currentAudit.Result = true;
                                        }
                                        else
                                        {
                                            threshold = currentAudit.Test.RowCount.ToString(CultureInfo.InvariantCulture);
                                            currentAudit.Test.FailedMessage = "The failure threshold was less than " + threshold + " rows. This audit returned " + rowCount + " rows.";
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
                                            threshold = currentAudit.Test.RowCount.ToString(CultureInfo.InvariantCulture);
                                            currentAudit.Test.FailedMessage = "The failure threshold was less than or equal to " + threshold + " rows. This audit returned " + rowCount + " rows.";
                                        }
                                        break;
                                    case "=":
                                        if (rowCount == currTest.RowCount)
                                        {
                                            if (currentAudit.Test.FailIfConditionIsTrue)
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
                                            if (currentAudit.Test.FailIfConditionIsTrue)
                                            {
                                                currentAudit.Result = false;
                                            }
                                            else
                                            {
                                                currentAudit.Result = true;
                                            }

                                            threshold = currentAudit.Test.RowCount.ToString(CultureInfo.InvariantCulture);
                                            currentAudit.Test.FailedMessage = "The failure threshold was equal to " + threshold + " rows. This audit returned " + rowCount + " rows.";
                                        }
                                        break;
                                    case "<>":
                                    case "!=":
                                        if (currentAudit.Test.FailIfConditionIsTrue)
                                        {
                                            currentAudit.Result = false;
                                        }
                                        else
                                        {
                                            currentAudit.Result = true;
                                        }
                                        break;
                                }
                                break;
                            default:
                                if (rowCount > 0)
                                {
                                    if (currentAudit.Test.FailIfConditionIsTrue)
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
                                    if (currentAudit.Test.FailIfConditionIsTrue)
                                    {
                                        currentAudit.Result = false;

                                        currentAudit.Test.FailedMessage = "This audit was set to have more than zero rows returned. " + "This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                                    }
                                    else
                                    {
                                        currentAudit.Result = true;
                                    }
                                }
                                break;
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
                            currentAudit.Test.FailedMessage = "This audit was set to not return any rows. " + "This audit returned " + rowCount.ToString(CultureInfo.InvariantCulture) + " rows.";
                        }
                    }

                    dsTest.Dispose();

                    }
                }

            currentAudit.HasRun = true;
        }

        private DataSet GetTestDataSet(ref Audit auditToRun, int testIndex)
        {
            IAuditDbProvider currDbProvider = _providers.Providers[_colAudits.DatabaseProvider];

            currDbProvider.ConnectionString = _colAudits.ConnectionString.ToString();

            if (_colAudits.ConnectionString.ConnectionTimeout != null)
            {
                currDbProvider.ConnectionTimeout = _colAudits.ConnectionString.ConnectionTimeout;
            }
            else
            {
                currDbProvider.CommandTimeout = 15.ToString();
                _colAudits.ConnectionString.ConnectionTimeout = 15.ToString();
            }

            if (_colAudits.ConnectionString.CommandTimeout != null)
            {
                currDbProvider.CommandTimeout = _colAudits.ConnectionString.CommandTimeout;
            }
            else
            {
                currDbProvider.CommandTimeout = 30.ToString();
                _colAudits.ConnectionString.CommandTimeout = 30.ToString();
            }

            currDbProvider.CreateDatabaseSession();

            var dsAudit = new DataSet();

            string sql = BuildSqlStatement(auditToRun, testIndex);

            CommandType commandType = (CommandType) 0;

            if (auditToRun.Test.SqlType == Audit.SqlStatementTypeEnum.SqlText)
            {
                commandType = CommandType.Text;
            }
            else if (auditToRun.Test.SqlType == Audit.SqlStatementTypeEnum.StoredProcedure)
            {
                commandType = CommandType.StoredProcedure;
            }

            IDbCommand cmdAudit = currDbProvider.CreateDbCommand(sql, commandType, int.Parse(_colAudits.ConnectionString.CommandTimeout));

            IDbDataAdapter daAudit = currDbProvider.CreateDbDataAdapter(cmdAudit);

            string intConnectionTimeout = _colAudits.ConnectionString.ConnectionTimeout;
            string intCommandTimeout = _colAudits.ConnectionString.CommandTimeout;

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
                    auditToRun.Test.FailedMessage = "Timeout expired while running this audit. The connection timeout was " + intConnectionTimeout.ToString(CultureInfo.InvariantCulture) + " seconds. The command timeout was " + intCommandTimeout + " seconds.";

                    auditToRun.ErrorMessages.Add(auditToRun.Test.FailedMessage);
                }
                else
                {
                    auditToRun.Test.FailedMessage = strMsg;
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

            string sqlStatement = auditToParse.Test.SqlStatementToCheck;
            string orderBy = auditToParse.OrderByClause;
            bool useCriteria = auditToParse.Test.UseCriteria;

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
                result = sqlStatement;
            }

            return result;
        }

        private string BuildWhereClause(Audit auditToParse, int testIndex)
        {
            string result;

            AuditTest currTest = auditToParse.Test;

            string criteria = currTest.Criteria;
            string columnName = currTest.ColumnName;
            string Operator = currTest.Operator;
            string whereClause = auditToParse.Test.WhereClause;

            // Add additional custom criteria inside this select statement
            switch (criteria.ToUpper())
            {
                case "TODAY":
                    result = Today(columnName) + Operator + "0";
                    auditToParse.Test.WhereClause = result;
                    break;
                case "COUNTROWS":
                    result = whereClause.Replace("COUNTROWS", auditToParse.Test.RowCount.ToString());
                    auditToParse.Test.WhereClause = result;
                    break;
                default:
                    result = auditToParse.Test.WhereClause;
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

        //private static void PrepareResultsEmailData(string sqlTested, Audit testedAudit, DataSet testData)
        //{
        //    var body = new StringBuilder();

        //    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //    string sourceEmailDescription = config.AppSettings.Settings["sourceEmailDescription"].Value;

        //    if (testedAudit.Test.SendReport)
        //    {
        //        testedAudit.ShowThresholdMessage = false;
        //        testedAudit.ShowQueryMessage = false;

        //        if (testedAudit.EmailSubject != null)
        //        {
        //            body.AppendLine("<h2>" + testedAudit.EmailSubject + "</h2>");
        //        }

        //        body.Append("This report ran at " +
        //                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) +
        //                    AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
        //    }

        //    if (testedAudit.ShowThresholdMessage)
        //    {
        //        body.AppendLine("<h2>ERROR MESSAGE</h2>");
        //        body.Append(testedAudit.Test.FailedMessage + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
        //    }

        //    if (testedAudit.ShowCommentMessage)
        //    {
        //        body.AppendLine("COMMENTS AND INSTRUCTIONS" + AuditUtils.HtmlBreak);
        //        body.AppendLine("============================" + AuditUtils.HtmlBreak);

        //        if (testedAudit.Test.Instructions != null)
        //        {
        //            if (testedAudit.Test.Instructions.Length > 0)
        //            {
        //                body.Append(testedAudit.Test.Instructions.ToHtml() + AuditUtils.HtmlBreak);
        //                body.AppendLine(AuditUtils.HtmlBreak);
        //            }
        //        }
        //    }

        //    if (testedAudit.IncludeDataInEmail)
        //    {
        //        if (testData.Tables.Count > 0)
        //        {
        //            EmailTableTemplate currTemplate = testedAudit.Test.TemplateColorScheme;

        //            string htmlData = AuditUtils.CreateHtmlData(testedAudit, testData, currTemplate);

        //            body.Append(htmlData);
        //        }
        //    }

        //    body.AppendLine(AuditUtils.HtmlBreak);

        //    if (testedAudit.Test.SendReport)
        //    {
        //        body.Append("This report ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
        //        body.Append("<b>This report was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
        //    }
        //    else
        //    {
        //        body.Append("This audit ran at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + AuditUtils.HtmlBreak);
        //    }

        //    if (testedAudit.ShowQueryMessage)
        //    {
        //        body.Append(AuditUtils.HtmlBreak);
        //        body.Append("The '" + testedAudit.Name + "' audit has failed. The following SQL statement was used to test this audit :" + AuditUtils.HtmlBreak);
        //        body.Append(sqlTested.ToHtml() + AuditUtils.HtmlBreak);
        //        body.Append("<b>This query was run on: " + testedAudit.TestServer + "</b>" + AuditUtils.HtmlBreak + AuditUtils.HtmlBreak);
        //    }

        //    string cleanBody = body.ToString().Replace("\r\n", string.Empty);

        //    // SendEmail(testedAudit, cleanBody, sourceEmailDescription);
        //}

        //private static void SendEmail(Audit testedAudit, string body, string sourceEmailDescription)
        //{
        //    var message = new MailMessage {IsBodyHtml = true};

        //    foreach (string recipient in _colAudits.EmailSubscribers)
        //    {
        //        message.To.Add(new MailAddress(recipient));
        //    }

        //    if (_colAudits.EmailCarbonCopySubscribers != null)
        //    {
        //        // Carbon Copies - CC
        //        foreach (string ccemail in _colAudits.EmailCarbonCopySubscribers)
        //        {
        //            message.CC.Add(new MailAddress(ccemail));
        //        }
        //    }

        //    if (_colAudits.EmailBlindCarbonCopySubscribers != null)
        //    {
        //        // Blind Carbon Copies - BCC
        //        foreach (string bccemail in _colAudits.EmailBlindCarbonCopySubscribers)
        //        {
        //            message.Bcc.Add(new MailAddress(bccemail));
        //        }
        //    }

        //    message.Body = body;

        //    switch (testedAudit.EmailPriority)
        //    {
        //        case EmailPriorityEnum.Low:
        //            message.Priority = MailPriority.Low;
        //            break;
        //        case EmailPriorityEnum.Normal:
        //            message.Priority = MailPriority.Normal;
        //            break;
        //        case EmailPriorityEnum.High:
        //            message.Priority = MailPriority.High;
        //            break;
        //        default:
        //            message.Priority = MailPriority.Normal;
        //            break;
        //    }

        //    if (!string.IsNullOrEmpty(testedAudit.EmailSubject))
        //    {
        //        message.Subject = testedAudit.EmailSubject;
        //    }
        //    else
        //    {
        //        message.Subject = "Audit Failure - " + testedAudit.Name;
        //    }

        //    message.From = new MailAddress(_colAudits.SmtpSourceEmail, sourceEmailDescription);

        //    var server = new SmtpClient();

        //    if (_colAudits.SmtpHasCredentials)
        //    {
        //        server.UseDefaultCredentials = false;
        //        server.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        server.Host = _colAudits.SmtpServerAddress;
        //        server.Port = _colAudits.SmtpPort;
        //        server.Credentials = new NetworkCredential(_colAudits.SmtpUserName, _colAudits.SmtpPassword);
        //        server.EnableSsl = _colAudits.SmtpUseSsl;
        //    }
        //    else
        //    {
        //        server.Host = _colAudits.SmtpServerAddress;
        //    }

        //    try
        //    {
        //        server.Send(message);
        //    }
        //    catch (SmtpException smtpEx)
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        sb.AppendLine(smtpEx.Message);

        //        if (smtpEx.InnerException != null)
        //        {
        //            sb.AppendLine(smtpEx.InnerException.Message);
        //        }

        //        throw;
        //    }
        //}

        #endregion

        #region Protected Members

        /// <summary>
        /// Used to fire the <see cref="AuditTestingStarting"/> event.
        /// </summary>
        protected virtual void ONDataAuditTestingStarting()
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

    /// <inheritdoc />
    /// <summary>
    /// Custom <see cref="T:System.Exception" /> to alert users that no Audits have been loaded for testing.
    /// </summary>
    public class NoAuditsLoadedException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">The message that will be associated with this <see cref="T:System.Exception" /> and that will be shown to users.</param>
        public NoAuditsLoadedException(string message) : base(message)
        {}
    }

    /// <inheritdoc />
    /// <summary>
    /// Class MissingRequiredConfigurations.
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class MissingRequiredConfigurations : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NDataAudit.Framework.MissingRequiredConfigurations" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingRequiredConfigurations(string message) : base(message)
        {}
    }
}
