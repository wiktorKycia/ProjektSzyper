using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.LogServices
{
    internal class LogManager
    {
        private const string _logFilePath = "../../../Data/logs.txt";

        static LogManager()
        {
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Dispose();
            }
        }

        public static void AddNewLog(string logText)
        {
            File.AppendAllText(_logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {logText}\n");
        }

        public static string GetAllLogs()
        {
            return File.ReadAllText(_logFilePath);
        }
    }
}
