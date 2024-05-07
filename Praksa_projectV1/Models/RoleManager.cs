using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Models
{
    public static class RoleManager
    {
        private static List<string> _roles = new List<string>();

        public static List<string> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }
        private static List<int> _rolesId = new List<int>();

        public static List<int> RolesId
        {
            get { return _rolesId; }
            set { _rolesId = value; }
        }

        private static string? _username;

        public static string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        private static int _id;

        public static int Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}
