//*********************************************************************
// File:       		AuditController.cs
// Author:  	    Hector Sosa, Jr
// Date:			2/16/2005
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		2/16/2005	    Created
// Hector Sosa, Jr		3/21/2005		Changed the private variables
//										to be compliant with C# naming
//										conventions.
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NLog;

namespace NDataAudit.Framework
{
    /// <summary>
    /// Summary description for AuditController.
    /// </summary>
    public class AuditController
    {
        #region  Declarations 

        private static AuditCollection _colAuditGroup;
        private string _auditGroupName;

        #endregion

        #region  Constructors 

        /// <summary>
        /// Empty constructor
        /// </summary>
        public AuditController()
        {
            // Empty constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditController"/> class.
        /// </summary>
        /// <param name="auditFilePath">The path to the XML audit file.</param>
        public AuditController(string auditFilePath)
        {
            _colAuditGroup = new AuditCollection();

            TableTemplates = AuditUtils.GeTableTemplates();

            LoadAuditGroup(auditFilePath);
        }

        #endregion

        #region  Properties 

        /// <summary>
        /// 
        /// </summary>
        public AuditCollection AuditGroup => _colAuditGroup;

        /// <summary>
        /// 
        /// </summary>
        public string AuditGroupName
        {
            get => _auditGroupName;

            set => _auditGroupName = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<EmailTableTemplate> TableTemplates { get; set; }

        #endregion

        #region  Public Members 

        /// <summary>
        /// Top level method that handles processing the xml audit group document and converting it
        /// into the different audit objects.
        /// </summary>
        /// <param name="xmlGroup">The full path to the audit group xml file.</param>
        public void LoadAuditGroup(string xmlGroup)
        {
            var auditGroup = new XmlDocument();

            try
            {
                auditGroup.Load(xmlGroup);

                _auditGroupName = auditGroup.DocumentElement.Attributes[0].InnerText;
                _colAuditGroup.AuditGroupName = _auditGroupName;

                GetEmailSettings(auditGroup);
                GetDatabaseDetails(auditGroup);
                GetOutputStyles(auditGroup);
                GetSmtpDetails(auditGroup);

                XmlNodeList auditList = auditGroup.GetElementsByTagName("audit");

                ProcessAudits(auditList);
            }
            catch (Exception ex)
            {
                //AuditLogger.Log(LogLevel.Debug, ex, ex.TargetSite + "::" + ex.Message, ex);
                // Site for future logger.

                var logger = AuditUtils.GetFileLogger();

                logger.Error(ex, ex.Message);
            }
        }

        #endregion

        #region Private Members
        
        private void ProcessAudits(XmlNodeList auditList)
        {
            int nodeCount;
            XmlDocument auditDoc = new XmlDocument();

            int counter = auditList.Count;
            
            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                var newAudit = new Audit();

                XmlNode auditBranch = auditList[nodeCount];
                newAudit.Name = auditBranch.Attributes[0].InnerText;
                auditDoc.LoadXml(auditBranch.OuterXml);

                GetReportUiElements(auditBranch, newAudit);

                XmlNodeList testList = auditDoc.GetElementsByTagName("test");
                ProcessTests(ref newAudit, testList);

                XmlNodeList includeDataNode = auditDoc.GetElementsByTagName("includedatainemail");
                if (includeDataNode.Count > 0)
                {
                    newAudit.IncludeDataInEmail = bool.Parse(includeDataNode[0].InnerText);
                }

                XmlNodeList testRunOn = auditDoc.GetElementsByTagName("testrunon");
                if (testRunOn.Count > 0)
                {
                    newAudit.TestServer = testRunOn[0].InnerText;
                }
                else
                {
                    newAudit.TestServer = _colAuditGroup.ConnectionString.DatabaseServer;
                }

                _colAuditGroup.Add(newAudit);
            }
        }

        private static void GetDatabaseDetails(XmlDocument auditGroup)
        {
            var xmlElement = auditGroup.GetElementsByTagName("database");
            {
                XmlElement dbProvider = xmlElement[0]["databaseprovider"];
                if (dbProvider != null)
                {
                    _colAuditGroup.DatabaseProvider = dbProvider.InnerText;
                }

                XmlElement connectionString = xmlElement[0]["connectionstring"];
                if (connectionString != null)
                {
                    _colAuditGroup.ConnectionString =
                        new AuditConnectionString(connectionString.InnerText, _colAuditGroup.DatabaseProvider);
                }

                XmlNode commandTimeout = auditGroup["commandtimeout"];
                if (commandTimeout != null)
                {
                    _colAuditGroup.ConnectionString.CommandTimeout = commandTimeout.InnerText;
                }
                else
                {
                    _colAuditGroup.ConnectionString.CommandTimeout = "180";
                }

                XmlNode connectionTimeout = auditGroup["connectiontimeout"];
                if (connectionTimeout != null)
                {
                    _colAuditGroup.ConnectionString.ConnectionTimeout = connectionTimeout.InnerText;
                }
                else
                {
                    _colAuditGroup.ConnectionString.ConnectionTimeout = "15";
                }
            }
        }

        private void GetOutputStyles(XmlDocument auditGroup)
        {
            var xmlOuputList = auditGroup.GetElementsByTagName("outputformat");
            {
                XmlElement templateElement = xmlOuputList[0]["template"];
                if (templateElement != null)
                {
                    string templateName = templateElement.InnerText;
                    EmailTableTemplate currTemplate = TableTemplates.FirstOrDefault(t => t.Name.ToLower() == templateName.ToLower());
                    _colAuditGroup.TemplateColorScheme = currTemplate;
                }

                XmlElement outputStyleElement = xmlOuputList[0]["outputstyle"];
                if (outputStyleElement != null)
                {
                    string outputStyle = outputStyleElement.InnerText;

                    foreach (string name in Enum.GetNames(typeof(OutputType)))
                    {
                        if (String.Equals(name, outputStyle, StringComparison.CurrentCultureIgnoreCase))
                        {
                            OutputType currOutputType = (OutputType)Enum.Parse(typeof(OutputType), name);

                            _colAuditGroup.AuditResultOutputType = currOutputType;

                            break;
                        }
                    }
                }
            }
        }

        private static void GetEmailSettings(XmlDocument auditDoc)
        {
            // Process email list
            XmlNodeList emailList = auditDoc.GetElementsByTagName("email");
            if (emailList.Count > 0)
            {
                ProcessEmails(emailList, EmailTypeEnum.Recipient); 
            }

            // Process cc email list
            XmlNodeList ccEmailList = auditDoc.GetElementsByTagName("ccemail");
            if (ccEmailList.Count > 0)
            {
                ProcessEmails(ccEmailList, EmailTypeEnum.CarbonCopy); 
            }

            // Process email list
            XmlNodeList bccEmailList = auditDoc.GetElementsByTagName("bccemail");
            if (bccEmailList.Count > 0)
            {
                ProcessEmails(bccEmailList, EmailTypeEnum.BlindCarbonCopy); 
            }

            // See if there is a custom email subject for this audit.
            var xmlElement = auditDoc["emailsubject"];
            if (xmlElement != null)
            {
                _colAuditGroup.EmailSubject = xmlElement.InnerText;
            }

            // See what is the email priority
            xmlElement = auditDoc["emailpriority"];
            if (xmlElement != null)
            {
                switch (xmlElement.InnerText.ToLower())
                {
                    case "low":
                        _colAuditGroup.EmailPriority = EmailPriorityEnum.Low;
                        break;
                    case "normal":
                        _colAuditGroup.EmailPriority = EmailPriorityEnum.Normal;
                        break;
                    case "high":
                        _colAuditGroup.EmailPriority = EmailPriorityEnum.High;
                        break;
                    default:
                        _colAuditGroup.EmailPriority = EmailPriorityEnum.High;
                        break;
                }
            }
            else
            {
                _colAuditGroup.EmailPriority = EmailPriorityEnum.High;
            }

            // See if there is a source email the FROM email address.
            var xmlSourceElement = auditDoc["sourceemail"];
            if (xmlSourceElement != null)
            {
                _colAuditGroup.SmtpSourceEmail = xmlSourceElement.InnerText;
            }
        }

        private static void ProcessEmails(XmlNodeList auditEmails, EmailTypeEnum emailType)
        {
            int nodeCount;

            var counter = auditEmails.Count;

            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                string currEmail = null;
                XmlNode emailNode = null;

                emailNode = auditEmails[nodeCount];
                currEmail = emailNode.InnerText;

                switch (emailType)
                {
                    case EmailTypeEnum.Recipient:
                        _colAuditGroup.EmailSubscribers.Add(currEmail);
                        break;
                    case EmailTypeEnum.CarbonCopy:
                        _colAuditGroup.EmailCarbonCopySubscribers.Add(currEmail);
                        break;
                    case EmailTypeEnum.BlindCarbonCopy:
                        _colAuditGroup.EmailBlindCarbonCopySubscribers.Add(currEmail);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(emailType), emailType, null);
                }
            }
        }

        private static void GetReportUiElements(XmlNode auditBranch, Audit newAudit)
        {
            // See if we should show the threshold message for this audit.
            var xmlShowThresholdElement = auditBranch["showthresholdmessage"];

            if (xmlShowThresholdElement != null)
            {
                newAudit.ShowThresholdMessage = bool.Parse(xmlShowThresholdElement.InnerText);
            }

            // See if we should show the query text for this audit.
            var xmlShowQueryElement = auditBranch["showquerymessage"];
            if (xmlShowQueryElement != null)
            {
                newAudit.ShowQueryMessage = bool.Parse(xmlShowQueryElement.InnerText);
            }

            // See if we should show the comments and instructions for this audit.
            var xmlShowCommentElement = auditBranch["showcomments"];
            if (xmlShowCommentElement != null)
            {
                newAudit.ShowCommentMessage = bool.Parse(xmlShowCommentElement.InnerText);
            }
        }

        private static void GetSmtpDetails(XmlDocument auditDoc)
        {
            XmlNodeList smtpNode = auditDoc.GetElementsByTagName("smtp"); //auditDoc["smtp"];

            if (smtpNode[0]["sourceemail"] != null)
            {
                _colAuditGroup.SmtpSourceEmail = smtpNode[0]["sourceemail"].InnerText;
            }

            if (smtpNode[0]["address"] != null)
            {
                _colAuditGroup.SmtpServerAddress = smtpNode[0]["address"].InnerText;
            }

            if (smtpNode[0]["port"] != null)
            {
                _colAuditGroup.SmtpPort = Convert.ToInt32(smtpNode[0]["port"].InnerText);
            }
            else
            {
                _colAuditGroup.SmtpPort = 25;
            }

            if (smtpNode[0]["usessl"] != null)
            {
                _colAuditGroup.SmtpUseSsl = bool.Parse(smtpNode[0]["usessl"].InnerText);
            }

            if (smtpNode[0]["emailclient"] != null)
            {
                EmailClientHtml parsedClient;
                
                bool result = Enum.TryParse(smtpNode[0]["emailclient"].InnerText.ToLower(), out parsedClient);

                if (result)
                {
                    _colAuditGroup.EmailClientToTarget = parsedClient;
                }
                else
                {
                    _colAuditGroup.EmailClientToTarget = EmailClientHtml.None;
                }
            }

            // Process SMTP credentials, if any
            XmlNode xmlSmtpCredElement = smtpNode[0]["smtpcredentials"];

            if (xmlSmtpCredElement != null)
            {
                _colAuditGroup.SmtpHasCredentials = true;

                if (xmlSmtpCredElement["username"] != null)
                {
                    _colAuditGroup.SmtpUserName = xmlSmtpCredElement["username"].InnerText;
                }

                if (xmlSmtpCredElement["password"] != null)
                {
                    _colAuditGroup.SmtpPassword = xmlSmtpCredElement["password"].InnerText;
                }
            }
        }

        private static void ProcessTests(ref Audit currentAudit, XmlNodeList auditTests)
        {
            int nodeCount = 0;
            int counter = 0;

            counter = auditTests.Count;

            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                var newTest = new AuditTest();

                XmlNode columnNode = auditTests[nodeCount];

                newTest.ColumnName = columnNode["column"].InnerText;
                newTest.Operator = columnNode["operator"].InnerText;
                newTest.Criteria = columnNode["criteria"].InnerText;
                newTest.WhereClause = newTest.ColumnName + " " + newTest.Operator + " " + newTest.Criteria;
                newTest.TestReturnedRows = Convert.ToBoolean(columnNode["testreturnedrows"].InnerText);
                newTest.UseCriteria = Convert.ToBoolean(columnNode["usecriteria"].InnerText);

                newTest.SqlStatementToCheck = columnNode["sqlcommand"].InnerText;

                newTest.SqlType = (Audit.SqlStatementTypeEnum)Convert.ToInt32(columnNode["sqltype"].InnerText);

                if (newTest.Criteria.ToUpper() == "COUNTROWS")
                {
                    newTest.RowCount = Convert.ToInt32(columnNode["rowcount"].InnerText);
                }

                newTest.FailIfConditionIsTrue = Convert.ToBoolean(columnNode["failiftrue"].InnerText);

                var xmlElement = columnNode["instructions"];
                if (xmlElement != null)
                {
                newTest.Instructions = columnNode["instructions"].InnerText;
                }

                xmlElement = columnNode["sendReport"];
                if (xmlElement != null)
                {
                    newTest.SendReport = Convert.ToBoolean(columnNode["sendReport"].InnerText);
                }

                xmlElement = columnNode["multipleResults"];
                if (xmlElement != null)
                {
                    newTest.MultipleResults = Convert.ToBoolean(columnNode["multipleResults"].InnerText);

                    if (newTest.MultipleResults)
                    {
                        xmlElement = columnNode["tableNames"];
                        if (xmlElement != null)
                        {
                            int tableCount;

                            string[] stringSeparators = new[] {"::"};
                            var tableCounter = xmlElement.InnerText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                            for (tableCount = 0; tableCount < tableCounter.Length; tableCount++)
                            {
                                newTest.TableNames.Add(tableCounter[tableCount]);
                            }
                        }
                    }
                }

                xmlElement = columnNode["reporttemplate"];
                if (xmlElement != null)
                {
                    string templateName = columnNode["reporttemplate"].InnerText;

                    EmailTableTemplate currTemplate = TableTemplates.FirstOrDefault(t => t.Name.ToLower() == templateName.ToLower());

                    newTest.TemplateColorScheme = currTemplate;
                }
                else
                {
                    newTest.TemplateColorScheme = AuditUtils.GetDefaultTemplate();
                }

                currentAudit.Test = newTest;
            }
        }

        #endregion
    }
}
