using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.IntegrationsTests
{
    /// <summary>
    /// This class can be used as a direct attribute to prepare a file in the Temp folder(usually in the AppData\Local\Temp) on which integration tests for methods in LogManager class could run, and then delete it after the test is completed.
    /// </summary>
    internal class IsolatedLogsFileAttribute : IsolatedFileAttribute
    {
        /// <summary>
        /// Creates an empty file in the Temp folder before executing the test and sets the IsolatedFilePath property with the path to the file so that the test itself can use it.
        /// Saves the path to the file that LogManager used at first and sets the new one in LogManager for tests.
        /// </summary>
        /// <param name="test">The test that will be launched</param>
        public override void BeforeTest(ITest test)
        {
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            OriginalFilePath = LogManager.LogFilePath;
            LogManager.LogFilePath = FilePath;
            File.WriteAllText(FilePath, "");
            test.Properties.Set("IsolatedFilePath", FilePath);
        }

        /// <summary>
        /// Deletes the file that was created before running the test and reestablishes the original file path in LogManager.
        /// </summary>
        /// <param name="test">The test that was launched</param>
        public override void AfterTest(ITest test)
        {
            LogManager.LogFilePath = OriginalFilePath!;
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
