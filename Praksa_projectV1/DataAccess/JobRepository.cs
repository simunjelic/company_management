using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Praksa_projectV1.DataAccess
{
    public class JobRepository
    {
        private Context jobContext = null;
        public JobRepository()
        {
            jobContext = new Context();
        }
        public List<Job> GetAllJobs()
        {
            using (jobContext = new Context())
            { 

                var deps = jobContext.Departments.ToList();
            var jobList = new List<Job>();
            jobList = jobContext.Jobs.ToList();
            foreach(var job in jobList)
            {
                foreach(var dep in deps)
                {
                    if(dep.Id == job.DepartmentId)
                    {
                        job.Department = dep;
                        break;
                    }
                }
            }

            return jobList;
            }
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
        public async Task<bool> RemoveJob(int Id)
        {
            try { 
            using (jobContext = new Context())
            {
                Job jobToDelete = await  jobContext.Jobs.FirstOrDefaultAsync(i => i.Id == Id);

                if (jobToDelete != null)
                jobContext.Jobs.Remove(jobToDelete);
                await jobContext.SaveChangesAsync();
                    return true;
            }
            }catch(Exception ex)
            {
                return false;
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
        public async Task<bool> updateJobAsync(Job job)
        {
            try { 
            using (jobContext = new Context())
            {
                
                    jobContext.Update(job);
                    await jobContext.SaveChangesAsync();
                    return true;   
            }
            }
            catch
            {
                return false;
            }
        }
        
    }
}
