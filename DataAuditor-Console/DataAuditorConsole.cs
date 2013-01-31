
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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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
                _logger.LogException(LogLevel.Debug, ex.TargetSite + "::" + ex.Message, ex);
            }
        }

        private static void ProcessAudits(string auditGroupFile)
        {
            var colAudits = new AuditController(auditGroupFile);

            var auditTesting = new AuditTesting(colAudits.AuditGroup);
            auditTesting.RunAudits();
        }

        private static bool CheckForValidFile(string fileNameToCheck)
        {
            var checkFileInfo = new FileInfo(fileNameToCheck);

            bool result = checkFileInfo.Exists;

            return result;
        }
    }
}