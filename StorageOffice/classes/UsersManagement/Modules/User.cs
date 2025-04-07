using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.UsersManagement.Modules
{
    internal class User
    {
        public string Username { get; set; }
        public List<Role> Roles { get; set; }
        public User(string username)
        {
            Roles = new List<Role>();
            Username = username;
        }

        public void AddRole(Role role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
            }
        }

        public void RemoveRole(Role role)
        {
            if (Roles.Contains(role))
            {
                Roles.Remove(role);
            }
        }
    }
}
