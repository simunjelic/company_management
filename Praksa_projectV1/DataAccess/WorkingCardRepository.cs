﻿using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Praksa_projectV1.DataAccess
{
    public class WorkingCardRepository : IWorkingCardRepository
    {

        

        internal static async Task<IEnumerable<Activity>> GetAllActivtiesAsync()
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        public async Task<bool> Add(WorkingCard newCard)
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

                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            };
        }

        public async Task<bool> EditAsync(WorkingCard updateCard)
        {
            try
            {
                using (var context = new Context())
                {
                    context.WorkingCards.Update(updateCard);
                    var RowsAffected = await context.SaveChangesAsync();

                    return RowsAffected > 0;

                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        public async Task<IEnumerable<WorkingCard>> GetByStartAndEndDate(DateOnly? startDate, DateOnly? endDate)
        {
            try
            {
                using (var context = new Context())
                {
                    if (LoggedUserData.Username != null)
                    {
                        var username = LoggedUserData.Username;

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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        public async Task<IEnumerable<MonthlySummary>> GetSummarizedDataByMonth(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                using (var context = new Context())
                {
                    if (LoggedUserData.Username != null)
                    {
                        var username = LoggedUserData.Username;
                        return await context.WorkingCards
                .Where(p => p.Date >= startDate && p.Date <= endDate && p.Employee.User.Username == username)
                .GroupBy(p => new { p.Date.Value.Year, p.Date.Value.Month })
                .Select(g => new MonthlySummary
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalHoursWorked = g.Sum(p => p.Hours ?? 0)
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Month)
                .ToListAsync();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }
    }
}
