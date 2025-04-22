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
    internal abstract class UseMissingFilePathAttribute : Attribute, ITestAction
    {
        protected string? OriginalFilePath;

        public ActionTargets Targets => ActionTargets.Test;

        public abstract void BeforeTest(ITest test);

        public abstract void AfterTest(ITest test);
    }
}
