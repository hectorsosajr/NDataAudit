
using System;
using System.IO;
using NDataAudit.Framework;

namespace DataAuditor.CommandLine
{
    ///	<summary>
    ///	Summary	description	for	NDataAuditModule.
    ///	</summary>
    internal sealed class DataAuditorConsole
    {
        private enum ExitCode : int
        {
            Success = 0,
            InvalidFilename = 1,
            NoFileToProcess = 2,
            UnknownError = 10
        }

        private static AuditTesting _auditTesting;

        internal static int Main(string[] cmdArgs)
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

                        return (int)ExitCode.InvalidFilename;
                    }
                }
                else
                {
                    Console.Write("No file to process!");
                    errorStream.Write("-1");
                    Console.SetOut(errorStream);

                    return (int)ExitCode.NoFileToProcess;
                }
            }
            catch (Exception ex)
            {
                //AuditLogger.Log(LogLevel.Debug, ex.TargetSite + "::" + ex.Message, ex);
                Console.WriteLine(ex.TargetSite + "::" + ex.Message);

                return (int)ExitCode.UnknownError;
            }

            return (int)ExitCode.Success;
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

            switch (_auditTesting.Audits.AuditResultOutputType)
            {
                case OutputType.Audit:
                    break;
                case OutputType.UnitTest:
                    AuditUtils.SendAuditUnitTestReportEmail(colAudits.AuditGroup);
                    break;
                case OutputType.Alert:
                    //AuditUtils.SendSingleAuditFailureEmail(colAudits.AuditGroup,)
                    break;
                case OutputType.Report:
                    break;
            }

            // Try Unit Test Type report until the rest gets sorted out
            //AuditUtils.SendAuditUnitTestReportEmail(colAudits.AuditGroup);

            Console.WriteLine("Sent results email to recipients.");
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

        private static void _auditTesting_CurrentAuditRunning(int auditNumber, string auditName)
        {
            Console.WriteLine("This is audit number {0}", (auditNumber + 1).ToString());
            Console.WriteLine("Running the {0} test.", auditName);
        }

        private static void _auditTesting_CurrentAuditDone(int auditNumber, string auditName)
        {
            Console.WriteLine("Finished the {0} test.", auditName);
        }
    }
}