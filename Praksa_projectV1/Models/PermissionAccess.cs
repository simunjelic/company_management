using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Models
{
    public static class PermissionAccess
    {
        private static List<Permission> _createPermission = new();

        public static List<Permission> CreatePermission
        {
            get { return _createPermission; }
            set { _createPermission = value; }
        }
    }
}
