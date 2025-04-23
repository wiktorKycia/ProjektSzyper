using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.UnitTests
{
    /// <summary>
    /// Contains unit tests that verify the functioning of methods from the RBAC class.
    /// </summary>
    internal class RBACTests
    {
        /// <summary>
        /// Checks that the HasPermission method returns true when called for a permissions that are allowed for a user's role.
        /// </summary>
        [Test]
        public void HasPermission_WhenUserHasPermission_ShouldReturnTrue()
        {
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            bool hasPermission = rbac.HasPermission(user, Permission.ViewLogs);

            Assert.That(hasPermission, Is.True);
        }

        /// <summary>
        /// Checks that the HasPermission method returns false when called for a permissions that aren't allowed for a user's role.
        /// </summary>
        [Test]
        public void HasPermission_WhenUserDoesNotHavePermission_ShouldReturnFalse()
        {
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            bool hasPermission = rbac.HasPermission(user, Permission.AssignTask);

            Assert.That(hasPermission, Is.False);
        }
    }
}
