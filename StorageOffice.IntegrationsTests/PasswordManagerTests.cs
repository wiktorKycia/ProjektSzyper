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
    /// <summary>
    /// Contains integration tests that verify the functioning and exceptions throwing of methods from the PasswordManager class.
    /// </summary>
    internal class PasswordManagerTests
    {
        /// <summary>
        /// Checks that the SaveNewUser method correctly adds user data to the file in the format: "username, hashed password, role".
        /// </summary>
        [Test, IsolatedUsersFile]
        public void SaveNewUser_AddsCorrectUserToFile()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";

            PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator);

            List<string> lines = File.ReadAllLines(path!).ToList();
            Assert.That(lines[lines.Count - 1], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        /// <summary>
        /// Checks that the SaveNewUser method correctly throws an InvalidOperationException exception when there is an attempt to add a user with a name that is already taken.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void SaveNewUser_WhenUserAlreadyExists_ShouldThrowInvalidOperationException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Assert.Throws<InvalidOperationException>(() => PasswordManager.SaveNewUser(testUsername, "xyz", Role.Administrator));
        }

        /// <summary>
        /// Checks that the SaveNewUser method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void SaveNewUser_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.SaveNewUser("Thomas", "xyz", Role.Logistician));
        }

        /// <summary>
        /// Checks that the DeleteUser method correctly removes user data from the file
        /// </summary>
        [Test, IsolatedUsersFile]
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

        /// <summary>
        /// Checks that the DeleteUser method correctly throws an InvalidOperationException exception when there is an attempt to remove a user with a name that doesn't exist in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void DeleteUser_WhenUserDoesNotExist_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => PasswordManager.DeleteUser("Admin"));
        }

        /// <summary>
        /// Checks that the DeleteUser method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void DeleteUser_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.DeleteUser("Thomas"));
        }

        /// <summary>
        /// Checks that the OverwriteData method correctly throws an InvalidOperationException exception when there is an attempt to modify a user with a name that doesn't exist in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void OverwriteData_WhenIncorrectUser_ShouldThrowInvalidOperationException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Assert.Throws<InvalidOperationException>(() => PasswordManager.OverwriteUserData("George", "Thomas", 0));
        }

        /// <summary>
        /// Checks that the OverwriteData method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void OverwriteData_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.OverwriteUserData("Thomas", "George", 0));
        }

        /// <summary>
        /// Checks that the ChangeUsername method(by using the OverwriteData method) correctly modifies user's name in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void ChangeUsername_ModifiesUsernameCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUsername(testUsername, "Admin1");

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo("Admin1,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator"));
        }

        /// <summary>
        /// Checks that the ChangePassword method(by using the OverwriteData method) correctly modifies user's password in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void ChangePassword_ModifiesPasswordCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Admin";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserPassword(testUsername, "xyz2");

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},y9MASH6J3z3QKtgfY4r969N5KZfmkwdsoelYcftQCp8=,Administrator"));
        }

        /// <summary>
        /// Checks that the ChangeUserRole method(by using the OverwriteData method) correctly modifies user's role in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void ChangeUserRole_ModifiesRoleCorrectly()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            string testUsername = "Thomas";
            File.AppendAllText(path!, $"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            PasswordManager.ChangeUserRole(testUsername, Role.Logistician);

            Assert.That(File.ReadAllLines(path!)[0], Is.EqualTo($"{testUsername},Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Logistician"));
        }

        /// <summary>
        /// Checks that the GetAllUSers method correctly returns a list of all users that the file contains.
        /// </summary>
        [Test, IsolatedUsersFile]
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

        /// <summary>
        /// Checks that the GetAllUsers method correctly throws a FormatException when any of the users in the file has an invalid role.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void GetAllUsers_WhenRoleIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n");

            Assert.Throws<FormatException>(() => PasswordManager.GetAllUsers());
        }

        /// <summary>
        /// Checks that the GetAllUsers method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void GetAllUsers_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.GetAllUsers());
        }

        /// <summary>
        /// Checks that the CheckFile method correctly returns false when no users are created in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void CheckFile_WhenNoUsersCreated_ShouldReturnFalse()
        {
            bool fileCheckResult = PasswordManager.CheckFile();

            Assert.That(fileCheckResult, Is.False);
        }

        /// <summary>
        /// Checks that the CheckFile method correctly returns true when any users are created in the file.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void CheckFile_WhenMoreThan0UsersCreated_ShouldReturnTrue()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            bool fileCheckResult = PasswordManager.CheckFile();

            Assert.That(fileCheckResult, Is.True);
        }

        /// <summary>
        /// Checks that the CheckFile method correctly throws a FormatException when the amount of data(should be: username, password, role) for one user is different than 3 separated values.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void CheckFile_WhenDataAmountIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=\n");

            Assert.Throws<FormatException>(() => PasswordManager.CheckFile());
        }

        /// <summary>
        /// Checks that the CheckFile method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void CkeckFile_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.CheckFile());
        }

        /// <summary>
        /// Checks that the VerifyPasswordAndGetRole method correctly returns the role of a given user when given valid login details.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void VerifyPasswordAndGetRole_WhenDataIsCorrect_ShouldReturnCorrectRole()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz");

            Assert.That(role, Is.EqualTo(Role.Administrator));
        }

        /// <summary>
        /// Checks that the VerifyPasswordAndGetRole method correctly returns null when given invalid login details.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void VerifyPasswordAndGetRole_WhenDataIsIncorrect_ShouldReturnNull()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Administrator\n");

            Role? role = PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz2");

            Assert.That(role, Is.Null);
        }

        /// <summary>
        /// Checks that the VerifyPasswordAndGetRole method correctly throws a FormatException when correctly verified user has incorrect role.
        /// </summary>
        [Test, IsolatedUsersFile]
        public void VerifyPasswordAndGetRole_WhenRoleIsIncorrect_ShouldThrowFormatException()
        {
            string? path = TestContext.CurrentContext.Test.Properties.Get("IsolatedFilePath") as string;
            File.AppendAllText(path!, "Admin,Ngi8oeROpsTSaOttsCJgJpiSwLQrhrvx53pvoWw8koI=,Mechanic\n"); ;

            Assert.Throws<FormatException>(() => PasswordManager.VerifyPasswordAndGetRole("Admin", "xyz"));
        }

        /// <summary>
        /// Checks that the VerifyPasswordAndGetRole method correctly throws a FileNotFoundException, in case the user data file is missing from the specified location.
        /// </summary>
        [Test, UseMissingUsersFilePath]
        public void VerifyPasswordAndGetRole_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => PasswordManager.VerifyPasswordAndGetRole("Thomas", "xyz"));
        }
    }
}
