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
        public static event Action<string> FileErrorFound;

        static LogManager()
        {
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Dispose();
            }

            FileErrorFound += (problem) => AddNewLog($"Error: problem in logs.txt file found - {problem}");
        }

        public static void AddNewLog(string logText)
        {
            File.AppendAllText(_logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {logText}\n");
        }

        public static string GetLogsFromSpecificDate(DateTime date)
        {
            try
            {
                string results = "";
                List<string> logs = File.ReadAllLines(_logFilePath).ToList();
                foreach (string log in logs)
                {
                    if (log.Substring(1, 10) == date.ToString("yyyy-MM-dd"))
                    {
                        results += $"{log}\n";
                    }
                }
                if (results == "") return "No relevant logs found";
                return results;
            }
            catch (FileNotFoundException)
            {
                FileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file logs.txt was removed while the application was running!");
            }
        }

        public static string GetAllLogs()
        {
            try
            {
                string logs = File.ReadAllText(_logFilePath);
                if (logs == "") return "No logs found";
                return logs;
            }
            catch (FileNotFoundException)
            {
                FileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file logs.txt was removed while the application was running!");
            }
        }
    }
}
