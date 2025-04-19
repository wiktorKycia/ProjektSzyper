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
    internal class IsolatedFileAttribute : Attribute, ITestAction
    {
        private string? _filePath;
        private string? _originalFilePath;

        public string? FilePath => _filePath;
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            _filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            _originalFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = _filePath;
            File.WriteAllText(_filePath, "");
            test.Properties.Set("IsolatedFilePath", _filePath);
        }

        public void AfterTest(ITest test)
        {
            PasswordManager.PasswordFilePath = _originalFilePath!;
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
