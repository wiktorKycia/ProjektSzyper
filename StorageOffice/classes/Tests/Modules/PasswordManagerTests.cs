using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.Tests.Services;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.TestsServices.Modules
{
    internal class PasswordManagerTests
    {
        private const string _passwordFilePath = "../../../Data/users.txt";

        public void SaveNewUser_AddsCorrectUserToFile()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                bool uniqueUsername = false;
                string testUsername = "Admin";
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
            });
        }

        public void DeleteUser_RemovesCorrectUser()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                bool uniqueUsername = false;
                string testUsername = "Admin";
                while (!uniqueUsername)
                {
                    if(File.ReadLines(_passwordFilePath).Any(line => line.Split(',')[0] == testUsername))
                    {
                        testUsername += "_";
                    }
                    else
                    {
                        File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
                        uniqueUsername = true;
                    }
                }

                int numberOfLines = File.ReadAllLines(_passwordFilePath).Length;
                PasswordManager.DeleteUser(testUsername);
                List<string> lines = File.ReadAllLines(_passwordFilePath).ToList();
                if (lines.Any(line => line == $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n"))
                {
                    throw new Exception($"Delete user test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' to not be inside users file");
                }
                else if (numberOfLines != lines.Count + 1)
                {
                    throw new Exception($"Delete user test failed: expected {numberOfLines - 1} lines in file, got {lines.Count}");
                }
            });
        }


    }
}
