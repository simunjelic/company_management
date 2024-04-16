using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Remove(int id)
        {
            using(var dContext = new Context())
            {
                try { 
                var depToDelete = dContext.Departments.FirstOrDefault(i => i.Id == id);
                dContext.Remove(depToDelete);
                dContext.SaveChanges();
                    return true;
                }
                catch(Exception ex) {
                    return false;

                }

            }
        }

        internal bool Add(Department newDepartment)
        {
            using (var dContext = new Context())
            {
                var checkDepartment = dContext.Departments.FirstOrDefault(i => i.Name == newDepartment.Name);

                if (checkDepartment == null)
                {
                    dContext.Add(newDepartment);
                    dContext.SaveChanges();
                    return true;
                }
                else return false;

            }
        }

        internal bool Update(Department department)
        {
            using (var dContext = new Context())
            {
                dContext.Departments.Update(department);
                dContext.SaveChanges();
                return true;
            }
            }
    }
}
