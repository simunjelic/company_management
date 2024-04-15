using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void AddJob(Job job)
        {
            using (jobContext)
            {
                if (job != null)
                {
                    jobContext.Jobs.Add(job);
                    jobContext.SaveChanges();
                }
            }
        }
        public void RemoveJob(int Id)
        {
            using (jobContext = new Context())
            {
                Job jobToDelete = jobContext.Jobs.FirstOrDefault(i => i.Id == Id);

                if (jobToDelete != null)
                jobContext.Jobs.Remove(jobToDelete);
                jobContext.SaveChanges();
            }
        }
        
    }
}
