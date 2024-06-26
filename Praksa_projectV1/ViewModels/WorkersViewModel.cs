﻿using Microsoft.IdentityModel.Tokens;
using Praksa_projectV1.Commands;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Praksa_projectV1.ViewModels
{

    public class WorkersViewModel : ViewModelBase
    {
        public IEmployeeRepository EmpolyeeRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IDepartmentRepository departmentRepository { get; set; }
        public IJobRepository jobRepository { get; set; }
        public ICommand ShowWindowCommand { get; }
        public IAsyncCommand DeleteCommand { get; }
        public IAsyncCommand AddEmployeeCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public IAsyncCommand UpdateEmployeeCommand { get; }
        public IAsyncCommand LoadedCommand { get; }
        public readonly string ModuleName = "Zaposlenici";


        public WorkersViewModel()
        {
            EmpolyeeRepository = new EmployeeRepository();
            UserRepository = new UserRepository();
            departmentRepository = new DepartmentRepository();
            jobRepository = new JobRepository();
            ShowWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            DeleteCommand = new AsyncCommand(DeleteEmployeeAsync, CanDeleteEmployeeAsync);
            AddEmployeeCommand = new AsyncCommand(AddEmployeeAsync, CanAddEmployeeAsync);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindowCommand);
            UpdateEmployeeCommand = new AsyncCommand(UpdateEmployeeAsync, CanUpdateEmployeeAsync);
            LoadedCommand = new AsyncCommand(OnLoadAsync);
            SelectedDate = DateTime.Today;
        }

        private async Task OnLoadAsync()
        {
            await GetAllWorkersAsync();
            await GetAllUsersAsync();
            await GetAllDepartmentsAsync();
            await GetAllJobsAsync();
        }

        private bool CanShowUpdateWindowCommand(object obj)
        {
            return CanUpdatePermission(ModuleName) && SelectedItem != null;
        }

        private void ShowUpdateWindow(object obj)
        {
            PopulateUpdateWindow();
            WorkersEditView workersEditView = new();
            workersEditView.DataContext = this;
            workersEditView.Title = "Uredi korisnika";
            IsAddButtonVisible = false;
            IsUpdateButtonVisible = true;
            workersEditView.Show();
        }

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {
            ResetData();
            WorkersEditView workersEditView = new();
            workersEditView.DataContext = this;
            workersEditView.Title = "Dodaj korisnika";
            IsUpdateButtonVisible = false;
            IsAddButtonVisible = true;
            workersEditView.Show();
        }

        private bool CanAddEmployeeAsync()
        {
            return true;
        }

        public async Task AddEmployeeAsync()
        {
            Employee newEmployee = new();
            if (ValidationInput())
            {
                populateEmployeeData(newEmployee);
                bool IsTrue = await EmpolyeeRepository.AddAsync(newEmployee);
                if (IsTrue)
                {
                    MessageBox.Show("Dodan novi zaposlenik.");
                    newEmployee.IsActiveText = IsActive ? "Da" : "Ne";
                    WorkersRecords.Add(newEmployee);
                    ResetData();
                }
                else
                {
                    MessageBox.Show("Korisnik povezan s evidencijom drugog zaposlenika.");
                }
            }
        }

        private bool CanUpdateEmployeeAsync()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private async Task UpdateEmployeeAsync()
        {
            if (ValidationInput())
            {
                Employee UpdateEmployee = new();
                UpdateEmployee = populateEmployeeData(SelectedItem);
                bool IsTrue = await EmpolyeeRepository.UpdateAsync(UpdateEmployee);
                if (IsTrue)
                {
                    MessageBox.Show("Zaposlenik ažuriran.");
                    UpdateEmployee.IsActiveText = IsActive ? "Da" : "Ne";

                    int index = -1;
                    index = WorkersRecords.IndexOf(WorkersRecords.Where(x => x.Id == Id).Single());
                    WorkersRecords[index] = await EmpolyeeRepository.GetByIdAsync(Id);
                    ResetData();
                }
                else
                {
                    MessageBox.Show("Greška.");
                }
            }
        }

        private bool CanDeleteEmployeeAsync()
        {
            return CanDeletePermission(ModuleName);

        }

        private async Task DeleteEmployeeAsync()
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ovog zaposlenika s imenom: " + SelectedItem.Name + " " + SelectedItem.Surname + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool isTrue = await EmpolyeeRepository.DeleteAsync(SelectedItem);
                    if (isTrue)
                    {
                        WorkersRecords.Remove(SelectedItem);
                        MessageBox.Show("Zaposlenik uspješno obrisan.");
                    }
                    else MessageBox.Show("Greška prilikom brisanja zaposlenika.");




                }
                SelectedItem = null;
            }
            else MessageBox.Show("Odaberite redak koji želite obrisati.");
        }

        

        




        public void PopulateUpdateWindow()
        {

            Id = SelectedItem.Id;
            SelectedUser = UsersRecords.Where(i => i.Id == SelectedItem.UserId).Single();
            if (SelectedItem.DepartmentId != null)
                SelectedDepartment = DepartmentRecords.Where(i => i.Id == SelectedItem.DepartmentId).Single();
            if (SelectedItem.JobId != null)
                SelectedJob = JobRecords.Where(i => i.Id == SelectedItem.JobId).Single();
            Name = SelectedItem.Name;
            Surname = SelectedItem.Surname;
            DateOnly dateOnly = (DateOnly)SelectedItem.Birthday;
            SelectedDate = dateOnly.ToDateTime(TimeOnly.Parse("10:00 PM"));
            if (SelectedItem.Jmbg != null)
                Jmbg = SelectedItem.Jmbg;
            Address = SelectedItem.Address;
            Email = SelectedItem.Email;
            Phone = SelectedItem.Phone;
            IsActive = SelectedItem.IsActive;



        }



        private Employee populateEmployeeData(Employee newEmployee)
        {
            newEmployee.UserId = SelectedUser.Id;
            if (SelectedJob != null)
            {
                newEmployee.JobId = SelectedJob.Id;
                newEmployee.Job = null;
            }

            if (SelectedDepartment != null)
            {
                newEmployee.DepartmentId = SelectedDepartment.Id;
                newEmployee.Department = null;
            }

            newEmployee.Name = Name;
            newEmployee.Surname = Surname;
            newEmployee.Birthday = new DateOnly(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
            if (Jmbg != null)
                newEmployee.Jmbg = Jmbg;
            if (Address != null)
                newEmployee.Address = Address;
            if (Email != null)
                newEmployee.Email = Email;
            if (Phone != null)
                newEmployee.Phone = Phone;
            newEmployee.IsActive = IsActive;


            return newEmployee;
        }

        public bool ValidationInput()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname))
            {
                MessageBox.Show("Unesi ime i prezime.");
                return false;
            }
            if (SelectedUser == null)
            {
                MessageBox.Show("Unesi korisnika.");
                return false;
            }
            if (SelectedItem == null)
            {
                if (WorkersRecords.Any(i => i.UserId == SelectedUser.Id))
                {
                    MessageBox.Show("Odabrani korisnik povezan sa drugim zaposlenikom.");

                    return false;
                }
            }
            else
            {
                if (WorkersRecords.Any(i => i.UserId == SelectedUser.Id && i.Id != SelectedItem.Id))
                {
                    MessageBox.Show("Odabrani korisnik povezan sa drugim zaposlenikom.");

                    return false;
                }

            }
            if (Jmbg != null)
            {

                if (Jmbg.ToString().Length != 13)
                {
                    MessageBox.Show("Nesipravan oblik JMBG-a.");
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
            {
                MessageBox.Show("Neisparvan oblik emaila.");
                return false;
            }
            TimeSpan age = DateTime.Today - SelectedDate;

            // Check if the age is outside the specified range
            if (age.TotalDays < 3650) // Less than 10 years (3650 days)
            {
                MessageBox.Show("Odabrani datum rođenja mora biti stariji od 10 godina.");
                return false;
            }
            else if (age.TotalDays > 36500) // More than 100 years (36500 days)
            {
                MessageBox.Show("Odabrani datum rođenja mora biti mlađi od 100 godina.");
                return false;
            }

            return true;

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
        private bool _isActive = true;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        private string _name;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                Validate(nameof(Name), value);
                OnPropertyChanged("Name");
            }
        }
        private string _surname;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                Validate(nameof(Surname), value);
                OnPropertyChanged("Surname");
            }
        }
        private long? _jmbg;
        public long? Jmbg
        {
            get
            {
                return _jmbg;
            }
            set
            {
                _jmbg = value;
                OnPropertyChanged("Jmbg");
            }
        }
        private string _address;
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }
        private string _email;
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Neisparvan format email adrese.")]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                Validate(nameof(Email), value);
                OnPropertyChanged("Email");
            }
        }
        private string _phone;
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (value != null && value.Length > 0 && value[0] != '+')
                {
                    // If the first character is not '+', add it
                    _phone = "+" + value;
                }
                else
                {
                    _phone = value;
                }
                OnPropertyChanged(nameof(Phone));
            }
        }
        private ObservableCollection<Employee> _workersRecords;
        public ObservableCollection<Employee> WorkersRecords
        {
            get
            {
                return _workersRecords;
            }
            set
            {
                _workersRecords = value;
                OnPropertyChanged(nameof(WorkersRecords));
            }
        }
        private ObservableCollection<User> _usersRecords;
        public ObservableCollection<User> UsersRecords
        {
            get
            {
                return _usersRecords;
            }
            set
            {
                _usersRecords = value;
                OnPropertyChanged(nameof(UsersRecords));
            }
        }
        private Employee _selectedItem;
        public Employee? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        private User _selectedUser;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public User? SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    Validate(nameof(SelectedUser), value);
                    OnPropertyChanged(nameof(SelectedUser));
                }
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
        private Job _selectedJob;

        public Job SelectedJob
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
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public async Task GetAllWorkersAsync()
        {
            var employes = await EmpolyeeRepository.GetAllAsync();



            WorkersRecords = new ObservableCollection<Employee>(employes);

        }
        public async Task GetAllUsersAsync()
        {
            UsersRecords = new ObservableCollection<User>(await UserRepository.getAllUsersAsync());
        }
        public async Task GetAllDepartmentsAsync()
        {
            var departments = await departmentRepository.GetAllDepartmentsAsync();

            DepartmentRecords = new ObservableCollection<Department>(departments);

        }
        public async Task GetAllJobsAsync()
        {

            JobRecords = new ObservableCollection<Job>(await jobRepository.GetAllJobsAsync());

        }
        public void ResetData()
        {
            SelectedDate = DateTime.Now;
            Id = -1;
            Name = string.Empty;
            Surname = string.Empty;
            Jmbg = null;
            Address = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            SelectedDepartment = null;
            SelectedJob = null;
            SelectedUser = null;
            SelectedItem = null;
            IsActive = true;

        }
    }
}
