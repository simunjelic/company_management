using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public class DepartmentRepository
    {
        private Context dContext = null;
        public DepartmentRepository()
        {
            dContext = new Context();
        }
        public List<Department> GetAllDepartments()
        {
            return dContext.Departments.ToList();
        }
        public Department GetDepart(int? id)
        {
            using (var dContext = new Context())
            {
                return dContext.Departments.FirstOrDefault(i => i.Id == id);
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            using (var dContext = new Context())
            {
                try
                {
                    var depToDelete = await dContext.Departments.FirstOrDefaultAsync(i => i.Id == id);
                    dContext.Remove(depToDelete);
                    int rowsAffected = await dContext.SaveChangesAsync();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    return false;

                }

            }
        }

        internal async Task<bool> AddAsync(Department newDepartment)
        {
            try
            {
                using (var dContext = new Context())
                {
                    
                        await dContext.AddAsync(newDepartment);
                        int rowsAffected = await dContext.SaveChangesAsync();
                        return rowsAffected > 0;
                    

                }
            }
            catch
            {
                return false;
            }
        }

        internal async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            try
            {
                using (var dContext = new Context())
                {
                    return await dContext.Departments.ToListAsync();
                }
                

            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Department>();
            }
        }

        internal async Task<bool> UpdateAsync(Department department)
        {
            try
            {
                using (var dContext = new Context())
                {
                    dContext.Departments.Update(department);
                    int rowsAffected = await dContext.SaveChangesAsync();
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
