using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.UsersManagement.Services
{
    internal class PasswordManager
    {
        private const string _passwordFilePath = "../../../Data/users.txt";
        public static event Action<string, bool> PasswordVerified;

        static PasswordManager()
        {
            if (!File.Exists(_passwordFilePath))
            {
                Console.WriteLine("The users file does not exist. The system creates a new one.");
                File.Create(_passwordFilePath).Dispose();
            }

            PasswordVerified += (username, success) => Console.WriteLine($"Login of user {username}: {(success ? "successful" : "unsuccessful")}");
        }

        public static void SaveNewUser(string username, string password, List<Role> roles)
        {
            if (File.ReadLines(_passwordFilePath).Any(line => line.Split(',')[0] == username))
            {
                Console.WriteLine($"User {username} already exists in the system");
                return;
            }

            string hashedPassword = HashPassword(password);
            /*Console.Write($"Podaj rolę nowego użytkownika({string.Join(", ", Enum.GetNames(typeof(Role)))}): ");
            Role role;
            while (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out role))
            {
                Console.WriteLine($"Podano niepoprawną rolę! Podaj poprawną({string.Join(", ", Enum.GetNames(typeof(Role)))}):");
            }*/
            File.AppendAllText(_passwordFilePath, $"{username},{hashedPassword},{string.Join(';', roles)}\n");
            Console.WriteLine($"User {username} has been saved");
        }

        public static void DeleteUser(string username)
        {
            if(!File.ReadLines(_passwordFilePath).Any(line => line.Split(',')[0] == username))
            {
                Console.WriteLine($"User {username} does not exist in the system");
                return;
            }

            List<string> fileLines = File.ReadAllLines(_passwordFilePath).ToList();
            int userIndex = fileLines.FindIndex(line => line.Split(',')[0] == username);
            fileLines.RemoveAt(userIndex);
            File.WriteAllLines(_passwordFilePath, fileLines.ToArray());
            Console.WriteLine($"User {username} has been deleted");
        }

        public static bool CheckFile()
        {
            List<string> lines = File.ReadAllLines(_passwordFilePath).ToList();
            if (lines.Count > 0)
            {
                foreach (string line in lines)
                {
                    if(line.Split(',').Length < 3)
                    {
                        throw new FormatException("Error: An anomaly was detected in the system data!");
                    }
                }
                return true;
            }
            return false;
        }

        public static List<Role> VerifyPasswordAndGetRoles(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            foreach (var line in File.ReadLines(_passwordFilePath))
            {
                var parts = line.Split(',');
                if (parts[0] == username && parts[1] == hashedPassword)
                {
                    List<Role> userRoles = new List<Role>();
                    PasswordVerified?.Invoke(username, true);
                    foreach(string role in parts[2].Split(';').ToList())
                    {
                        if(Enum.TryParse(role, true, out Role userRole))
                        {
                            userRoles.Add(userRole);
                        }
                    }
                    return userRoles;
                }
            }
            PasswordVerified?.Invoke(username, false);
            return new List<Role>();
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
