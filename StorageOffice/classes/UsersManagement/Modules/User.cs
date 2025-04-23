using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.UsersManagement.Modules
{
    /// <summary>
    /// A class representing the user and their data for the purpose and runtime of the system.
    /// </summary>
    public class User
    {
        private string _username;
        public Role Role { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class. Sets the default role to administrator and the username to defaultUser, which is useful when there is no user created on the system yet.
        /// </summary>
        public User()
        {
            Role = Role.Administrator;
            _username = "defaultUser";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="username">The username that the given user has.</param>
        /// <param name="role">The role that a user has in the system.</param>
        public User(string username, Role role)
        {
            Role = role;
            _username = "defaultUser";
            Username = username;
        }

        /// <summary>
        /// Allows you to get a username, as well as set it. Checks that when setting, the new username is not null, empty or consists only of letters of the Polish alphabet, numbers and the characters '_' and '.'.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(null, "The username must not be empty! ");
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

        /// <summary>
        /// Allows you to get user information in a simple and accessible form to display.
        /// </summary>
        /// <returns>A string, which contains information about user data.</returns>
        public override string ToString()
        {
            return $"Username: {Username}, Role: {Role}";
        }
    }
}
