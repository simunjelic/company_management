using Praksa_projectV1.Commands;
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
    public class JobsViewModel : ViewModelBase
    {
        private JobRepository repository;
        private DepartmentRepository departmentRepository;
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddjobCommand { get; }
        public ICommand DeleteJobCommand { get; }
        public ICommand UpdateJobCommand { get; }
        public IAsyncCommand EditJobCommand { get; private set; }
        public readonly string ModuleName = "Radno mjesto";



        public JobsViewModel()
        {
            repository = new JobRepository();
            departmentRepository = new DepartmentRepository();
            ShowAddWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            AddjobCommand = new ViewModelCommand(AddJob, CanAddJob);
            DeleteJobCommand = new ViewModelCommand(DeleteJob, CanDeleteJob);
            UpdateJobCommand = new ViewModelCommand(ShowEditJob, CanShowEditJob);
            EditJobCommand = new AsyncCommand(EditJob, CanEditJob);
            GetAll();
            GetAllDepartments();
            ResetData();

        }

        private async Task EditJob()
        {
            Job updateJob = new Job();

            updateJob.Name = AddName;
            updateJob.DepartmentId = SelectedDepartment.Id;
            updateJob.Id = Id;
            var progress = false;
            if (!JobRecords.Any(i => i.Id != updateJob.Id && i.Name == updateJob.Name))
            {
                MessageBoxResult result = MessageBox.Show("Jeste li sigurni da želite spremiti promjene?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    progress = await repository.updateJobAsync(updateJob);
                    if (progress == true)
                    {
                        string message = "Naziv promijenjen " + AddName;

                        MessageBox.Show(message);
                        _isViewVisible = false;
                        GetAll();
                        ResetData();
                    }
                    else
                    {
                        MessageBox.Show("Greška pri uređivanju.");
                    }
                    updateJob = null;
                }
            }
            else MessageBox.Show("Posao sa istim imenom već postoji");

        }

        private bool CanEditJob()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        

        private bool CanShowEditJob(object obj)
        {
            return CanUpdatePermission(ModuleName) && SelectedJob != null;
        }

        private void ShowEditJob(object obj)
        {

            Job job = SelectedJob;
            Id = SelectedJob.Id;
            AddName = job.Name;
            SelectedDepartment = (Department)DepartmentRecords.Where(x => x.Id == job.DepartmentId).Single();
            UpdateJobView update = new UpdateJobView();
            update.DataContext = this;
            _isViewVisible = true;
            IsUpdateButtonVisible = true;
            IsAddButtonVisible = false;
            update.Show();

        }

        private bool CanDeleteJob(object obj)
        {
            return CanDeletePermission(ModuleName) && SelectedJob != null;
        }

        private async void DeleteJob(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ovaj posao?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);


            if (result == MessageBoxResult.Yes)
            {
                bool check = await repository.RemoveJob(SelectedJob.Id);
                if (check)
                {
                    MessageBox.Show("Posao obrisan.");
                    JobRecords.Remove(SelectedJob);
                    ResetData();
                }
                else
                {
                    MessageBox.Show("Nije moguće izbrisati, posao povezan sa drugim poljima.");
                }

            }


        }


        private bool CanAddJob(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null) && CanCreatePermission(ModuleName);

        }

        private void AddJob(object obj)
        {

            Job newJob = new Job();
            {
                newJob.Name = AddName;
                newJob.DepartmentId = SelectedDepartment.Id;

            };

            if (repository.AddJob(newJob) == true)
            {

                MessageBox.Show("Novi zapis je uspješno spremljen.");
                ResetData();
                IsViewVisible = false;
                GetAll();
            }
            else
            {
                MessageBox.Show("Posao s istim imenom postoji.");
            }




        }



        private bool CanShowWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowWindow(object obj)
        {
            AddJobView addJobView = new AddJobView();
            addJobView.DataContext = this;
            addJobView.Title = "Dodaj novi posao";
            _isViewVisible = true;
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
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
        private Job _selectedJob;
        public Job? SelectedJob
        {
            get
            {
                return _selectedJob;
            }
            set
            {
                _selectedJob = value;
                OnPropertyChanged("SelectedJob");
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
        public string? AddName
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
        public Department? SelectedDepartment
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
            SelectedJob = null;
        }



    }


}
