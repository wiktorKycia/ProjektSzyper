using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.LogServices
{
    /// <summary>
    /// This class is responsible for methods to add and read system logs in various ways.
    /// </summary>
    public class LogManager
    {
        public static string? LogFilePath = "../../../../StorageOffice/Data/logs.txt";
        private static event Action<string> _fileErrorFound;

        /// <summary>
        /// This static constructor takes care of creating a file for logs if it doesn't exist and sets the logging method for the event in case of an error with the file.
        /// </summary>
        static LogManager()
        {
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Dispose();
            }

            _fileErrorFound += (problem) => AddNewLog($"Error: problem in logs.txt file found - {problem}");
        }

        /// <summary>
        /// Responsible for adding logs to the file. The time of its addition is placed before each log.
        /// </summary>
        /// <remarks>
        /// The text that is added as a log should mostly start with 'Info: ', 'Warning: ' or 'Error: '.
        /// </remarks>
        /// <param name="logText">The text that has to be added as a log message.</param>
        public static void AddNewLog(string logText)
        {
            File.AppendAllText(LogFilePath!, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {logText}\n");
        }

        /// <summary>
        /// Returns all logs from a specific day or information about their absence.
        /// </summary>
        /// <param name="date">DateTime object, which represents the date from which the logs are returned.</param>
        /// <returns>A string that contains all logs from specific date or information about their absence. This string preserves transitions to a new line from the file.</returns>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with logs. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// Returns all logs or information about their absence.
        /// </summary>
        /// <returns>A string that contains all logs or information about their absence. This string preserves transitions to a new line from the file.</returns>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with logs. This is done just in case someone deletes this file while the system is running.</exception>
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
