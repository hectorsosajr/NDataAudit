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
using System.Xml;

namespace NDataAudit.Framework
{
    /// <summary>
    /// Summary description for AuditController.
    /// </summary>
    public class AuditController
    {
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
        /// <param name="AuditFilePath">The full path of the Audit group xml file.</param>
        public AuditController(string AuditFilePath)
        {
            _colAuditGroup = new AuditCollection();

            LoadAuditGroup(AuditFilePath);
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

        #endregion

        #region  Public Members 

        /// <summary>
        /// Top level method that handles processing the xml audit group document and converting it
        /// into the different audit objects.
        /// </summary>
        /// <param name="xmlGroup">The full path to the audit group xml file.</param>
        public void LoadAuditGroup(string xmlGroup)
        {
            XmlDocument auditGroup = new XmlDocument();
            XmlNodeList auditList;

            auditGroup.Load(xmlGroup);

            _auditGroupName = auditGroup.DocumentElement.Attributes[0].InnerText;

            auditList = auditGroup.GetElementsByTagName("audit");

            ProcessAudits(auditList);
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
                Audit newAudit = new Audit();

                XmlNode auditBranch = auditList[nodeCount];

                newAudit.SqlStatement = auditBranch["sqlcommand"].InnerText;
                newAudit.Name = auditBranch.Attributes[0].InnerText;
                auditDoc.LoadXml(auditBranch.OuterXml);

                XmlNodeList emailList = auditDoc.GetElementsByTagName("email");
                ProcessEmails(ref newAudit, emailList);

                // See if there is a custom email subject for this audit.
                var xmlElement = auditBranch["emailSubject"];
                if (xmlElement != null)
                {
                    newAudit.EmailSubject = xmlElement.InnerText;
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
                    newAudit.TestServer =testRunOn[0].InnerText;
                }

                _colAuditGroup.Add(newAudit);
            }
        }		
        
        private void ProcessEmails(ref Audit currentAudit, XmlNodeList auditEmails)
        {
            int nodeCount;
            int counter;
            
            counter = auditEmails.Count;

            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                string currEmail = null;
                XmlNode emailNode = null;

                emailNode = auditEmails[nodeCount];
                currEmail = emailNode.InnerText;

                currentAudit.EmailSubscribers.Add(currEmail);
            }
        }		
        
        private void ProcessTests(ref Audit currentAudit, XmlNodeList auditTests)
        {
            int nodeCount = 0;
            int counter = 0;
            
            counter = auditTests.Count;
            
            for (nodeCount = 0; nodeCount < counter; nodeCount++)
            {
                AuditTest newTest = new AuditTest();

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

                xmlElement = columnNode["reportTemplate"];
                if (xmlElement != null)
                {
                    TableTemplateNames currTemplate;
                    string templateName = columnNode["reportTemplate"].InnerText;

                    switch (templateName.ToLower())
                    {
                        case "default":
                            currTemplate = TableTemplateNames.Default;
                            break;
                        case "redreport":
                            currTemplate = TableTemplateNames.RedReport;
                            break;
                        case "yellow":
                            currTemplate = TableTemplateNames.Yellow;
                            break;
                        case "yellowreport":
                            currTemplate = TableTemplateNames.YellowReport;
                            break;
                        default:
                            currTemplate = TableTemplateNames.Default;
                            break;
                    }

                    newTest.TemplateColorScheme = currTemplate;
                }
                else
                {
                    newTest.TemplateColorScheme = TableTemplateNames.Default;
                }

                currentAudit.Tests.Add(newTest);
            }
        }

        #endregion 
    }
}
