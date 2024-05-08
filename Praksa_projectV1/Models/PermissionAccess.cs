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
        private static List<Permission> _deletePermission = new();

        public static List<Permission> DeletePermission
        {
            get { return _deletePermission; }
            set { _deletePermission = value; }
        }
        private static List<Permission> _readPermission = new();

        public static List<Permission> ReadPermission
        {
            get { return _readPermission; }
            set { _readPermission = value; }
        }

        private static List<Permission> _updatePermission = new();

        public static List<Permission> UpdatePermission
        {
            get { return _updatePermission; }
            set { _updatePermission = value; }
        }
    }
}
