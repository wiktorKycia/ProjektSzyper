using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.UsersManagement.Services
{
    /// <summary>
    /// Roles that users must have.
    /// </summary>
    public enum Role
    {
        Administrator = 1,
        Warehouseman,
        Logistician,
        WarehouseManager
    }

    /// <summary>
    /// Permissions that allow users to have access to relevant parts of the system.
    /// </summary>
    public enum Permission
    {
        BrowseWarehouse,
        AssignTask,
        DoTasks,
        ManageShipments,
        BrowseShipments,
        ManageUsers,
        ViewLogs
    }

    /// <summary>
    /// Represents the RBAC system that allows to check whether the user has permissions to specific system functionalities.
    /// </summary>
    public class RBAC
    {
        /// <summary>
        /// A dictionary containing the roles and permissions that a user with the given roles has.
        /// </summary>
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RBAC"/> class and populates the dictionary with appropriate roles and permissions.
        /// </summary>
        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<Permission>>
            {
                { Role.Administrator, new List<Permission>{ Permission.ManageUsers, Permission.ViewLogs }},
                { Role.WarehouseManager, new List<Permission>{ Permission.BrowseWarehouse, Permission.AssignTask, Permission.BrowseShipments }},
                { Role.Logistician, new List<Permission>{ Permission.ManageShipments } },
                { Role.Warehouseman, new List<Permission>{ Permission.BrowseWarehouse, Permission.DoTasks, Permission.BrowseShipments } }
            };
        }

        /// <summary>
        /// Checks whether a user has a particular permissions based on his or her role.
        /// </summary>
        /// <param name="user">The user whose role is being checked.</param>
        /// <param name="permission">Permission type object, which will be checked to see if the user has it</param>
        /// <returns>True if the user has the given permission or false if he/she does not have it.</returns>
        public bool HasPermission(User user, Permission permission)
        {
            if (_rolePermissions.ContainsKey(user.Role) && _rolePermissions[user.Role].Contains(permission))
            {
                return true;
            }
            return false;
        }
    }
}
