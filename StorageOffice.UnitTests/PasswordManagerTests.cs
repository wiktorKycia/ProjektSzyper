using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.UnitTests
{
    internal class PasswordManagerTests
    {
        [Test]
        public void ChangeUserName_WhenNewUsernameIsEmpty_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PasswordManager.ChangeUsername("Admin", ""));
        }

        [Test]
        public void ChangeUserName_WhenNewUsernameHasInvalidCharacters_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PasswordManager.ChangeUsername("Admin", "xyz, xyz"));
        }
    }
}
