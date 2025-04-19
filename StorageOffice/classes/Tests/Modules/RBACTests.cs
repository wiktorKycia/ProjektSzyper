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

        /// <summary>
        /// This test verifies that the HasPermission method of the RBAC class returns true if a valid permission is given for the user's role. If not, it throws an Exception
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void HasPermission_WhenUserHasPermission_ShouldReturnTrue()
        {
            /// Arrange: RBAC system and User objects are prepared for later use
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            /// Act
            bool hasPermission = rbac.HasPermission(user, Permission.ViewLogs);

            /// Assert: if the result of the method is not as expected(equal to false) an Exception is thrown
            if (!hasPermission)
            {
                throw new Exception("Has Permission test failed: excpected true, got false");
            }
        }

        /// <summary>
        /// This test verifies that the HasPermission method of the RBAC class returns false if an invalid permissions is given for a user's role. If not, it throws an Exception
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void HasPermission_WhenUserDoesNotHavePermission_ShouldReturnFalse()
        {
            /// Arrange: RBAC system and User objects are prepared for later use
            RBAC rbac = new RBAC();
            User user = new User("Thomas", Role.Administrator);

            /// Act
            bool hasPermission = rbac.HasPermission(user, Permission.AssignTask);

            /// Assert: if the result of the method is not as expected(equal to true) an Exception is thrown
            if (hasPermission)
            {
                throw new Exception("Has Permission test failed: excpected false, got true");
            }
        }
    }
}
