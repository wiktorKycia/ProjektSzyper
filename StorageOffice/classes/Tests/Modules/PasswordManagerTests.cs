using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.TestsServices.Modules
{
    internal class PasswordManagerTests
    {
        private const string _passwordFilePath = "../../../Data/users.txt";

        public void SaveNewUser_AddsCorrectUserToFile()
        {
            bool uniqueUsername = false;
            string testUsername = "Admin1";
            while (!uniqueUsername)
            {
                try
                {
                    PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);
                    uniqueUsername = true;
                }
                catch (InvalidOperationException)
                {
                    testUsername += "_";
                }
            }

            List<string> lines = File.ReadAllLines(_passwordFilePath).ToList();
            if (lines[lines.Count - 1] != $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n")
            {
                throw new Exception($"Save new user test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' as the last added user");
            }
            else
            {
                lines.RemoveAt(lines.Count - 1);
                File.WriteAllLines(_passwordFilePath, lines);
            }
        }

        public void DeleteUser_RemovesCorrectUser()
        {
            bool uniqueUsername = false;
            string testUsername = "Admin1";
            while (!uniqueUsername)
            {
                try
                {
                    PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);
                    uniqueUsername = true;
                }
                catch (InvalidOperationException)
                {
                    testUsername += "_";
                }
            }

            PasswordManager.DeleteUser(testUsername);
            List<string> lines = File.ReadAllLines(_passwordFilePath).ToList();
            if (lines.Any(line => line == $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n"))
            {
                throw new Exception($"Delete user test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' to not be inside users file");
            }
        }


    }
}
