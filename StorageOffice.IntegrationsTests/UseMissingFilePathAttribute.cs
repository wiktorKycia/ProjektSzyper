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
    internal class UseMissingFilePathAttribute : Attribute, ITestAction
    {
        private string? _originalFilePath;

        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            _originalFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        }

        public void AfterTest(ITest test)
        {
            PasswordManager.PasswordFilePath = _originalFilePath!;
        }
    }
}
