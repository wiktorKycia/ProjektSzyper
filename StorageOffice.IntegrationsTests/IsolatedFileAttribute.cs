using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace StorageOffice.IntegrationsTests
{
    /// <summary>
    /// This class can be used as a direct attribute to prepare a file in the Temp folder(usually in the AppDataLocalTemp) on which integration tests could run, and then delete it after the test is completed.
    /// It can also be used as a base class for more advanced implementations of this mechanism.
    /// </summary>
    internal class IsolatedFileAttribute : Attribute, ITestAction
    {
        protected string? FilePath; // Path to the file on which the tests run
        protected string? OriginalFilePath; // Path to the production file, which can be saved and then restored to the class after the test to keep it working properly. Used in derived classes

        public ActionTargets Targets => ActionTargets.Test;

        /// <summary>
        /// Creates an empty file in the Temp folder before executing the test and sets the IsolatedFilePath property with the path to the file so that the test itself can use it.
        /// </summary>
        /// <param name="test">The test that will be launched</param>
        public virtual void BeforeTest(ITest test)
        {
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            File.WriteAllText(FilePath, "");
            test.Properties.Set("IsolatedFilePath", FilePath);
        }

        /// <summary>
        /// Deletes the file that was created before running the test
        /// </summary>
        /// <param name="test">The test that was launched</param>
        public virtual void AfterTest(ITest test)
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
