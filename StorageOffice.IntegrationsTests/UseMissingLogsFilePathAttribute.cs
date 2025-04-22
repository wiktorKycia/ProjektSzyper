using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.IntegrationsTests
{
    /// <summary>
    /// This class can be used as a direct attribute to prepare a path to the Temp folder(usually in the AppData\Local\Temp) on which integration tests for methods in LogManager class could run, but without the file created.
    /// Therefore, the method may be tested for situations when no file is created.
    /// </summary>
    internal class UseMissingLogsFilePathAttribute : UseMissingFilePathAttribute
    {
        /// <summary>
        /// Creates a path to the file that doesn't exist in the Temp folder before executing the test.
        /// Saves the path that LogManager used at first and sets the new one in LogManager for tests.
        /// </summary>
        /// <param name="test">The test that will be launched</param>
        public override void BeforeTest(ITest test)
        {
            OriginalFilePath = LogManager.LogFilePath;
            LogManager.LogFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        }

        /// <summary>
        /// Reestablishes the original file path in LogManager.
        /// </summary>
        /// <param name="test">The test that was launched</param>
        public override void AfterTest(ITest test)
        {
            LogManager.LogFilePath = OriginalFilePath!;
        }
    }
}
