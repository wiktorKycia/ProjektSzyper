using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.LogServices;

namespace StorageOffice.IntegrationsTests
{
    /// <summary>
    /// Contains integration tests that verify the functioning of methods from the LogManager class.
    /// </summary>
    internal class LogManagerTests
    {
        /// <summary>
        /// Checks that the AddNewLog method correctly adds a new log to the file in the format: "[yyyy-MM-dd HH:mm:ss] Type: log".
        /// </summary>
        [Test, IsolatedLogsFile]
        public void AddNewLog_AppendsNewLogCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;

            LogManager.AddNewLog("Info: trying to add a test log");

            string[] lines = File.ReadAllLines(path!);
            Assert.That(lines[0], Is.EqualTo($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Info: trying to add a test log").Or.EqualTo($"[{DateTime.Now.AddSeconds(-1):yyyy-MM-dd HH:mm:ss}] Info: trying to add a test log")); // This checks two adjacent seconds, in case the method call occurred at a second earlier than the check in the test
        }

        /// <summary>
        /// Checks that the GetAllLogs method correctly returns a string with logs with the maintenance of transitions to a new line.
        /// </summary>
        [Test, IsolatedLogsFile]
        public void GetAllLogs_ReturnsAllLogsCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string log1 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Info: trying to add a test log\n";
            string log2 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Info: added new user\n";
            File.AppendAllText(path!, log1 + log2);

            string logs = LogManager.GetAllLogs();

            Assert.That(logs, Is.EqualTo(log1 + log2));
        }

        /// <summary>
        /// Checks that the GetAllLogs method correctly returns information about no logs, rather than an empty string, in the situation of lack of logs.
        /// </summary>
        [Test, IsolatedLogsFile]
        public void GetAllLogs_WhenNoLogsFound_ReturnsCorrectInformation()
        {
            string logs = LogManager.GetAllLogs();

            Assert.That(logs, Is.EqualTo("No logs found"));
        }

        /// <summary>
        /// Checks that the GetAllLogs method correctly throws a FileNotFoundException, in case the logs data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingLogsFilePath]
        public void GetAllLogs_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => LogManager.GetAllLogs());
        }

        /// <summary>
        /// Checks that the GetLogsFromSepcificDate method correctly returns a string with logs from specific date with the maintenance of transitions to a new line.
        /// </summary>
        [Test, IsolatedLogsFile]
        public void GetLogsFromSepcificDate_ReturnsAllSpecififcLogsCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string log1 = $"[2025-04-10 10:10:10] Info: trying to add a test log\n";
            string log2 = $"[2025-04-11 10:10:10] Info: added new user\n";
            File.AppendAllText(path!, log1 + log2);

            string logs = LogManager.GetLogsFromSpecificDate(new DateTime(2025, 4, 10, 1, 1, 1));

            Assert.That(logs, Is.EqualTo(log1));
        }

        /// <summary>
        /// Checks that the GetLogsFromSpecificDate method correctly returns information about no logs, rather than an empty string, in the situation of lack of logs from specific date.
        /// </summary>
        [Test, IsolatedLogsFile]
        public void GetLogsFromSpecificDate_WhenNoRelevantLogsFound_ReturnsCorrectInformation()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, $"[2025-04-11 10:10:10] Info: added new user\n");

            string logs = LogManager.GetLogsFromSpecificDate(new DateTime(2025, 4, 10, 1, 1, 1));

            Assert.That(logs, Is.EqualTo("No relevant logs found"));
        }

        /// <summary>
        /// Checks that the GetLogsFromSpecificDate method correctly throws a FileNotFoundException, in case the logs data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingLogsFilePath]
        public void GetLogsFromSpecificDate_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => LogManager.GetLogsFromSpecificDate(new DateTime(2025, 4, 10, 1, 1, 1)));
        }
    }
}
