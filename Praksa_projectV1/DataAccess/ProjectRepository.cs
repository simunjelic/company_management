using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    internal class ProjectRepository
    {
        Context _context = null;
        public static IEnumerable<Project> GetAll()
        {
            using (var _context = new Context())
            {
                return _context.Projects
                    .Include(e => e.Type)
                    .Include(e => e.Location)
                    .ToList();
            }

        }

        internal void Add(Project newProject)
        {
            using (var _context = new Context())
            {
                _context.Projects.Add(newProject);
                _context.SaveChanges();
            }
        }

        internal async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {

                using (var _context = new Context())
                {
                    var check = await _context.Projects.FirstOrDefaultAsync(i => i.Id == id);
                    if (check != null)
                    {
                        _context.Projects.Remove(check);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        internal IEnumerable<Location> GetAllLocations()
        {
            using (_context = new Context())
            {
                return _context.Locations.ToList();
            }
        }

        internal IEnumerable<Models.Type> GetAllTypes()
        {
            using (var _context = new Context())
            {
                return _context.Types.ToList();
            }
        }
        public async Task<bool> UpdateProjectAsync(Project project)
        {
            try
            {
                using (_context = new Context())
                {
                    _context.Projects.Update(project);
                    var RowsAffected = await _context.SaveChangesAsync();
                    return RowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Projects
                              .Include(p => p.Type)
                              .Include(p => p.Location)
                              .FirstOrDefaultAsync(p => p.Id == projectId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju projekta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; // Or throw the exception if you want to handle it differently
            }
        }

        internal async Task<IEnumerable<Project>> FilterData(string searchQuery)
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Projects
                              .Include(p => p.Type)
                              .Include(p => p.Location)
                              .Where(p => p.Name.Contains(searchQuery) || p.Id.ToString().Contains(searchQuery))
                              .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal async Task<IEnumerable<EmployeeProject>> GetTeam(int id)
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.EmployeeProjects
                              .Include(p => p.Project)
                              .Include(p => p.Employee)
                              .Where(p => p.ProjectId == id)
                              .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal async Task<bool> DeleteEmployeeFromProjectAsync(EmployeeProject member)
        {
            try
            {
                using (var context = new Context())
                {
                    context.EmployeeProjects.Remove(member);
                    var RowsAffected = await context.SaveChangesAsync();
                    return RowsAffected > 0;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        internal async Task<bool> AddMemberToProject(EmployeeProject teamMember)
        {
            try
            {
                using (var context = new Context())
                {

                    await context.EmployeeProjects.AddAsync(teamMember);
                    var RowsAffected = await context.SaveChangesAsync();
                    return RowsAffected > 0;

                }

            }
            catch (Exception ex)
            {

                return false;
            }
        }

        internal async Task<IEnumerable<Project>> GetAllAsync()
        {
            try
            {
                using (var _context = new Context())
                {
                    return await _context.Projects
                        .Include(e => e.Type)
                        .Include(e => e.Location)
                        .ToListAsync();
                }

            }
            catch
            {
                return Enumerable.Empty<Project>();
            }
        }

        internal async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            try
            {
                using (_context = new Context())
                {
                    return await _context.Locations.ToListAsync();
                }

            }
            catch
            { return Enumerable.Empty<Location>(); }
        }

        internal async Task<IEnumerable<Models.Type>> GetAllTypesAsync()
        {
            try
            {
                using (var _context = new Context())
                {
                    return await _context.Types.ToListAsync();
                }

            }
            catch
            {
                return Enumerable.Empty<Models.Type>();
            }
        }

        internal async Task<bool> DeleteAsync(Project selectedItem)
        {
            try
            {

                using (var _context = new Context())
                {

                    _context.Projects.Remove(selectedItem);
                    var RowsAffected = await _context.SaveChangesAsync();
                    return RowsAffected > 0;


                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        internal async Task<bool> AddAsync(Project newProject)
        {
            try
            {
                using (var _context = new Context())
                {
                    await _context.Projects.AddAsync(newProject);
                    var RowsAffected = await _context.SaveChangesAsync();
                    return RowsAffected > 0;
                }

            }
            catch
            {
                return false;
            }
        }
    }

}
