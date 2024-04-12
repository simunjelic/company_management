using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class JobsViewModel: ViewModelBase
    {
        private JobRepository repository;
        private DepartmentRepository departmentRepository;
        public ICommand ShowWindowCommand { get;}
        public ICommand AddjobCommand { get; }

        public JobsViewModel()
        {
            repository = new JobRepository();   
            departmentRepository = new DepartmentRepository();
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            AddjobCommand = new ViewModelCommand(AddJob, CanAddJob);
            GetAll();
            GetAllDepartments();
        }

        private bool CanAddJob(object obj)
        {
            if(AddName != null && SelectedDepartment != null)
            return true;
            else return false;
        }

        private void AddJob(object obj)
        {
            
                var newJob = new Job();
              { 
                newJob.Name = AddName;
                newJob.DepartmentId = SelectedDepartment.Id;
                    
              };
            
                repository.AddJob(newJob);


        }

        private bool CanShowWindow(object obj)
        {
            return true;
        }

        private void ShowWindow(object obj)
        {
            AddJobView addJobView = new AddJobView();
            addJobView.Show();
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
        private string _addName;
        public string AddName
        {
            get
            {
                return _addName;
            }
            set
            {
                _addName = value;
                OnPropertyChanged("AddName");
            }
        }
        private Department _selectedDepartment;

        public Department SelectedDepartment
        {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged("SelectedDepartment");
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
        private ObservableCollection<Department> _departmentRecords;
        public ObservableCollection<Department> DepartmentRecords
        {
            get
            {
                return _departmentRecords;
            }
            set
            {
                _departmentRecords = value;
                OnPropertyChanged("DepartmentRecords");
            }
        }

        public void GetAll()
        {
            JobRecords = new ObservableCollection<Job>(repository.GetAllJobs());

        }
        public void GetAllDepartments()
        {
            DepartmentRecords = new ObservableCollection<Department>(departmentRepository.GetAllDepartments());

        }

        

    }


}
