using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.UsersManagement.Services
{
    public enum Role
    {
        Administrator = 1,
        Warehouseman,
        Logistician,
        WarehouseManager
    }

    enum Permission
    {
        BrowseWarehouse,
        AssignTask,
        DoTasks,
        ManageShipments,
        BrowseShipments,
        ManageUsers,
        ViewLogs
    }

    internal class RBAC
    {
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;


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
