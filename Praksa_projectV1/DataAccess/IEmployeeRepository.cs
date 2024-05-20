using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface IEmployeeRepository
    {
        Task<bool> AddAsync(Employee newEmployee);
        Task<bool> UpdateAsync(Employee selectedItem);
        Task<bool> DeleteAsync(Employee selectedItem);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllActiveAsync();
    }
}
