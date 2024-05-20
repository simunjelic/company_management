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
    public class ProjectRepository : IProjectRepository
    {
        Context _context = null;



        public async Task<bool> AddAsync(Project newProject)
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
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<bool> AddMemberToProject(EmployeeProject teamMember)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeFromProjectAsync(EmployeeProject member)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Project selectedItem)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<IEnumerable<Project>> FilterData(string searchQuery)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
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
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return Enumerable.Empty<Project>();
            }
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            try
            {
                using (_context = new Context())
                {
                    return await _context.Locations.ToListAsync();
                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return Enumerable.Empty<Location>(); 
            }
        }

        public async Task<IEnumerable<Models.Type>> GetAllTypesAsync()
        {
            try
            {
                using (var _context = new Context())
                {
                    return await _context.Types.ToListAsync();
                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return Enumerable.Empty<Models.Type>();
            }
        }

        public async Task<IEnumerable<EmployeeProject>> GetTeam(int id)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        public async Task<bool> UpdateMemberToProject(EmployeeProject projectHasManager)
        {
            try
            {
                using (var context = new Context())
                {

                    context.EmployeeProjects.Update(projectHasManager);
                    var RowsAffected = await context.SaveChangesAsync();
                    return RowsAffected > 0;

                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }
    }

}
