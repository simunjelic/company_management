using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
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
                                   .FirstOrDefault(i => i.UserId == userId) ?? new Employee();

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

                    var employees = await dContext.Employees
                                          .Include(e => e.Job)
                                          .Include(e => e.Department)
                                          .OrderByDescending(e => e.IsActive)
                                          .ToListAsync();

                    foreach (var employee in employees)
                    {
                        employee.IsActiveText = employee.IsActive ? "Da" : "Ne";
                    }

                    return employees;

                }

            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Employee>();
            }
        }
        public async Task<IEnumerable<Employee>> GetAllActiveAsync()
        {
            try
            {
                using (dContext = new Context())
                {

                    var employees = await dContext.Employees
                                          .Include(e => e.Job)
                                          .Include(e => e.Department)
                                          .Where(e => e.IsActive == true)
                                          .ToListAsync();

                    foreach (var employee in employees)
                    {
                        employee.IsActiveText = employee.IsActive ? "Da" : "Ne";
                    }

                    return employees;

                }

            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Employee>();
            }
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            try
            {
                using (dContext = new Context())
                {

                    var employee = await dContext.Employees
                                          .Include(e => e.Job)
                                          .Include(e => e.Department)
                                          .FirstOrDefaultAsync(i => i.Id == id) ?? new Employee();

                        employee.IsActiveText = employee.IsActive ? "Da" : "Ne";

                    return employee;

                }

            }
            catch (Exception ex)
            {
                return new Employee();
            }
        }
    }
}
