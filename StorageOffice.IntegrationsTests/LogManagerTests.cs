using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.IntegrationsTests
{
    internal class LogManagerTests
    {
        [Test, IsolatedLogsFile]
        public void AddNewLog_AppendsNewLogCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;

            LogManager.AddNewLog("Info: trying to add a test log");

            string[] lines = File.ReadAllLines(path!);
            Assert.That(lines[0], Is.EqualTo($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Info: trying to add a test log").Or.EqualTo($"[{DateTime.Now.AddSeconds(-1):yyyy-MM-dd HH:mm:ss}] Info: trying to add a test log"));
        }

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

        [Test, IsolatedLogsFile]
        public void GetAllLogs_WhenNoLogsFound_ReturnsCorrectInformation()
        {
            string logs = LogManager.GetAllLogs();

            Assert.That(logs, Is.EqualTo("No logs found"));
        }

        [Test, UseMissingLogsFilePath]
        public void GetAllLogs_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => LogManager.GetAllLogs());
        }

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

        [Test, IsolatedLogsFile]
        public void GetLogsFromSpecificDate_WhenNoRelevantLogsFound_ReturnsCorrectInformation()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, $"[2025-04-11 10:10:10] Info: added new user\n");

            string logs = LogManager.GetLogsFromSpecificDate(new DateTime(2025, 4, 10, 1, 1, 1));

            Assert.That(logs, Is.EqualTo("No relevant logs found"));
        }

        [Test, UseMissingLogsFilePath]
        public void GetLogsFromSpecificDate_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => LogManager.GetLogsFromSpecificDate(new DateTime(2025, 4, 10, 1, 1, 1)));
        }
    }
}
