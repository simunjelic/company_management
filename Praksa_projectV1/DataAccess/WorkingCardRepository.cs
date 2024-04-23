using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    class WorkingCardRepository
    {
        internal static async Task<IEnumerable<Activity>> GetAllActivties()
        {
            try
            {
                using (var context = new Context())
                {
                    
                        return await context.Activities
                                  .ToListAsync();
                    
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {

                using (var _context = new Context())
                {
                    var check = await _context.WorkingCards.FirstOrDefaultAsync(i => i.Id == id);
                    if (check != null)
                    {
                        _context.WorkingCards.Remove(check);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        internal async Task<IEnumerable<WorkingCard>> GetAllData()
        {
            try
            {
                using (var context = new Context())
                {
                    if (Thread.CurrentPrincipal?.Identity.Name != null)
                    {
                        var username = Thread.CurrentPrincipal?.Identity.Name.ToString();
                        //var employee = await context.Employees.FirstOrDefaultAsync(i => i.User.Username == username);



                    return await context.WorkingCards
                              .Include(p => p.Project)
                              .Include(p => p.Activity)
                              .Include(p => p.Employee)
                              .Where(p => p.Employee.User.Username == username)
                              .ToListAsync();
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
