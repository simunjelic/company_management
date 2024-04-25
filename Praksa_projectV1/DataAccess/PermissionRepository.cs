using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public class PermissonRepository
    {
        
        internal List<Permission> getPermissionByModuleRead(string modul)
        {
            try
            {
                using (var context = new Context())
                {
                    return context.Permissions
                        .Include(e => e.Role)
                        .Include(e => e.Module)
                        .Where(i => i.Module.Name == modul &&  i.Action=="Dodaj")
                        .ToList();
                }

            }
            catch
            {
                return null;
            }
        }
    }
}
