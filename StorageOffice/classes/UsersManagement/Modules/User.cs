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
        private string _username;
        public List<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(null, "The username must not be empty! ");
                }
                else
                {
                    _username = value;
                }
            }
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

        public override string ToString()
        {
            return $"{_username}: {string.Join(";", Roles)}";
        }
    }
}
