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
    internal class PasswordManagementTests
    {
        [Test, IsolatedFileAttribiute]
        public void SaveNewUser_AddsCorrectUserToFile()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;

            PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            List<string> lines = File.ReadAllLines(path!).ToList();
            Assert.That(lines[lines.Count - 1], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        [Test, IsolatedFileAttribiute]
        public void DeleteUser_RemovesCorrectUser()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
            File.AppendAllText(path!, "George,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");
            File.AppendAllText(path!, "Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman\n");

            PasswordManager.DeleteUser("George");

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            List<string> lines = File.ReadAllLines(path!).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(lines[0], Is.EqualTo("Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
                Assert.That(lines[1], Is.EqualTo("Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman"));
                Assert.That(lines.Count, Is.EqualTo(2));
            };
        }

        [Test, IsolatedFileAttribiute]
        public void ChangeUsername_ModifiesUsernameCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUsername(testUsername, "Admin1");

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo("Admin1,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        [Test, IsolatedFileAttribiute]
        public void ChangePassword_ModifiesPasswordCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserPassword(testUsername, "xyz2");

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},y9MASH6J3z3QKtgfY4r969N5KZfmkwdsoelYcftQCp8=,Administrator"));
        }

        [Test, IsolatedFileAttribiute]
        public void ChangeUserRole_ModifiesRoleCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            string testUsername = "Thomas";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserRole(testUsername, Role.Logistician);

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician"));
        }

        [Test, IsolatedFileAttribiute]
        public void GetAllUsers_ReturnsCorrectUsers()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");
            File.AppendAllText(path!, "User,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician\n");
            File.AppendAllText(path!, "Thomas,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Warehouseman\n");
            File.AppendAllText(path!, "George,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,WarehouseManager\n");

            List<User> users = PasswordManager.GetAllUsers();

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
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

        [Test, IsolatedFileAttribiute]
        public void CheckFile_WhenNoUsersCreated_ShouldReturnFalse()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;

            bool fileCheckResult = PasswordManager.CheckFile();

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(fileCheckResult, Is.False);
        }

        [Test, IsolatedFileAttribiute]
        public void CheckFile_WhenMoreThan0UsersCreated_ShouldReturnTrue()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            bool fileCheckResult = PasswordManager.CheckFile();

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(fileCheckResult, Is.True);
        }

        [Test, IsolatedFileAttribiute]
        public void VerifyPasswordAndGetRole_WhenDataIsCorrect_ShouldReturnCorrectRole()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz");

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(role, Is.EqualTo(Role.Administrator));
        }

        [Test, IsolatedFileAttribiute]
        public void VerifyPasswordAndGetRole_WhenDataIsIncorrect_ShouldReturnNull()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string originalPasswordFilePath = PasswordManager.PasswordFilePath;
            PasswordManager.PasswordFilePath = path!;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz2");

            PasswordManager.PasswordFilePath = originalPasswordFilePath;
            Assert.That(role, Is.Null);
        }
    }
}
