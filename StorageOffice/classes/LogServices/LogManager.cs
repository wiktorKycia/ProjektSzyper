using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.LogServices
{
    public class LogManager
    {
        public static string? LogFilePath = "../../../../StorageOffice/Data/logs.txt";
        private static event Action<string> _fileErrorFound;

        static LogManager()
        {
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Dispose();
            }

            _fileErrorFound += (problem) => AddNewLog($"Error: problem in logs.txt file found - {problem}");
        }

        public static void AddNewLog(string logText)
        {
            File.AppendAllText(LogFilePath!, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {logText}\n");
        }

        public static string GetLogsFromSpecificDate(DateTime date)
        {
            try
            {
                string results = "";
                List<string> logs = File.ReadAllLines(LogFilePath!).ToList();
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
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file logs.txt was removed while the application was running!");
            }
        }

        public static string GetAllLogs()
        {
            try
            {
                string logs = File.ReadAllText(LogFilePath!);
                if (logs == "") return "No logs found";
                return logs;
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file logs.txt was removed while the application was running!");
            }
        }
    }
}
