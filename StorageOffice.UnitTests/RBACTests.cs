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
    internal class RBACTests
    {
        [Test]
        public void HasPermission_WhenUserHasPermission_ShouldReturnTrue()
        {
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            bool hasPermission = rbac.HasPermission(user, Permission.ViewLogs);

            Assert.That(hasPermission, Is.True);
        }

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
