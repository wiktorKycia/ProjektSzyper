using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.UsersManagement.Services
{
    enum Role
    {
        Administrator,
        Warehouseman,
        Logistician,
        WarehouseManager
    }

    enum Permission
    {

    }

    internal class RBAC
    {
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;


        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<Permission>>
            {
                { Role.Administrator, new List<Permission>{ Permission.Read, Permission.Write, Permission.Delete, Permission.ManageUsers }},
                { Role.WarehouseManager, new List<Permission>{ Permission.Read, Permission.Write }},
                { Role.Logistician, new List<Permission>{ Permission.Read } },
                { Role.Warehouseman, new List<Permission>{} }
            };
        }

        public bool HasPermission(User user, Permission permission)
        {
            foreach (var role in user.Roles)
            {
                if (_rolePermissions.ContainsKey(role) && _rolePermissions[role].Contains(permission))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
