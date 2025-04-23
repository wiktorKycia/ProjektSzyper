using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.IntegrationsTests
{
    internal class PasswordManagerTests
    {
        [Test, IsolatedFile]
        public void SaveNewUser_AddsCorrectUserToFile()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";

            PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);

            List<string> lines = File.ReadAllLines(path!).ToList();
            Assert.That(lines[lines.Count - 1], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        [Test, IsolatedFile]
        public void SaveNewUser_WhenUserAlreadyExists_ShouldThrowInvalidOperationException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Assert.Throws<InvalidOperationException>(() => PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator));
        }

        [Test, UseMissingFilePath]
        public void SaveNewUser_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.SaveNewUser("Thomas", "xyz", Role.Logistician));
        }

        [Test, IsolatedFile]
        public void DeleteUser_RemovesCorrectUser()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
            File.AppendAllText(path!, "George,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");
            File.AppendAllText(path!, "Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman\n");

            PasswordManager.DeleteUser("George");

            List<string> lines = File.ReadAllLines(path!).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(lines[0], Is.EqualTo("Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
                Assert.That(lines[1], Is.EqualTo("Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman"));
                Assert.That(lines.Count, Is.EqualTo(2));
            };
        }

        [Test, IsolatedFile]
        public void DeleteUser_WhenUserDoesNotExist_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => PasswordManager.DeleteUser("Admin"));
        }

        [Test, UseMissingFilePath]
        public void DeleteUser_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.DeleteUser("Thomas"));
        }

        [Test, IsolatedFile]
        public void OverwriteData_WhenIncorrectUser_ShouldThrowInvalidOperationException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Assert.Throws<InvalidOperationException>(() => PasswordManager.OverwriteUserData("George", "Thomas", 0));
        }

        [Test, UseMissingFilePath]
        public void OverwriteData_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.OverwriteUserData("Thomas", "George", 0));
        }

        [Test, IsolatedFile]
        public void ChangeUsername_ModifiesUsernameCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUsername(testUsername, "Admin1");

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo("Admin1,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        [Test, IsolatedFile]
        public void ChangePassword_ModifiesPasswordCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserPassword(testUsername, "xyz2");

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},y9MASH6J3z3QKtgfY4r969N5KZfmkwdsoelYcftQCp8=,Administrator"));
        }

        [Test, IsolatedFile]
        public void ChangeUserRole_ModifiesRoleCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Thomas";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserRole(testUsername, Role.Logistician);

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician"));
        }

        [Test, IsolatedFile]
        public void GetAllUsers_ReturnsCorrectUsers()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
            File.AppendAllText(path!, "User,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");
            File.AppendAllText(path!, "Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman\n");
            File.AppendAllText(path!, "George,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,WarehouseManager\n");

            List<User> users = PasswordManager.GetAllUsers();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(users[0].Username, Is.EqualTo("Admin"));
                Assert.That(users[0].Role, Is.EqualTo(Role.Administrator));
                Assert.That(users[1].Username, Is.EqualTo("User"));
                Assert.That(users[1].Role, Is.EqualTo(Role.Logistician));
                Assert.That(users[2].Username, Is.EqualTo("Thomas"));
                Assert.That(users[2].Role, Is.EqualTo(Role.Warehouseman));
                Assert.That(users[3].Username, Is.EqualTo("George"));
                Assert.That(users[3].Role, Is.EqualTo(Role.WarehouseManager));
            }
        }

        [Test, IsolatedFile]
        public void GetAllUsers_WhenRoleIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n");

            Assert.Throws<FormatException>(() => PasswordManager.GetAllUsers());
        }

        [Test, UseMissingFilePath]
        public void GetAllUsers_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.GetAllUsers());
        }

        [Test, IsolatedFile]
        public void CheckFile_WhenNoUsersCreated_ShouldReturnFalse()
        {
            bool fileCheckResult = PasswordManager.CheckFile();

            Assert.That(fileCheckResult, Is.False);
        }

        [Test, IsolatedFile]
        public void CheckFile_WhenMoreThan0UsersCreated_ShouldReturnTrue()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            bool fileCheckResult = PasswordManager.CheckFile();

            Assert.That(fileCheckResult, Is.True);
        }

        [Test, IsolatedFile]
        public void CheckFile_WhenDataAmountIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=\n");

            Assert.Throws<FormatException>(() => PasswordManager.CheckFile());
        }

        [Test, UseMissingFilePath]
        public void CkeckFile_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.CheckFile());
        }

        [Test, IsolatedFile]
        public void VerifyPasswordAndGetRole_WhenDataIsCorrect_ShouldReturnCorrectRole()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz");

            Assert.That(role, Is.EqualTo(Role.Administrator));
        }

        [Test, IsolatedFile]
        public void VerifyPasswordAndGetRole_WhenDataIsIncorrect_ShouldReturnNull()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz2");

            Assert.That(role, Is.Null);
        }

        [Test, IsolatedFile]
        public void VerifyPasswordAndGetRole_WhenRoleIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n"); ;

            Assert.Throws<FormatException>(() => PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz"));
        }

        [Test, UseMissingFilePath]
        public void VerifyPasswordAndGetRole_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.VerifyPasswordAndGetRole("Thomas", "xyz"));
        }
    }
}
