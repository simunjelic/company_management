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
            return jobContext.Jobs.ToList();
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
    }
}
