using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    internal class ProjectRepository
    {
        Context _context = null;
        public static IEnumerable<Project> GetAll()
        {
            using (var _context = new Context()) {
                return _context.Projects
                    .Include(e => e.Type)
                    .Include(e => e.Location)
                    .ToList();
            }
            
        }

        internal void Add(Project newProject)
        {
            using (var _context = new Context())
            {
                _context.Projects.Add(newProject);
                _context.SaveChanges();
            }
        }

        internal void DeleteById(int id)
        {
            using(var _context = new Context())
            {
                var check =_context.Projects.FirstOrDefault(i => i.Id == id);
                if(check != null)
                {
                    _context.Projects.Remove(check);
                    _context.SaveChanges();
                }

            }
        }

        internal IEnumerable<Location> GetAllLocations()
        {
            using(_context = new Context())
            {
                return _context.Locations.ToList();
            }
        }

        internal IEnumerable<Models.Type> GetAllTypes()
        {
            using( var _context = new Context())
            {
                return _context.Types.ToList();
            }
        }
    }
}
