using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.Tests.Abstractions;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Tests.Modules
{
    internal class RBACTests : TestsBase
    {
        public RBACTests()
        {
            _tests += HasPermission_WhenUserHasPermission_ShouldReturnTrue;
            _tests += HasPermission_WhenUserDoesNotHavePermission_ShouldReturnFalse;
        }
        public void HasPermission_WhenUserHasPermission_ShouldReturnTrue()
        {
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            bool hasPermission = rbac.HasPermission(user, Permission.ViewLogs);

            if (!hasPermission)
            {
                throw new Exception("Has Permission test failed: excpected true, got false");
            }
        }

        public void HasPermission_WhenUserDoesNotHavePermission_ShouldReturnFalse()
        {
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            bool hasPermission = rbac.HasPermission(user, Permission.AssignTask);

            if (hasPermission)
            {
                throw new Exception("Has Permission test failed: excpected false, got true");
            }
        }
    }
}
