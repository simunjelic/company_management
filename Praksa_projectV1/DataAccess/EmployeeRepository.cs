using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private Context dContext = null;
        public List<Employee> GetAll()
        {
            using (dContext = new Context())
            {

                var emps = dContext.Employees
                                   .Include(e => e.Job)
                                   .Include(e => e.Department)
                                   .ToList();



                return emps;
            }
        }

        public bool Add(Employee newEmployee)
        {
            using (dContext = new Context())
            {
                var userCheck = dContext.Employees.FirstOrDefault(i => i.UserId == newEmployee.UserId);
                if (userCheck == null)
                {
                    dContext.Employees.Add(newEmployee);
                    dContext.SaveChanges();
                    return true;
                }
                else return false;


            }
        }
        internal void DeleteById(int id)
        {
            using (dContext = new Context())
            {
                var record = dContext.Employees.FirstOrDefault(i => i.Id == id);
                if (record != null)
                {
                    dContext.Employees.Remove(record);
                    dContext.SaveChanges();
                }
            }
        }
        internal bool Update(Employee employee)
        {
            using (dContext = new Context())
            {


                dContext.Employees.Update(employee);
                dContext.SaveChanges();
                return true;



            }
        }

        internal async Task<Employee> GetProjectByIdAsync(int id)
        {
            using (dContext = new Context())
            {
                return dContext.Employees
                     .Include(e => e.Job)
                     .Include(e => e.Department)
                     .FirstOrDefault(i => i.Id == id);
            }
        }

        

       

        public async Task<bool> AddAsync(Employee newEmployee)
        {
            try
            {
                using (dContext = new Context())
                {
                    await dContext.Employees.AddAsync(newEmployee);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Employee FindByUserId(int? userId)
        {
            using (dContext = new Context())
            {
                try
                {

                    return dContext.Employees
                                   .Include(e => e.Job)
                                   .Include(e => e.Department)
                                   .FirstOrDefault(i => i.UserId == userId);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("No user in database: " + ex);

                    return null;
                }
            }
        }

        public async Task<bool> UpdateAsync(Employee selectedItem)
        {
            try
            {
                using (dContext = new Context())
                {
                    dContext.Employees.Update(selectedItem);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }

            }
            catch
            {
                return false;
            }
        }

        public Employee GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Employee selectedItem)
        {
            try
            {
                using (dContext = new Context())
                {

                    dContext.Employees.Remove(selectedItem);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;

                }


            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                using (dContext = new Context())
                {

                    return await dContext.Employees
                                       .Include(e => e.Job)
                                       .Include(e => e.Department)
                                       .ToListAsync();

                }

            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Employee>();
            }
        }
    }
}
