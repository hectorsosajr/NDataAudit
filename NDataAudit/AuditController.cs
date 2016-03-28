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
        private static readonly Logger AuditLogger = LogManager.GetCurrentClassLogger();

        #region  Declarations 

        private AuditCollection _colAuditGroup;
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
        /// 
        /// </summary>
        /// <param name="auditFilePath">The full path of the Audit group xml file.</param>
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
        public static List<TableTemplate> TableTemplates { get; set; }

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
                AuditLogger.Log(LogLevel.Debug, ex, ex.TargetSite + "::" + ex.Message, ex);
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

                newAudit.SqlStatement = auditBranch["sqlcommand"].InnerText;
                newAudit.Name = auditBranch.Attributes[0].InnerText;
                auditDoc.LoadXml(auditBranch.OuterXml);

                // Process email list
                XmlNodeList emailList = auditDoc.GetElementsByTagName("email");
                ProcessEmails(ref newAudit, emailList);

                // See if there is a custom email subject for this audit.
                var xmlElement = auditBranch["emailSubject"];
                if (xmlElement != null)
                {
                    newAudit.EmailSubject = xmlElement.InnerText;
                }
                
                // Process SMTP credentials, if any
                var xmlSmtpElement = auditBranch["smtpcredentials"];
                if (xmlSmtpElement != null)
                {
                    newAudit.SmtpHasCredentials = true;

                    if (xmlSmtpElement["port"] != null)
                    {
                        newAudit.SmtpPort = Convert.ToInt32(xmlSmtpElement["port"].InnerText);
                    }

                    if (xmlSmtpElement["username"] != null)
                    {
                        newAudit.SmtpUserName = xmlSmtpElement["username"].InnerText;
                    }

                    if (xmlSmtpElement["password"] != null)
                    {
                        newAudit.SmtpPassword = xmlSmtpElement["password"].InnerText;
                    }

                    if (xmlSmtpElement["usessl"] != null)
                    {
                        newAudit.SmtpUseSsl = bool.Parse(xmlSmtpElement["usessl"].InnerText);
                    }
                }

                // See if we should show the threshold message for this audit.
                var xmlShowThresholdElement = auditBranch["showThresholdMessage"];
                if (xmlShowThresholdElement != null)
                {
                    newAudit.ShowThresholdMessage = bool.Parse(xmlShowThresholdElement.InnerText);
                }

                // See if we should show the query text for this audit.
                var xmlShowQueryElement = auditBranch["showQueryMessage"];
                if (xmlShowQueryElement != null)
                {
                    newAudit.ShowQueryMessage = bool.Parse(xmlShowQueryElement.InnerText);
                }

                // See if we should show the comments and instructions for this audit.
                var xmlShowCommentElement = auditBranch["showComments"];
                if (xmlShowCommentElement != null)
                {
                    newAudit.ShowCommentMessage = bool.Parse(xmlShowCommentElement.InnerText);
                }

                XmlNodeList testList = auditDoc.GetElementsByTagName("test");
                ProcessTests(ref newAudit, testList);

                XmlNodeList sqlType = auditDoc.GetElementsByTagName("sqltype");
                newAudit.SqlType = (Audit.SqlStatementTypeEnum) Convert.ToInt32(sqlType[0].InnerText);

                XmlNodeList connectionString = auditDoc.GetElementsByTagName("connectionstring");
                newAudit.ConnectionString = connectionString[0].InnerText;

                XmlNodeList orderbyNode = auditDoc.GetElementsByTagName("orderbyclause");
                if (orderbyNode.Count > 0)
                {
                    newAudit.OrderByClause = orderbyNode[0].InnerText;
                }

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

                _colAuditGroup.Add(newAudit);
            }
        }		
        
        private static void ProcessEmails(ref Audit currentAudit, XmlNodeList auditEmails)
        {
            int nodeCount;

            var counter = auditEmails.Count;

            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                string currEmail = null;
                XmlNode emailNode = null;

                emailNode = auditEmails[nodeCount];
                currEmail = emailNode.InnerText;

                currentAudit.EmailSubscribers.Add(currEmail);
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
                
                if (newTest.Criteria.ToUpper() == "COUNTROWS")
                {
                    newTest.RowCount = Convert.ToInt32(columnNode["rowcount"].InnerText);
                }

                newTest.FailIfConditionIsTrue = Convert.ToBoolean(columnNode["failiftrue"].InnerText);
                newTest.Instructions = columnNode["instructions"].InnerText;

                var xmlElement = columnNode["sendReport"];
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

                xmlElement = columnNode["reportTemplate"];
                if (xmlElement != null)
                {
                    string templateName = columnNode["reportTemplate"].InnerText;

                    TableTemplate currTemplate = TableTemplates.FirstOrDefault(t => t.Name.ToLower() == templateName.ToLower());

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
