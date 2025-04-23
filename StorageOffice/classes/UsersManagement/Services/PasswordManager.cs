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
    /// <summary>
    /// This class is responsible for methods to manage stored user data
    /// </summary>
    public class PasswordManager
    {
        public static string PasswordFilePath = "../../../../StorageOffice/Data/users.txt";
        private static event Action<string, bool> _passwordVerified;
        private static event Action<string> _fileErrorFound;
        private static event Action<string, string> _userDataChanged;

        /// <summary>
        /// This static constructor takes care of creating a file for users' data if it doesn't exist and sets the logging methods for events in case of an error with the file, login of user or action performed on users' data.
        /// </summary>
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

        /// <summary>
        /// Adds a new user to the system files and adds appropriate log if no such user has been created yet.
        /// </summary>
        /// <param name="username">String that represents new user's name.</param>
        /// <param name="password">String that represents new user's password.</param>
        /// <param name="role">New user's role.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to add a user with a name that is already occupied in the system.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// Removes the user with the given name from the system files and adds the appropriate log if such a user exists.
        /// </summary>
        /// <param name="username">string that represents the name of the user to be deleted.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to remove a user with a name that does not exist in system files.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// Changes the specified data in the file for the user with the specified name.
        /// </summary>
        /// <param name="username">String containing the name of the user whose data is to be changed.</param>
        /// <param name="newData">String containing the new value to be used for overwriting.</param>
        /// <param name="dataColumnNumber">The number of the column in which the new value is to be entered in the file. 0 is the username, 1 is the password, 2 is the role.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to modify user's data, whose name does not exist in system files.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// This method validates the new username, overwrites it in the file by calling OverwriteUserData method and adds the appropriate log.
        /// </summary>
        /// <param name="username">String representing the name of the user to be changed.</param>
        /// <param name="newUsername">String that represents the new username. It can only contain letters of the Polish alphabet, numbers and the characters '_' and '.'.</param>
        /// <exception cref="ArgumentException">This exception is thrown when the new username does not consist only of letters of the Polish alphabet, numbers and the characters '_' and '.'.</exception>
        public static void ChangeUsername(string username, string newUsername)
        {
            if (string.IsNullOrEmpty(newUsername))
            {
                throw new ArgumentException(null, "The new username must not be empty! ");
            }
            else if (!Regex.IsMatch(newUsername, @"^[a-zA-Z0-9_.ąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                throw new ArgumentException("The new username can only contain letters, numbers, characters '_' and '.'! ");
            }
            OverwriteUserData(username, newUsername, 0);
            _userDataChanged?.Invoke($"changed user's name to {newUsername}", username);
        }

        /// <summary>
        /// This method hashes user's password, overwrites it in the file by calling OverwriteUserData method and adds the appropriate log.
        /// </summary>
        /// <param name="username">String representing the name of the user to be changed.</param>
        /// <param name="password">String representing the user's new password.</param>
        public static void ChangeUserPassword(string username, string password)
        {
            OverwriteUserData(username, HashPassword(password), 1);
            _userDataChanged?.Invoke("changed user's password", username);
        }

        /// <summary>
        /// This method overwrites user's role in the file by calling OverwriteUserData method and adds the appropriate log.
        /// </summary>
        /// <param name="username">String representing the name of the user to be changed.</param>
        /// <param name="role">String representing the user's new role.</param>
        public static void ChangeUserRole(string username, Role role)
        {
            OverwriteUserData(username, role.ToString(), 2);
            _userDataChanged?.Invoke("changed user's role", username);
        }

        /// <summary>
        /// This method allows you to get User objects of all users in the system.
        /// </summary>
        /// <returns>The list of all users in the system.</returns>
        /// <exception cref="FormatException">This exception is thrown when an invalid user role is encountered in the file. This is only added for invalid attempts to modify the file.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// It checks the correctness of the amount of data in the user data file and the number of users in the file to determine whether the current user should be the first administrator.
        /// </summary>
        /// <returns>True if there are any users in the file and false if no users have been added to the system yet.</returns>
        /// <exception cref="FormatException">This exception is thrown when there is a user with an invalid amount of data in the user data file. This can only happen if there are incorrect manual changes to the file.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// Checks if the given password is correct for the user and returns the user's role if it is.
        /// </summary>
        /// <param name="username">String that represents the name of the user being checked.</param>
        /// <param name="password">String representing the password to check.</param>
        /// <returns>The role of a given user if the password is correct for him or null if it is incorrect.</returns>
        /// <exception cref="FormatException">This exception is thrown when an invalid user role is encountered in the file. This is only added for invalid attempts to modify the file.</exception>
        /// <exception cref="FileNotFoundException">This exception is thrown when the method can't find file with users. This is done just in case someone deletes this file while the system is running.</exception>
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

        /// <summary>
        /// This method hashes the string using sha256, which allows to store user data securely.
        /// </summary>
        /// <param name="password">String representing the password to be hashed.</param>
        /// <returns>String that contains the hashed password.</returns>
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
