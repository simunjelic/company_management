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
        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                using (_context = new Context())
                {
                    _context.Projects.Update(project);
                    await _context.SaveChangesAsync();
                    MessageBox.Show("Projekt je uspješno ažuriran.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Pogreška pri ažuriranju projekta {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    var recordToDelete = await context.EmployeeProjects.FirstOrDefaultAsync(i => i.ProjectId == member.ProjectId && i.EmployeeId == member.EmployeeId);
                    if (recordToDelete != null)
                    {
                        context.EmployeeProjects.Remove(recordToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri brisanju zaposlenika sa projekta.");
                return false;
            }
        }

        internal async Task<bool> AddMemberToProject(EmployeeProject teamMember)
        {
            try
            {
                using (var context = new Context())
                {
                    var check = context.EmployeeProjects.FirstOrDefault(i => i.ProjectId == teamMember.ProjectId && i.EmployeeId == teamMember.EmployeeId);
                    if (check == null)
                    {
                        await context.EmployeeProjects.AddAsync(teamMember);
                        context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Zaposlenik već dodan na projekt.");
                        return false;
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri unosu novog člana u bazu.");
                return false;
            }
        }
    }

}
