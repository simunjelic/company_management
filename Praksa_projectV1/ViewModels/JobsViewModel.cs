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
        public IAsyncCommand ShowAddWindowCommand { get; }
        public IAsyncCommand AddjobCommand { get; }
        public IAsyncCommand DeleteJobCommand { get; }
        public IAsyncCommand UpdateJobCommand { get; }
        public IAsyncCommand EditJobCommand { get; private set; }
        public readonly string ModuleName = "Radno mjesto";



        public JobsViewModel()
        {
            repository = new JobRepository();
            departmentRepository = new DepartmentRepository();
            ShowAddWindowCommand = new AsyncCommand(ShowWindowAsync, CanShowWindowAsync);
            AddjobCommand = new AsyncCommand(AddJobAsync, CanAddJobAsync);
            DeleteJobCommand = new AsyncCommand(DeleteJobAsync, CanDeleteJobAsync);
            UpdateJobCommand = new AsyncCommand(ShowEditJobAsync, CanShowEditJobAsync);
            EditJobCommand = new AsyncCommand(EditJob, CanEditJob);
            GetAll();
            GetAllDepartmentsAsync();
            ResetData();

        }

        private bool CanShowWindowAsync()
        {
            return CanCreatePermission(ModuleName);
        }

        private async Task ShowWindowAsync()
        {
            AddJobView addJobView = new AddJobView();
            addJobView.DataContext = this;
            addJobView.Title = "Dodaj novi posao";
            _isViewVisible = true;
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            await ResetData();
            addJobView.Show();
        }

       

       

        private bool CanAddJobAsync()
        {
            return true;
        }

        private async Task AddJobAsync()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null))
            {
                Job newJob = new Job();
                {
                    newJob.Name = AddName;
                    if(SelectedDepartment != null)
                    newJob.DepartmentId = SelectedDepartment.Id;

                };

                if (await repository.AddJobAsync(newJob))
                {

                    newJob.Department = SelectedDepartment;
                    JobRecords.Add(newJob);
                    ResetData();
                    IsViewVisible = false;
                    MessageBox.Show("Novi zapis je uspješno spremljen.");

                }
                else
                {
                    MessageBox.Show("Posao s istim imenom postoji.");
                }
            }
            else MessageBox.Show("Popunite sva polja označena crveno.", "Upozorenje");
        }

        private bool CanDeleteJobAsync()
        {
            return CanDeletePermission(ModuleName);
        }

        private async Task DeleteJobAsync()
        {
            if (SelectedJob != null)
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
            else MessageBox.Show("Odaberite podatak koji želite obrisati.");
        }

        private bool CanShowEditJobAsync()
        {
            return CanUpdatePermission(ModuleName);
        }

        private async Task ShowEditJobAsync()
        {
            if(SelectedJob != null) { 
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
                        await GetAll();
                        await ResetData();
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

        public async Task GetAll()
        {
            JobRecords =new ObservableCollection<Job>(await repository.GetAllJobsAsync());

        }
        public async Task GetAllDepartmentsAsync()
        {
            DepartmentRecords = new ObservableCollection<Department>(await departmentRepository.GetAllDepartmentsAsync());

        }
        private Task ResetData()
        {
            AddName = null;
            SelectedDepartment = null;
            SelectedJob = null;
            return Task.CompletedTask;
        }
    }


}
