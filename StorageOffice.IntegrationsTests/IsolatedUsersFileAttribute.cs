using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.IntegrationsTests
{
    /// <summary>
    /// This class can be used as a direct attribute to prepare a file in the Temp folder(usually in the AppData\Local\Temp) on which integration tests for methods in PasswordManager class could run, and then delete it after the test is completed.
    /// </summary>
    internal class IsolatedUsersFileAttribute : IsolatedFileAttribute
    {
        /// <summary>
        /// Creates an empty file in the Temp folder before executing the test and sets the IsolatedFilePath property with the path to the file so that the test itself can use it.
        /// Saves the path to the file that PasswordManager used at first and sets the new one in PasswordManager for tests.
        /// </summary>
        /// <param name="test">The test that will be launched</param>
        public override void BeforeTest(ITest test)
        {
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            OriginalFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = FilePath;
            File.WriteAllText(FilePath, "");
            test.Properties.Set("IsolatedFilePath", FilePath);
        }

        /// <summary>
        /// Deletes the file that was created before running the test and reestablishes the original file path in PasswordManager.
        /// </summary>
        /// <param name="test">The test that was launched</param>
        public override void AfterTest(ITest test)
        {
            PasswordManager.PasswordFilePath = OriginalFilePath!;
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
