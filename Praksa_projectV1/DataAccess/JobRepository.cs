using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Enums;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Praksa_projectV1.DataAccess
{
    public class JobRepository : IJobRepository
    {
        private Context jobContext = null;
        public JobRepository()
        {
            jobContext = new Context();
        }
        
        public bool AddJob(Job job)
        {
            using (jobContext = new Context())
            {
                Job checkJob = jobContext.Jobs.FirstOrDefault(i => i.Name == job.Name);
                if (checkJob == null)
                {
                    jobContext.Jobs.Add(job);
                    jobContext.SaveChanges();
                    return true;
                }else return false;

            }
        }
        
        public Job GetJob(int Id)
        {
            using (jobContext = new Context())
            {
                Job job = jobContext.Jobs.FirstOrDefault(i => i.Id == Id);
                return job;
            }
        }
        

        public async Task<bool> AddJobAsync(Job newJob)
        {
            try
            {
                using (jobContext = new Context())
                {
                        await jobContext.Jobs.AddAsync(newJob);
                        var isTrue = await jobContext.SaveChangesAsync();
                        return isTrue > 0;

                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveJob(Job job)
        {
            try
            {
                using (jobContext = new Context())
                {
                    jobContext.Jobs.Remove(job);
                    var RowsAffected = await jobContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> updateJobAsync(Job job)
        {
            try
            {
                using (jobContext = new Context())
                {

                    jobContext.Update(job);
                    var isChanged = await jobContext.SaveChangesAsync();
                    return isChanged > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            try
            {
                using (var _context = new Context())
                {
                    var permissions = await _context.Jobs
                        .Include(e => e.Department)
                        .ToListAsync();
                    return permissions;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
