
using System;
using System.IO;
using NDataAudit.Framework;
using NLog;

namespace DataAuditor.CommandLine
{
    ///	<summary>
    ///	Summary	description	for	NAuditModule.
    ///	</summary>
    internal sealed class DataAuditorConsole
    {
        private static readonly Logger AuditLogger = LogManager.GetCurrentClassLogger();
        private static AuditTesting _auditTesting;

        internal static void Main(string[] cmdArgs)
        {
            var errorStream = new StringWriter();

            try
            {
                string firstArg = cmdArgs[0];

                if (firstArg.Length > 0)
                {
                    if (CheckForValidFile(firstArg))
                    {
                        ProcessAudits(firstArg);
                    }
                    else
                    {
                        Console.Write("Invalid file name. File or path not found.");
                        errorStream.Write("-1");
                        Console.SetOut(errorStream);
                    }
                }
                else
                {
                    Console.Write("No file to process!");
                    errorStream.Write("-1");
                    Console.SetOut(errorStream);
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log(LogLevel.Debug, ex.TargetSite + "::" + ex.Message, ex);
                Console.WriteLine(ex.TargetSite + "::" + ex.Message);
            }
        }

        private static void ProcessAudits(string auditGroupFile)
        {
            var colAudits = new AuditController(auditGroupFile);

            _auditTesting = new AuditTesting(colAudits.AuditGroup);

            // Wire up the events
            _auditTesting.AuditTestingStarting += _auditTesting_AuditTestingStarting;
            _auditTesting.CurrentAuditRunning += _auditTesting_CurrentAuditRunning;
            _auditTesting.CurrentAuditDone += _auditTesting_CurrentAuditDone;

            _auditTesting.RunAudits();

            Console.WriteLine("DataAuditor ran {0} test(s).", _auditTesting.Audits.Count);
        }

        private static bool CheckForValidFile(string fileNameToCheck)
        {
            var checkFileInfo = new FileInfo(fileNameToCheck);

            bool result = checkFileInfo.Exists;

            return result;
        }

        private static void _auditTesting_AuditTestingStarting()
        {
            Console.WriteLine("Starting audits...");
        }

        private static void _auditTesting_CurrentAuditRunning(int AuditNumber, string AuditName)
        {
            Console.WriteLine("This is audit number {0}", (AuditNumber + 1).ToString());
            Console.WriteLine("Running the {0} test.", AuditName);
        }

        private static void _auditTesting_CurrentAuditDone(int AuditNumber, string AuditName)
        {
            Console.WriteLine("Finished the {0} test.", AuditName);
        }
    }
}