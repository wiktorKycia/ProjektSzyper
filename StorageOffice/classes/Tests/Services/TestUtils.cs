using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.Tests.Services
{
    internal class TestUtils
    {
        public static void WithTxtFileBackup(string path, Action testLogic)
        {
            string backupFilePath = path.Replace(".txt", "_testBackup");
            File.Copy(path, backupFilePath, true);
            try
            {
                testLogic?.Invoke();
            }
            finally
            {
                File.Copy(backupFilePath, path, true);
                File.Delete(backupFilePath);
            }
        }
    }
}
