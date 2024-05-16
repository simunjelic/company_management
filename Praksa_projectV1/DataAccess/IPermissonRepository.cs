using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface  IPermissonRepository
    {
        List<Permission> GetUserRoles(int action, List<int> roles);
        Task<IEnumerable<Permission>> GetAllPermissions();
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Module>> GetAllModules();
        Task<bool> AddAsync(Permission permission);
        Task<bool> RemoveRoleAsync(Role SelectedItem);
        Task<bool> RemoveAsync(Permission selectedItem);
    }
}
