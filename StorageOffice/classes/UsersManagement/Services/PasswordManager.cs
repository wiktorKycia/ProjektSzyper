using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StorageOffice.classes.UsersManagement.Services
{
    public class PasswordManager
    {
        public static string PasswordFilePath = "../../../../StorageOffice/Data/users.txt";
        private static event Action<string, bool> _passwordVerified;
        private static event Action<string> _fileErrorFound;
        private static event Action<string, string> _userDataChanged;

        static PasswordManager()
        {
            if (!File.Exists(PasswordFilePath))
            {
                Console.WriteLine("The users file does not exist. The system creates a new one.");
                File.Create(PasswordFilePath).Dispose();
            }

            _passwordVerified += (username, success) => LogManager.AddNewLog($"Info: login of user {username} - {(success ? "successful" : "unsuccessful")}");
            _fileErrorFound += (problem) => LogManager.AddNewLog($"Error: problem in users.txt file found - {problem}");
            _userDataChanged += (actionType, username) => LogManager.AddNewLog($"Info: action was performed on data of a user named {username} - {actionType}");
        }

        public static void SaveNewUser(string username, string password, Role role)
        {
            try
            {
                if (File.ReadLines(PasswordFilePath).Any(line => line.Split(',')[0] == username))
                {
                    throw new InvalidOperationException($"User {username} already exists in the system");
                }

                string hashedPassword = HashPassword(password);
                File.AppendAllText(PasswordFilePath, $"{username},{hashedPassword},{role}\n");
                Console.WriteLine($"User {username} has been saved");
                _userDataChanged?.Invoke("added a new user", username);
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        public static void DeleteUser(string username)
        {
            try
            {
                if (!File.ReadLines(PasswordFilePath).Any(line => line.Split(',')[0] == username))
                {
                    throw new InvalidOperationException($"User {username} does not exist in the system");
                }

                List<string> fileLines = File.ReadAllLines(PasswordFilePath).ToList();
                int userIndex = fileLines.FindIndex(line => line.Split(',')[0] == username);
                fileLines.RemoveAt(userIndex);
                File.WriteAllLines(PasswordFilePath, fileLines.ToArray());
                Console.WriteLine($"User {username} has been deleted");
                _userDataChanged?.Invoke("deleted a user", username);
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        public static void OverwriteUserData(string username, string newData, int dataColumnNumber)
        {
            try
            {
                if (File.ReadLines(PasswordFilePath).Any(line => line.Split(',')[0] == username))
                {
                    string[] fileLines = File.ReadAllLines(PasswordFilePath);
                    int userIndex = Array.FindIndex(fileLines, line => line.Split(',')[0] == username);
                    string userLineInFile = fileLines[userIndex];
                    string[] parts = userLineInFile.Split(',');

                    parts[dataColumnNumber] = newData;
                    fileLines[userIndex] = string.Join(",", parts);

                    File.WriteAllLines(PasswordFilePath, fileLines);
                }
                else
                {
                    throw new InvalidOperationException($"User '{username}' does not exist");
                }
            }
            catch(FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        public static void ChangeUsername(string username, string newUsername)
        {
            if (string.IsNullOrEmpty(newUsername))
            {
                throw new ArgumentNullException(null, "The new username must not be empty! ");
            }
            else if (!Regex.IsMatch(newUsername, @"^[a-zA-Z0-9_.ąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                throw new ArgumentException("The new username can only contain letters, numbers, characters '_' and '.'! ");
            }
            OverwriteUserData(username, newUsername, 0);
            _userDataChanged?.Invoke($"changed user's name to {newUsername}", username);
        }

        public static void ChangeUserPassword(string username, string password)
        {
            OverwriteUserData(username, HashPassword(password), 1);
            _userDataChanged?.Invoke("changed user's password", username);
        }

        public static void ChangeUserRole(string username, Role role)
        {
            OverwriteUserData(username, role.ToString(), 2);
            _userDataChanged?.Invoke("changed user's role", username);
        }

        public static List<User> GetAllUsers()
        {
            try
            {
                List<User> users = new List<User>();
                List<string> usersData = File.ReadAllLines(PasswordFilePath).ToList();

                foreach (string line in usersData)
                {
                    string[] parts = line.Split(',');

                    if (!Enum.TryParse(parts[2], true, out Role userRole))
                    {
                        _fileErrorFound?.Invoke($"incorrect user's role was found in users.txt file for user {parts[0]}");
                        throw new FormatException($"Error: Incorrect user's role was found in users.txt file for user {parts[0]}");
                    }

                    User user = new User(parts[0], userRole);
                    users.Add(user);
                }

                return users;
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        public static bool CheckFile()
        {
            try
            {
                List<string> lines = File.ReadAllLines(PasswordFilePath).ToList();
                Console.WriteLine(lines.Count);
                if (lines.Count > 0)
                {
                    foreach (string line in lines)
                    {
                        if (line.Split(',').Length < 3)
                        {
                            _fileErrorFound?.Invoke("an anomaly was detected in number of users' data");
                            throw new FormatException("Error: An anomaly was detected in the system data!");
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        public static Role? VerifyPasswordAndGetRole(string username, string password)
        {
            try
            {
                string hashedPassword = HashPassword(password);
                foreach (var line in File.ReadLines(PasswordFilePath))
                {
                    var parts = line.Split(',');

                    if (parts[0] == username && parts[1] == hashedPassword)
                    {
                        _passwordVerified?.Invoke(username, true);
                        if (!Enum.TryParse(parts[2], true, out Role userRole))
                        {
                            _fileErrorFound?.Invoke($"incorrect user's role was found in users.txt file for user {username}");
                            throw new FormatException($"Error: Incorrect user's role was found in users.txt file for user {username}");
                        }

                        return userRole;
                    }
                }
                _passwordVerified?.Invoke(username, false);

                return null;
            }
            catch (FileNotFoundException)
            {
                _fileErrorFound?.Invoke("the file was removed while the application was running");
                throw new FileNotFoundException("The file users.txt was removed while the application was running!");
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
