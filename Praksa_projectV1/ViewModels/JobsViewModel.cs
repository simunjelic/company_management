using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.ViewModels
{
    public class JobsViewModel: ViewModelBase
    {
        private JobRepository repository;

        public JobsViewModel()
        {
            repository = new JobRepository();
            GetAll();
        }

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private ObservableCollection<Job> _jobRecords;
        public ObservableCollection<Job> JobRecords
        {
            get
            {
                return _jobRecords;
            }
            set
            {
                _jobRecords = value;
                OnPropertyChanged("JobRecords");
            }
        }

        public void GetAll()
        {
            JobRecords = new ObservableCollection<Job>(repository.GetAllJobs());

        }

    }


}
