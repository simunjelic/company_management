﻿using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        internal async Task<bool> Add(WorkingCard newCard)
        {
            try
            {
                using (var context = new Context())
                {
                    await context.WorkingCards.AddAsync(newCard);
                    await context.SaveChangesAsync();
                    return true;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Greška pri spremanju podataka: " + ex);
                return false;
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

        internal bool Edit(WorkingCard card)
        {
            try
            {
                using (var context = new Context())
                {
                    context.WorkingCards.Update(card);
                    context.SaveChanges();

                    return true;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        internal async Task<WorkingCard> FindByIdAsync(int id)
        {
            try
            {
                using (var context = new Context())
                {
                    return await  context.WorkingCards.Include(p => p.Project)
                                  .Include(p => p.Activity)
                                  .Include(p => p.Employee).FirstOrDefaultAsync(i => i.Id == id);
                }

            }
            catch
            {
                return null;
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

        internal async Task<IEnumerable<WorkingCard>> GetByStartAndEndDate(DateOnly? startDate, DateOnly? endDate)
        {
            try
            {
                using (var context = new Context())
                {
                    if (Thread.CurrentPrincipal?.Identity.Name != null)
                    {
                        var username = Thread.CurrentPrincipal?.Identity.Name.ToString();

                        var query = context.WorkingCards
                                            .Include(p => p.Project)
                                            .Include(p => p.Activity)
                                            .Include(p => p.Employee)
                                            .Where(p => p.Employee.User.Username == username);

                        // Apply filters for startDate and endDate if provided
                        if (startDate.HasValue)
                        {
                            query = query.Where(p => p.Date >= startDate.Value);
                        }

                        if (endDate.HasValue)
                        {
                            query = query.Where(p => p.Date <= endDate.Value);
                        }

                        return await query.ToListAsync();
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return null;
            }
        }


    }
}
