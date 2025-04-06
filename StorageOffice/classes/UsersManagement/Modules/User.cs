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
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Role> Roles { get; set; }
        public User(string username, string name, string surname)
        {
            Roles = new List<Role>();
            Username = username;
            Name = name;
            Surname = surname;
        }

        public void AddRole(Role role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
            }
        }
    }
}
