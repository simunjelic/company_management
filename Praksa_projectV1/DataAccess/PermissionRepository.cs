﻿using Microsoft.EntityFrameworkCore;
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
    public class PermissonRepository
    {
        internal async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            try { 
            using (var _context = new Context())
            {
                return await  _context.Permissions
                    .Include(e => e.Module)
                    .Include(e => e.Role)
                    .ToListAsync();
            }
            } 
            catch {
                return null;
            }
        }

        internal async Task<IEnumerable<Role>> GetAllRoles()
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
                    return await context.Modules.ToListAsync();
                }

            }
            catch
            {
                return null;
            }

        }

        internal List<Permission> getPermissionByModuleRead(string modul, string action)
        {
            try
            {
                using (var context = new Context())
                {
                    return context.Permissions
                        .Include(e => e.Role)
                        .Include(e => e.Module)
                        .Where(i => i.Module.Name == modul &&  i.Action== action)
                        .ToList();
                }

            }
            catch
            {
                return null;
            }
        }

        internal bool RemoveById(int id)
        {
            try
            {
                using (var context = new Context())
                {
                    var check = context.Permissions.FirstOrDefault(i => i.Id == id);

                    if(check != null) { 
                    context.Permissions.Remove(check);
                    context.SaveChanges();
                    return true;
                    }
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> Add(Permission permission)
        {
            try
            {
                using (var context = new Context())
                {
                    
                        await context.Permissions.AddAsync(permission);
                        await context.SaveChangesAsync();
                        return true;
                    

                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<IEnumerable<Permission>> FilterData(string searchQuery)
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Permissions
                              .Include(e => e.Role)
                             .Include(e => e.Module)
                              .Where(p => p.Module.Name.Contains(searchQuery) || p.Role.RoleName.Contains(searchQuery) || p.Action.Contains(searchQuery))
                              .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal async Task<bool> RemoveRole(Role SelectedItem)
        {
            try
            {
                using (var context = new Context())
                {
                    
                        context.Roles.Remove(SelectedItem);
                        context.SaveChanges();
                        return true;
                   
                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> UpdateRole(Role SelectedItem)
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

        internal async Task<bool> AddRole(Role SelectedItem)
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
    }
}