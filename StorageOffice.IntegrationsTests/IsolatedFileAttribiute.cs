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
    internal class IsolatedFileAttribiute : Attribute, ITestAction
    {
        private string? _filePath;

        public string? FilePath => _filePath;
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            _filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            File.WriteAllText(_filePath, "");
            test.Properties.Set("IsolatedFilePath", _filePath);
        }

        public void AfterTest(ITest test)
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
