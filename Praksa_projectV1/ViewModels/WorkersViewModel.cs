using Microsoft.IdentityModel.Tokens;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
   
    public class WorkersViewModel: ViewModelBase
    {
        EmployeeRepository EmpolyeeRepository { get; }
        UserRepository UserRepository { get; }
        DepartmentRepository departmentRepository { get; }
        JobRepository jobRepository { get; }
        public ICommand ShowWindowCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }


        public WorkersViewModel()
        {
            EmpolyeeRepository = new EmployeeRepository();
            UserRepository = new UserRepository();
            departmentRepository = new DepartmentRepository();
            jobRepository = new JobRepository();
            GetAllWorkers();
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            AddEmployeeCommand = new ViewModelCommand(AddEmployee, CanAddEmployee);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindowCommand);
            UpdateEmployeeCommand = new ViewModelCommand(UpdateEmployee, CanUpdateEmployee);
            SelectedDate = DateTime.Today;
        }

        private bool CanUpdateEmployee(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void UpdateEmployee(object obj)
        {
            if (validationInput()) {

                SelectedItem = populateEmployeeData(SelectedItem);
            if (EmpolyeeRepository.Update(SelectedItem))
            {
                MessageBox.Show(" Employee updated");
                    int index = -1;
                    index = WorkersRecords.IndexOf(WorkersRecords.Where(x => x.Id == Id).Single());
                    
                    WorkersRecords[index] = EmpolyeeRepository.GetById(Id);
                    ResetData();
            }
            else
            {
                MessageBox.Show("Error.");
            }
            }
        }

        private bool CanShowUpdateWindowCommand(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void ShowUpdateWindow(object obj)
        {
            
            GetAllUsers();
            GetAllDepartments();
            GetAllJobs();
            populateUpdateWindow();
            WorkersEditView workersEditView = new();
            workersEditView.DataContext = this;
            workersEditView.Title = "Edit user";
            _isAddButtonVisible = false;
            _isUpdateButtonVisible = true;
            workersEditView.Show();
            
        }

       

        private void populateUpdateWindow()
        {
            
            Id = SelectedItem.Id;
            SelectedUser = UsersRecords.Where(i => i.Id == SelectedItem.UserId).Single();
            if(SelectedItem.DepartmentId != null)
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
            


        }

        private bool CanAddEmployee(object obj)
        {
            return true;
        }

        private void AddEmployee(object obj)
        {
            Employee newEmployee = new();
            if (validationInput())
            {
                populateEmployeeData(newEmployee);
                if (EmpolyeeRepository.Add(newEmployee))
                {
                    MessageBox.Show("New employee added");
                    WorkersRecords.Add(EmpolyeeRepository.FindByUserId(newEmployee.UserId));
                    ResetData();
                }
                else
                {
                    MessageBox.Show("User connected with other employee record.");
                }
            }
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
            if (Jmbg!= null)
                newEmployee.Jmbg = Jmbg;
            if (Address != null)
                newEmployee.Address = Address;
            if (Email != null)
                newEmployee.Email = Email;
            if (Phone != null)
                newEmployee.Phone = Phone;

            
            return newEmployee;
        }

        private bool validationInput()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname))
            {
                MessageBox.Show("Enter name or surname");
                return false;
            }
            if(SelectedUser == null) {
                MessageBox.Show("Enter user");
                return false;
            }
            if(Jmbg != null)
            {
               
                if (Jmbg.ToString().Length != 13) { 
                MessageBox.Show("JMBG must contain 13 numbers");
                return false;
                }
            }
            if(!string.IsNullOrEmpty(Email) && !Email.Contains("@"))
            {
                MessageBox.Show("Email must contain @");
                return false;
            }
            TimeSpan age = DateTime.Today - SelectedDate;

            // Check if the age is outside the specified range
            if (age.TotalDays < 3650) // Less than 10 years (3650 days)
            {
                MessageBox.Show("Selected date should be older than 10 years.");
                return false;
            }
            else if (age.TotalDays > 36500) // More than 100 years (36500 days)
            {
                MessageBox.Show("Selected date should be younger than 100 years.");
                return false;
            }
            
            return true;
           
        }

        private bool CanDelete(object obj)
        {
            if(SelectedItem!=null)
                return true;
            return false;
        }

        private void Delete(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Employee with name: "+SelectedItem.Name +" "+SelectedItem.Surname+"?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                EmpolyeeRepository.DeleteById(SelectedItem.Id);
                WorkersRecords.Remove(SelectedItem);
                


            }
            SelectedItem = null;
        }

        private bool CanShowWindow(object obj)
        {
            return true;
        }

        private void ShowWindow(object obj)
        {
            GetAllUsers();
            GetAllDepartments();
            GetAllJobs();
            ResetData();
            WorkersEditView workersEditView = new();
            workersEditView.DataContext = this;
            workersEditView.Title = "Add user";
            _isUpdateButtonVisible = false;
            _isAddButtonVisible = true;
            workersEditView.Show();


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
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
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
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
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
        public User? SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
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
        private bool _isUpdateButtonVisible = true; // Initially visible

        public bool IsUpdateButtonVisible
        {
            get { return _isUpdateButtonVisible; }
            set
            {
                if (_isUpdateButtonVisible != value)
                {
                    _isUpdateButtonVisible = value;
                    OnPropertyChanged(nameof(IsUpdateButtonVisible)); // Notify property changed
                }
            }
        }
        private bool _isAddButtonVisible = true; // Initially visible

        public bool IsAddButtonVisible
        {
            get { return _isAddButtonVisible; }
            set
            {
                if (_isAddButtonVisible != value)
                {
                    _isAddButtonVisible = value;
                    OnPropertyChanged(nameof(IsAddButtonVisible)); // Notify property changed
                }
            }
        }

        public void GetAllWorkers()
        {
            var employes = EmpolyeeRepository.GetAll();

         

            WorkersRecords = new ObservableCollection<Employee>(employes);

        }
        public void GetAllUsers()
        {
            UsersRecords = new ObservableCollection<User>(UserRepository.getAllUsers());
        }
        public void GetAllDepartments()
        {
            var departments = departmentRepository.GetAllDepartments();
            foreach (var department in departments)
            {
                if (department.ParentDepartmentId != null)
                {
                    department.ParentDepartment = departments.Where(i => i.Id == department.ParentDepartmentId).FirstOrDefault();
                }
            }
            DepartmentRecords = new ObservableCollection<Department>(departments);

        }
        public void GetAllJobs()
        {
            
            JobRecords = new ObservableCollection<Job>(jobRepository.GetAllJobs());

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

        }
    }
}
