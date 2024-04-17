using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Praksa_projectV1.DataAccess
{
    public class EmpolyeeRepository
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
    }
}
