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


    }
}
