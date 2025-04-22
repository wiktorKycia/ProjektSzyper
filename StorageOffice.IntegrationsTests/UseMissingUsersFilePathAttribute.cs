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
    internal class UseMissingUsersFilePathAttribute : UseMissingFilePathAttribute
    {
        public override void BeforeTest(ITest test)
        {
            OriginalFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        }

        public override void AfterTest(ITest test)
        {
            PasswordManager.PasswordFilePath = OriginalFilePath!;
        }
    }
}
