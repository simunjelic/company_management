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
    }
}
