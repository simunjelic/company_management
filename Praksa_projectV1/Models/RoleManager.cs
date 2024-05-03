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

        private static string? _username;

        public static string Username
        {
            get { return _username; }
            set { _username = value; }
        }
    }
}
