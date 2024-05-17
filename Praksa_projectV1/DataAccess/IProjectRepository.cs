using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface IProjectRepository
    {
        Task<bool> AddAsync(Project newProject);
        Task<bool> AddMemberToProject(EmployeeProject teamMember);
        Task<bool> DeleteEmployeeFromProjectAsync(EmployeeProject member);
        Task<bool> UpdateProjectAsync(Project project);
        Task<bool> DeleteAsync(Project selectedItem);
        Task<IEnumerable<Project>> FilterData(string searchQuery);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<IEnumerable<Models.Type>> GetAllTypesAsync();
        Task<IEnumerable<EmployeeProject>> GetTeam(int id);
        Task<bool> UpdateMemberToProject(EmployeeProject projectHasManager);
    }
}
