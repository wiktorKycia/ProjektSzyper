using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.UnitTests
{
    /// <summary>
    /// Contains unit tests that verify the functioning and exceptions throwing of methods from the PasswordManager class.
    /// </summary>
    internal class PasswordManagerTests
    {
        /// <summary>
        /// Checks that the ChangeUsername method throws an ArgumentException when there is an attempt to change the username to an empty value.
        /// </summary>
        [Test]
        public void ChangeUserName_WhenNewUsernameIsEmpty_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PasswordManager.ChangeUsername("Admin", ""));
        }

        /// <summary>
        /// Checks that the ChangeUsername method throws an ArgumentException exception when there is an attempt to change the username to a value containing characters other than the letters of the Polish alphabet, the '_' character, and '.'.
        /// </summary>
        [Test]
        public void ChangeUserName_WhenNewUsernameHasInvalidCharacters_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PasswordManager.ChangeUsername("Admin", "xyz, xyz"));
        }
    }
}
