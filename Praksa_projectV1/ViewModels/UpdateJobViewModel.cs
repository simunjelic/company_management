using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public  class UpdateJobViewModel : ViewModelBase
    {
        private JobRepository repository;
        private DepartmentRepository departmentRepository;
        public ICommand UpdateJobCommand { get; }


        public UpdateJobViewModel()
        {
            repository = new JobRepository();
            departmentRepository = new DepartmentRepository();
            GetAllDepartments();
            UpdateJobCommand = new ViewModelCommand(UpdateCommand, CanUpdateCommand);
            
        }

        private bool CanUpdateCommand(object obj)
        {
            if (SelectedDepartment != null && !string.IsNullOrEmpty(ChangeName))
            {
               return true;
                
            }
            else return false;
            
        }

        private void UpdateCommand(object obj)
        {
            Job updateJob = new Job();
            {
                updateJob.Name = ChangeName;
                updateJob.DepartmentId = SelectedDepartment.Id;
                updateJob.Id = Id;
                var progress = false;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to update this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    progress = repository.updateJob(updateJob);
                }
                

                if(progress == true)
                {
                    string message = "Success! Name changed to: " + ChangeName;

                    MessageBox.Show(message);
                    _isViewVisible = false;
                }
                updateJob = null;
                
                

            };
        }

        private string _changeName;
        public string ChangeName
        {
            get
            {
                return _changeName;
            }
            set
            {
                _changeName = value;
                OnPropertyChanged(nameof(ChangeName));
            }
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
                OnPropertyChanged(nameof(Id));
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
                OnPropertyChanged(nameof(SelectedDepartment));
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
        public void GetAllDepartments()
        {
            DepartmentRecords = new ObservableCollection<Department>(departmentRepository.GetAllDepartments());

        }

        public void ShowWindow(int id)
        {

           Job job = repository.GetJob(id);
            DepartmentRecords.Where(x => x.Id == job.DepartmentId);
            Id = id;
            ChangeName = job.Name;
            SelectedDepartment = (Department)DepartmentRecords.Where(x => x.Id == job.DepartmentId).Single();
            UpdateJobView update = new UpdateJobView();
            update.DataContext = this;
            update.Show();
        }

        
    }
}
