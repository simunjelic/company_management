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
    public class DepartmentRepository : IDepartmentRepository
    {
        private Context dContext = null;
        public DepartmentRepository()
        {
            dContext = new Context();
        }

        public async Task<bool> AddAsync(Department newDepartment)
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
            catch (Exception ex) 
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return false;
            }
        }

        

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            try
            {
                using (var dContext = new Context())
                {
                    return await dContext.Departments.Include(e => e.ParentDepartment).ToListAsync();
                }


            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return Enumerable.Empty<Department>();
            }
        }

        public async Task<bool> RemoveAsync(Department department)
        {
            using (var dContext = new Context())
            {
                try
                {
                    dContext.Remove(department);
                    int rowsAffected = await dContext.SaveChangesAsync();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                    
                    return false;

                }

            }
        }

        public async Task<bool> UpdateAsync(Department department)
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
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return false;
            }
        }
    }
}
