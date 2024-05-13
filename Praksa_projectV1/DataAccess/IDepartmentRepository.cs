using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface IDepartmentRepository
    {
        Task<bool> AddAsync(Department newDepartment);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<bool> RemoveAsync(Department department);
        Task<bool> UpdateAsync(Department department);
    }
}
