using Moq;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Praksa1.Tests
{
    public class WorkersTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly WorkersViewModel _viewModel;

        public WorkersTests()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _mockJobRepository = new Mock<IJobRepository>();

            _viewModel = new WorkersViewModel(
            );
            _viewModel.EmpolyeeRepository = _mockEmployeeRepository.Object;
            _viewModel.UserRepository = _mockUserRepository.Object;
            _viewModel.departmentRepository = _mockDepartmentRepository.Object;
            _viewModel.jobRepository = _mockJobRepository.Object;
        }
        [Fact]
        public void ValidationInput_ValidData_ReturnsTrue()
        {
            // Arrange
            var viewModel = new WorkersViewModel(); 
            viewModel.Name = "John";
            viewModel.Surname = "Doe";
            viewModel.SelectedUser = new User(); // Provide a valid user
            viewModel.SelectedItem = null; // No selected item
            viewModel.WorkersRecords = new ObservableCollection<Employee>(); // Provide an empty collection or appropriate data
            viewModel.Jmbg = 1234567890123; // Provide a valid JMBG
            viewModel.Email = "john.doe@example.com";
            viewModel.SelectedDate = DateTime.Today.AddYears(-20); // A date that makes the user older than 10 years

            // Act
            var result = viewModel.ValidationInput();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidationInput_InvalidData_ReturnsFalseAndShowsMessageBox()
        {
            // Arrange
            var viewModel = new WorkersViewModel(); 
            viewModel.Name = null; // Invalid: Name is null
            viewModel.Surname = ""; // Invalid: Surname is empty
            viewModel.SelectedUser = null; // Invalid: No selected user
            viewModel.SelectedItem = null; // No selected item
            viewModel.WorkersRecords = new ObservableCollection<Employee>(); // Provide an empty collection or appropriate data
            viewModel.Jmbg = 1234567890; // Invalid: JMBG length is less than 13
            viewModel.Email = "john.doeexample.com"; // Invalid: Invalid email format
            viewModel.SelectedDate = DateTime.Today.AddYears(-5); // Invalid: User is younger than 10 years

            // Act
            var result = viewModel.ValidationInput();

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ValidationInput_InvalidData_JMBGFalse()
        {
            // Arrange
            var viewModel = new WorkersViewModel();
            viewModel.Name = "TEST"; // Invalid: Name is null
            viewModel.Surname = "TEST"; // Invalid: Surname is empty
            viewModel.SelectedUser = new User();
            viewModel.SelectedItem = null; // No selected item
            viewModel.WorkersRecords = new ObservableCollection<Employee>(); // Provide an empty collection or appropriate data
            viewModel.Jmbg = 1234567890; // Invalid: JMBG length is less than 13
            viewModel.Email = "john.doeexample.com"; // Invalid: Invalid email format
            viewModel.SelectedDate = DateTime.Today.AddYears(-5); // Invalid: User is younger than 10 years

            // Act
            var result = viewModel.ValidationInput();

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ValidationInput_InvalidData_EmailFormatNotGood()
        {
            // Arrange
            var viewModel = new WorkersViewModel();
            viewModel.Name = "TEST"; // Invalid: Name is null
            viewModel.Surname = "TEST"; // Invalid: Surname is empty
            viewModel.SelectedUser = new User();
            viewModel.SelectedItem = null; // No selected item
            viewModel.WorkersRecords = new ObservableCollection<Employee>(); // Provide an empty collection or appropriate data
            viewModel.Jmbg = 1234567890123; // Invalid: JMBG length is less than 13
            viewModel.Email = "john.doeexample.com"; // Invalid: Invalid email format
            viewModel.SelectedDate = DateTime.Today.AddYears(-5); // Invalid: User is younger than 10 years

            // Act
            var result = viewModel.ValidationInput();

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ValidationInput_InvalidData_DateNotGood()
        {
            // Arrange
            var viewModel = new WorkersViewModel();
            viewModel.Name = "TEST";
            viewModel.Surname = "TEST"; 
            viewModel.SelectedUser = new User();
            viewModel.SelectedItem = null; 
            viewModel.WorkersRecords = new ObservableCollection<Employee>(); 
            viewModel.Jmbg = 1234567890123; 
            viewModel.Email = "john.doe@example.com"; 
            viewModel.SelectedDate = DateTime.Today.AddYears(-5); 

            // Act
            var result = viewModel.ValidationInput();

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void PopulateUpdateWindow_PopulatesPropertiesCorrectly()
        {
            // Arrange
            var viewModel = new WorkersViewModel();
            var selectedEmployee = new Employee
            {
                Id = 1,
                UserId = 1,
                DepartmentId = 1,
                JobId = 1,
                Name = "John",
                Surname = "Doe",
                Birthday = new DateOnly(1990, 1, 1),
                Jmbg = 1234567890123,
                Address = "123 Main St",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            };
            var users = new ObservableCollection<User>
            {
                new User { Id = 1, Username = "John" }
            };
            var departments = new ObservableCollection<Department>
            {
                new Department { Id = 1, Name = "IT" }
            };
            var jobs = new ObservableCollection<Job>
            {
                new Job { Id = 1, Name = "Software Developer" }
            };
            viewModel.UsersRecords = users;
            viewModel.DepartmentRecords = departments;
            viewModel.JobRecords = jobs;
            viewModel.SelectedItem = selectedEmployee;

            // Act
            viewModel.PopulateUpdateWindow();

            // Assert
            Assert.Equal(selectedEmployee.Id, viewModel.Id);
            Assert.Equal(users.Single(), viewModel.SelectedUser);
            Assert.Equal(departments.Single(), viewModel.SelectedDepartment);
            Assert.Equal(jobs.Single(), viewModel.SelectedJob);
            Assert.Equal(selectedEmployee.Name, viewModel.Name);
            Assert.Equal(selectedEmployee.Surname, viewModel.Surname);
            Assert.Equal(selectedEmployee.Jmbg, viewModel.Jmbg);
            Assert.Equal(selectedEmployee.Address, viewModel.Address);
            Assert.Equal(selectedEmployee.Email, viewModel.Email);
            Assert.Equal(selectedEmployee.Phone, viewModel.Phone);
        }
        [Fact]
        public void WorkersViewModel_Initialization()
        {
            Assert.NotNull(_viewModel.WorkersRecords);
            Assert.NotNull(_viewModel.UsersRecords);
            Assert.NotNull(_viewModel.DepartmentRecords);
            Assert.NotNull(_viewModel.JobRecords);
            Assert.Equal(DateTime.Today, _viewModel.SelectedDate);
        }


    }
}
