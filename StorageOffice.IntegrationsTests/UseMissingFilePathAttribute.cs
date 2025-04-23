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
    /// This abstract class is made to avoid redundancy of code in all attributes for tests to check the performance of methods in the absence of the expected file.
    /// </summary>
    internal abstract class UseMissingFilePathAttribute : Attribute, ITestAction
    {
        protected string? OriginalFilePath; // Path to the production file, which can be saved and then restored to the class after the test to keep it working properly. Used in derived classes

        public ActionTargets Targets => ActionTargets.Test;

        public abstract void BeforeTest(ITest test); //Method used to prepare class before test

        public abstract void AfterTest(ITest test); //Method reestablish paths in class after test
    }
}
