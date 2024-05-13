using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Enums;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Praksa_projectV1.DataAccess
{
    public class PermissonRepository : IPermissonRepository
    {
        
         public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            try
            {
                using (var _context = new Context())
                {
                    var permissions = await _context.Permissions
                        .Include(e => e.Module)
                        .Include(e => e.Role)
                        .ToListAsync();

                    // Map enum values to their corresponding names
                    foreach (var permission in permissions)
                    {
                        if (permission.ActionId != null)
                            permission.Action = ((AvailableActions)permission.ActionId).ToString();
                    }

                    return permissions;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Roles.ToListAsync();
                }

            }
            catch
            {
                return null;
            }

        }
        internal async Task<IEnumerable<Module>> GetAllModules()
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Modules.OrderBy(m => m.Id).ToListAsync();
                }

            }
            catch
            {
                return null;
            }

        }

        

        

        internal async Task<bool> AddAsync(Permission permission)
        {
            try
            {
                using (var context = new Context())
                {

                    await context.Permissions.AddAsync(permission);
                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;


                }

            }
            catch
            {
                return false;
            }
        }

       

        internal async Task<bool> RemoveRoleAsync(Role SelectedItem)
        {
            try
            {
                using (var context = new Context())
                {

                    context.Roles.Remove(SelectedItem);
                    var RowsAffected = await context.SaveChangesAsync();
                    return RowsAffected > 0;

                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> UpdateRoleAsync(Role SelectedItem)
        {
            try
            {
                using (var context = new Context())
                {

                    context.Roles.Attach(SelectedItem);
                    context.Entry(SelectedItem).State = EntityState.Modified;
                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;

                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> AddRoleAsync(Role SelectedItem)
        {
            try
            {
                using (var context = new Context())
                {


                    context.Roles.Add(SelectedItem);


                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;

                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> RemoveAsync(Permission selectedItem)
        {
            try
            {
                using (var context = new Context())
                {
                    
                        context.Permissions.Remove(selectedItem);
                        var RowsAffected = await context.SaveChangesAsync();
                        return RowsAffected > 0;
                    
                }

            }
            catch
            {
                return false;
            }
        }

        public List<Permission> GetUserRoles(int action, List<int> roles)
        {
            try
            {
                using (var context = new Context())
                {
                    return context.Permissions
                        .Include(e => e.Role)
                        .Include(e => e.Module)
                        .Where(i => i.ActionId == action && roles.Contains((int)i.RoleId)).ToList();
                }

            }
            catch
            {
                return null;
            }
        }

        
    }
}
