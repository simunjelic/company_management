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
        public void RemoveJob(int Id)
        {
            try { 
            using (jobContext = new Context())
            {
                Job jobToDelete = jobContext.Jobs.FirstOrDefault(i => i.Id == Id);

                if (jobToDelete != null)
                jobContext.Jobs.Remove(jobToDelete);
                jobContext.SaveChanges();
            }
            }catch(Exception ex)
            {
                MessageBox.Show("Nije moguće izbrisati, posao povezan sa drugim poljima.");
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
        public  bool updateJob(Job job)
        {
            using (jobContext = new Context())
            {
                
                    jobContext.Update(job);
                    jobContext.SaveChanges();
                    return true;
                
                

                
            }
        }
        
    }
}
