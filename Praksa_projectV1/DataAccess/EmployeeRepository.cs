using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    public class EmployeeRepository
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

        internal Employee FindByUserId(int? userId)
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
        internal bool Update(Employee employee)
        {
            using (dContext = new Context())
            {
                
                
                    dContext.Employees.Update(employee);
                    dContext.SaveChanges();
                    return true;
                


            }
        }

        internal Employee GetById(int id)
        {
            using (dContext = new Context())
            {
               return dContext.Employees
                    .Include(e => e.Job)
                    .Include(e => e.Department)
                    .FirstOrDefault(i => i.Id == id);
            }

          }

        internal async Task<Employee> GetProjectByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
