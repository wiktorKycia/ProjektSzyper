using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.Tests.Abstractions;
using StorageOffice.classes.Tests.Services;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;
using static System.Net.Mime.MediaTypeNames;

namespace StorageOffice.classes.TestsServices.Modules
{
    internal class PasswordManagerTests : TestsBase
    {
        
        private const string _passwordFilePath = "../../../Data/users.txt";

        public PasswordManagerTests()
        {
            _tests += SaveNewUser_AddsCorrectUserToFile;
            _tests += SaveNewUser_WhenUserAlreadyExists_ShouldThrowException;
            _tests += DeleteUser_RemovesCorrectUser;
            _tests += DeleteUser_WhenUserDoesNotExist_ShouldThrowException;
            _tests += OverrideData_WhenIncorrectUser_ShouldThrowException;
            _tests += ChangeUsername_ModifiesUsernameCorrectly;
            _tests += ChangePassword_ModifiesPasswordCorrectly;
            _tests += ChangeUserRole_ModifiesRoleCorrectly;
            _tests += GetAllUsers_ReturnsCorrectUsers;
            _tests += GetAllUsers_WhenRoleIsIncorrect_ShouldThrowException;
            _tests += CheckFile_WhenNoUsersCreated_ShouldReturnFalse;
            _tests += CheckFile_WhenMoreThan0UsersCreated_ShouldReturnTrue;
            _tests += CheckFile_WhenDataAmountIsIncorrect_ShouldThrowException;
            _tests += VerifyPasswordAndGetRole_WhenDataIsCorrect_ShouldReturnCorrectRole;
            _tests += VerifyPasswordAndGetRole_WhenDataIsIncorrect_ShouldReturnNull;
            _tests += VerifyPasswordAndGetRole_WhenRoleIsIncorrect_ShouldThrowException;
        }

        public void SaveNewUser_AddsCorrectUserToFile()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                string testUsername = "Admin";
                File.WriteAllText(_passwordFilePath, "");

                PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);

                List<string> lines = File.ReadAllLines(_passwordFilePath).ToList();
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
                if (lines[lines.Count - 1] != $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator")
                {
                    throw new Exception($"Save new user test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' as the last added user");
                }
            });
        }

        public void SaveNewUser_WhenUserAlreadyExists_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    string testUsername = "Admin";
                    File.WriteAllText(_passwordFilePath, "");
                    File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                    PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);
                    throw new Exception("Save new user test failed: expected InvalidOperationException, but none was thrown");
                }
                catch (InvalidOperationException)
                {
                    //Success
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
                if (lines.Any(line => line == $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"))
                {
                    throw new Exception($"Delete user test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' to not be inside users file");
                }
                else if (numberOfLines != lines.Count + 1)
                {
                    throw new Exception($"Delete user test failed: expected {numberOfLines - 1} lines in file, got {lines.Count}");
                }
            });
        }

        public void DeleteUser_WhenUserDoesNotExist_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    File.WriteAllText(_passwordFilePath, "");
                    PasswordManager.DeleteUser("Admin");

                    throw new Exception("Delete user test failed: expected InvalidOperationException, but none was thrown");
                }
                catch (InvalidOperationException)
                {
                    //Success
                }
            });
        }

        public void OverrideData_WhenIncorrectUser_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    string testUsername = "Admin";
                    File.WriteAllText(_passwordFilePath, "");
                    File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                    PasswordManager.OverwriteUserData("George", "Thomas", 0);
                    throw new Exception("Override data test failed: expected InvalidOperationException, but none was thrown");
                }
                catch (InvalidOperationException)
                {
                    //Success
                }
            });
        }

        public void ChangeUsername_ModifiesUsernameCorrectly()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                string testUsername = "Admin";
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                PasswordManager.ChangeUsername(testUsername, "Admin1");

                if (File.ReadAllLines(_passwordFilePath)[0] != "Admin1,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator")
                {
                    throw new Exception("Change username test failed: expected 'Admin1,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator' to be inside users file");
                }
            });
        }

        public void ChangePassword_ModifiesPasswordCorrectly()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                string testUsername = "Admin";
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                PasswordManager.ChangeUserPassword(testUsername, "xyz2");

                if (File.ReadAllLines(_passwordFilePath)[0] != $"{testUsername},y9MASH6J3z3QKtgfY4r969N5KZfmkwdsoelYcftQCp8=,Administrator")
                {
                    throw new Exception($"Change username test failed: expected '{testUsername},y9MASH6J3z3QKtgfY4r969N5KZfmkwdsoelYcftQCp8=,Administrator' to be inside users file");
                }
            });
        }

        public void ChangeUserRole_ModifiesRoleCorrectly()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                string testUsername = "User";
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                PasswordManager.ChangeUserRole(testUsername, Role.Logistician);

                if (File.ReadAllLines(_passwordFilePath)[0] != $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician")
                {
                    throw new Exception($"Change username test failed: expected '{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician' to be inside users file");
                }
            });
        }

        public void GetAllUsers_ReturnsCorrectUsers()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
                File.AppendAllText(_passwordFilePath, "User,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");
                File.AppendAllText(_passwordFilePath, "Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman\n");
                File.AppendAllText(_passwordFilePath, "George,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,WarehouseManager\n");

                List<User> users = PasswordManager.GetAllUsers();

                if (users[0].Username != "Admin" || users[0].Role != Role.Administrator)
                {
                    throw new Exception($"Get all users test failed: expected username - Admin, role - Administrator, got username - {users[0].Username}, role - {users[0].Role}");
                }
                if (users[1].Username != "User" || users[1].Role != Role.Logistician)
                {
                    throw new Exception($"Get all users test failed: expected username - User, role - Logistician, got username - {users[1].Username}, role - {users[1].Role}");
                }
                if (users[2].Username != "Thomas" || users[2].Role != Role.Warehouseman)
                {
                    throw new Exception($"Get all users test failed: expected username - Thomas, role - Warehouseman, got username - {users[2].Username}, role - {users[2].Role}");
                }
                if (users[3].Username != "George" || users[3].Role != Role.WarehouseManager)
                {
                    throw new Exception($"Get all users test failed: expected username - George, role - WarehouseManager, got username - {users[3].Username}, role - {users[3].Role}");
                }
            });
        }

        public void GetAllUsers_WhenRoleIsIncorrect_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    File.WriteAllText(_passwordFilePath, "");
                    File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n");

                    List<User> users = PasswordManager.GetAllUsers();
                    throw new Exception("Get all users test failed: expected FormatException, but none was thrown");
                }
                catch (FormatException)
                {
                    //Success
                }
            });
        }

        public void CheckFile_WhenNoUsersCreated_ShouldReturnFalse()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                File.WriteAllText(_passwordFilePath, "");

                bool fileCheckResult = PasswordManager.CheckFile();

                if (fileCheckResult)
                {
                    throw new Exception("Check file test failed: expected false, got true");
                }
            });
        }

        public void CheckFile_WhenMoreThan0UsersCreated_ShouldReturnTrue()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
                File.AppendAllText(_passwordFilePath, "User,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");

                bool fileCheckResult = PasswordManager.CheckFile();

                if (!fileCheckResult)
                {
                    throw new Exception("Check file test failed: expected true, got false");
                }
            });
        }

        public void CheckFile_WhenDataAmountIsIncorrect_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    File.WriteAllText(_passwordFilePath, "");
                    File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=\n"); ;

                    bool fileCheckResult = PasswordManager.CheckFile();
                    throw new Exception("Check file test failed: expected FormatException, but none was thrown");
                }
                catch (FormatException)
                {
                    //Success
                }
            });
        }

        public void VerifyPasswordAndGetRole_WhenDataIsCorrect_ShouldReturnCorrectRole()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz");

                if(role != Role.Administrator)
                {
                    throw new Exception($"Verify password test failed: expected role - Administrator, got role - {role.ToString() ?? "null"}");
                }
            });
        }

        public void VerifyPasswordAndGetRole_WhenDataIsIncorrect_ShouldReturnNull()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                File.WriteAllText(_passwordFilePath, "");
                File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

                Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz2");

                if (role != null)
                {
                    throw new Exception($"Verify password test failed: expected role - null, got role - {role}");
                }
            });
        }

        public void VerifyPasswordAndGetRole_WhenRoleIsIncorrect_ShouldThrowException()
        {
            TestUtils.WithTxtFileBackup(_passwordFilePath, () =>
            {
                try
                {
                    File.WriteAllText(_passwordFilePath, "");
                    File.AppendAllText(_passwordFilePath, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n"); ;

                    Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz");
                    throw new Exception("Verify password test failed: expected FormatException, but none was thrown");
                }
                catch (FormatException)
                {
                    //Success
                }
            });
        }

        public override bool RunAllTests()
        {
            if (_tests != null)
            {
                if (!File.Exists(_passwordFilePath))
                {
                    File.Create(_passwordFilePath).Dispose();
                }

                LogManager.AddNewLog("Info: system service tests started");
                var originalOutput = Console.Out;
                Console.SetOut(TextWriter.Null);
                string results = "";
                bool everyTestPassed = true;
                foreach (var test in _tests.GetInvocationList())
                {
                    try
                    {
                        test.DynamicInvoke();
                    }
                    catch (Exception ex)
                    {
                        everyTestPassed = false;
                        results += "- " + ex.InnerException!.Message + "\n";
                        LogManager.AddNewLog($"Error: {ex.InnerException!.Message}");
                    }
                }
                LogManager.AddNewLog("Info: system service tests ended");
                Console.SetOut(originalOutput);
                Console.WriteLine(results);
                return everyTestPassed;
            }
            return false;
        }
    }
}
