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
    internal class UseMissingLogsFilePathAttribute : UseMissingFilePathAttribute
    {
        public override void BeforeTest(ITest test)
        {
            OriginalFilePath = LogManager.LogFilePath;
            LogManager.LogFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        }

        public override void AfterTest(ITest test)
        {
            LogManager.LogFilePath = OriginalFilePath!;
        }
    }
}
