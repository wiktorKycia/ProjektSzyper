using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.UsersManagement.Modules
{
    public class User
    {
        private string _username;
        public Role Role { get; set; }

        public User()
        {
            Role = Role.Administrator;
            _username = "defaultUser";
        }

        public User(string username, Role role)
        {
            Role = role;
            _username = "defaultUser";
            Username = username;
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
                else if(!Regex.IsMatch(value, @"^[a-zA-Z0-9_.ąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
                {
                    throw new ArgumentException("The username can only contain letters, numbers, characters '_' and '.'! ");
                }
                else
                {
                    _username = value;
                }
            }
        }
    }
}
