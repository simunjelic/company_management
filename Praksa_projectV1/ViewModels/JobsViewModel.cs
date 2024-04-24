using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class JobsViewModel: ViewModelBase
    {
        private JobRepository repository;
        private DepartmentRepository departmentRepository;
        public ICommand ShowWindowCommand { get;}
        public ICommand AddjobCommand { get; }
        public ICommand DeleteJobCommand { get; }
        public ICommand UpdateJobCommand { get; }
        public ICommand EditJobCommand { get; }



        public JobsViewModel()
        {
            repository = new JobRepository();   
            departmentRepository = new DepartmentRepository();
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            AddjobCommand = new ViewModelCommand(AddJob, CanAddJob);
            DeleteJobCommand = new ViewModelCommand(DeleteJob, CanDeleteJob);
            UpdateJobCommand = new ViewModelCommand(ShowEditJob, CanShowEditJob);
            EditJobCommand = new ViewModelCommand(EditJob, CanEditJob);
            GetAll();
            GetAllDepartments();
            ResetData();
            
        }

        private bool CanEditJob(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private void EditJob(object obj)
        {
            Job updateJob = new Job();

            updateJob.Name = AddName;
            updateJob.DepartmentId = SelectedDepartment.Id;
            updateJob.Id = Id;
            var progress = false;

            MessageBoxResult result = MessageBox.Show("Jeste li sigurni da želite spramiti promjene?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                progress = repository.updateJob(updateJob);
            }


            if (progress == true)
            {
                string message = "Success! Name changed to: " + AddName;

                MessageBox.Show(message);
                _isViewVisible = false;
                GetAll();
                ResetData();
            }
            updateJob = null;
        }

        private bool CanShowEditJob(object obj)
        {
            return true;
        }

        private void ShowEditJob(object obj)
        {
            if(obj is int id)
            {
                Job job = repository.GetJob(id);
                Id = id;
                AddName = job.Name;
                SelectedDepartment = (Department)DepartmentRecords.Where(x => x.Id == job.DepartmentId).Single();
                UpdateJobView update = new UpdateJobView();
                update.DataContext = this;
                _isViewVisible = true;
                update.Show();
                GetAll();

            }
        }

        private bool CanDeleteJob(object obj)
        {
            return true;
        }

        private void DeleteJob(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this job?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            

            if (obj is int Id && result == MessageBoxResult.Yes)
            {
                bool check = repository.RemoveJob(Id);
                if (check)
                {
                    MessageBox.Show("Posao obrisan.");
                    JobRecords.Remove(JobRecords.Where(x => x.Id == Id).Single());
                }
                else
                {
                    MessageBox.Show("Nije moguće izbrisati, posao povezan sa drugim poljima.");
                }
                
            }

            
        }


        private bool CanAddJob(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
            
        }

        private void AddJob(object obj)
        {
            
                Job newJob = new Job();
              { 
                newJob.Name = AddName;
                newJob.DepartmentId = SelectedDepartment.Id;
                    
              };

            if (repository.AddJob(newJob)== true)
            {

                MessageBox.Show("New record successfully saved.");
                ResetData();
                IsViewVisible = false;
                GetAll();
            }
            else
            {
                MessageBox.Show("Job with same name exist.");
            }
            



        }
       
       

        private bool CanShowWindow(object obj)
        {
            return true;
        }

        private void ShowWindow(object obj)
        {
            AddJobView addJobView = new AddJobView();
            addJobView.DataContext = this;
            _isViewVisible = true;
            ResetData();
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
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string AddName
        {
            get
            {
                return _addName;
            }
            set
            {
                _addName = value;
                Validate(nameof(AddName), value);
                OnPropertyChanged("AddName");
            }
        }
        private Department _selectedDepartment;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Department SelectedDepartment
        {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                _selectedDepartment = value;
                Validate(nameof(SelectedDepartment), value);
                OnPropertyChanged("SelectedDepartment");
            }

        }
        private bool _isViewVisible = true;
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
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
               
                OnPropertyChanged(nameof(JobRecords));
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
        private void ResetData()
        {
            AddName = null;
            SelectedDepartment = null;
        }



    }


}
