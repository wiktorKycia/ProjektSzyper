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
    internal class IsolatedUsersFileAttribute : IsolatedFileAttribute
    {
        public override void BeforeTest(ITest test)
        {
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            OriginalFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = FilePath;
            File.WriteAllText(FilePath, "");
            test.Properties.Set("IsolatedFilePath", FilePath);
        }

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
