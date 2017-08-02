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

namespace NDataAudit.Framework
{
    /// <summary>
    /// Summary description for AuditController.
    /// </summary>
    public class AuditController
    {
        #region  Declarations 

        private readonly AuditCollection _colAuditGroup;
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
        public AuditCollection AuditGroup
        {
            get
            {
                return _colAuditGroup;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AuditGroupName
        {
            get
            {
                return _auditGroupName;
            }

            set
            {
                _auditGroupName = value;
            }
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

                XmlNodeList auditList = auditGroup.GetElementsByTagName("audit");

                ProcessAudits(auditList);
            }
            catch (Exception ex)
            {
                //AuditLogger.Log(LogLevel.Debug, ex, ex.TargetSite + "::" + ex.Message, ex);
                // Site for future logger.
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

                GetEmailSettings(auditDoc, ref newAudit, auditBranch);

                GetDatabaseDetails(auditBranch, newAudit);

                GetSmtpDetails(auditBranch, newAudit);

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
                    newAudit.TestServer = newAudit.ConnectionString.DatabaseServer;
                }

                _colAuditGroup.Add(newAudit);
            }
        }

        private static void GetDatabaseDetails(XmlNode auditBranch, Audit newAudit)
        {
            var xmlElement = auditBranch["database"];
            if (xmlElement != null)
            {
                XmlNodeList sqlType = xmlElement.GetElementsByTagName("sqltype");
                newAudit.SqlType = (Audit.SqlStatementTypeEnum)Convert.ToInt32(sqlType[0].InnerText);

                XmlNodeList dbProvider = xmlElement.GetElementsByTagName("databaseprovider");
                newAudit.DatabaseProvider = dbProvider[0].InnerText;

                XmlNodeList connectionString = xmlElement.GetElementsByTagName("connectionstring");
                newAudit.ConnectionString = new AuditConnectionString(connectionString[0].InnerText, newAudit.DatabaseProvider);

                XmlNode commandTimeout = auditBranch["commandtimeout"];
                if (commandTimeout != null)
                {
                    newAudit.ConnectionString.CommandTimeout = commandTimeout.InnerText;
                }
                else
                {
                    newAudit.ConnectionString.CommandTimeout = "180";
                }

                XmlNode connectionTimeout = auditBranch["connectiontimeout"];
                if (connectionTimeout != null)
                {
                    newAudit.ConnectionString.ConnectionTimeout = connectionTimeout.InnerText;
                }
                else
                {
                    newAudit.ConnectionString.ConnectionTimeout = "15";
                }

                XmlNodeList orderbyNode = xmlElement.GetElementsByTagName("orderbyclause");
                if (orderbyNode.Count > 0)
                {
                    newAudit.OrderByClause = orderbyNode[0].InnerText;
                }
            }
        }

        private static void GetEmailSettings(XmlDocument auditDoc, ref Audit newAudit, XmlNode auditBranch)
        {
            // Process email list
            XmlNodeList emailList = auditDoc.GetElementsByTagName("email");
            if (emailList.Count > 0)
            {
                ProcessEmails(ref newAudit, emailList, Audit.EmailTypeEnum.Recipient); 
            }

            // Process cc email list
            XmlNodeList ccEmailList = auditDoc.GetElementsByTagName("ccemail");
            if (ccEmailList.Count > 0)
            {
                ProcessEmails(ref newAudit, ccEmailList, Audit.EmailTypeEnum.CarbonCopy); 
            }

            // Process email list
            XmlNodeList bccEmailList = auditDoc.GetElementsByTagName("bccemail");
            if (bccEmailList.Count > 0)
            {
                ProcessEmails(ref newAudit, bccEmailList, Audit.EmailTypeEnum.BlindCarbonCopy); 
            }

            // See if there is a custom email subject for this audit.
            var xmlElement = auditBranch["emailsubject"];
            if (xmlElement != null)
            {
                newAudit.EmailSubject = xmlElement.InnerText;
            }

            // See if there is a custom email subject for this audit.
            xmlElement = auditBranch["emailpriority"];
            if (xmlElement != null)
            {
                switch (xmlElement.InnerText.ToLower())
                {
                    case "low":
                        newAudit.EmailPriority = Audit.EmailPriorityEnum.Low;
                        break;
                    case "normal":
                        newAudit.EmailPriority = Audit.EmailPriorityEnum.Normal;
                        break;
                    case "high":
                        newAudit.EmailPriority = Audit.EmailPriorityEnum.High;
                        break;
                    default:
                        newAudit.EmailPriority = Audit.EmailPriorityEnum.High;
                        break;
                }
            }
            else
            {
                newAudit.EmailPriority = Audit.EmailPriorityEnum.High;
            }

            // See if there is a source email the FROM email address.
            var xmlSourceElement = auditBranch["sourceemail"];
            if (xmlSourceElement != null)
            {
                newAudit.SmtpSourceEmail = xmlSourceElement.InnerText;
            }
        }

        private static void ProcessEmails(ref Audit currentAudit, XmlNodeList auditEmails, Audit.EmailTypeEnum emailType)
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
                    case Audit.EmailTypeEnum.Recipient:
                        currentAudit.EmailSubscribers.Add(currEmail);
                        break;
                    case Audit.EmailTypeEnum.CarbonCopy:
                        currentAudit.EmailCarbonCopySubscribers.Add(currEmail);
                        break;
                    case Audit.EmailTypeEnum.BlindCarbonCopy:
                        currentAudit.EmailBlindCarbonCopySubscribers.Add(currEmail);
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

        private static void GetSmtpDetails(XmlNode auditBranch, Audit newAudit)
        {
            var xmlSmtpElement = auditBranch["smtp"];

            if (xmlSmtpElement["sourceemail"] != null)
            {
                newAudit.SmtpSourceEmail = xmlSmtpElement["sourceemail"].InnerText;
            }

            if (xmlSmtpElement["address"] != null)
            {
                newAudit.SmtpServerAddress = xmlSmtpElement["address"].InnerText;
            }

            if (xmlSmtpElement["port"] != null)
            {
                newAudit.SmtpPort = Convert.ToInt32(xmlSmtpElement["port"].InnerText);
            }
            else
            {
                newAudit.SmtpPort = 25;
            }

            if (xmlSmtpElement["usessl"] != null)
            {
                newAudit.SmtpUseSsl = bool.Parse(xmlSmtpElement["usessl"].InnerText);
            }

            // Process SMTP credentials, if any
            XmlNode xmlSmtpCredElement = xmlSmtpElement["smtpcredentials"];

            if (xmlSmtpCredElement != null)
            {
                newAudit.SmtpHasCredentials = true;

                if (xmlSmtpCredElement["username"] != null)
                {
                    newAudit.SmtpUserName = xmlSmtpCredElement["username"].InnerText;
                }

                if (xmlSmtpCredElement["password"] != null)
                {
                    newAudit.SmtpPassword = xmlSmtpCredElement["password"].InnerText;
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

                currentAudit.Tests.Add(newTest);
            }
        }

        #endregion
    }
}
