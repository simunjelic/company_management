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

namespace Praksa1.Tests
{
    public class DepartmentTests
    {
        [Fact]
        public async Task AddDepartmentAsync_ValidData_DepartmentAdded()
        {
            // Arrange
            var fakeRepository = new Mock<IDepartmentRepository>();
            fakeRepository.Setup(repo => repo.AddAsync(It.IsAny<Department>())).ReturnsAsync(true);

            var viewModel = new DepartmentsViewModel();
            viewModel.IdepartmentRepository = fakeRepository.Object;
            viewModel.Name = "Test Department";
            viewModel.DepartmentRecords = new ObservableCollection<Department> { new Department() };

            // Act
            await viewModel.AddDepartmentAsync();

            // Assert
            Assert.Contains(viewModel.DepartmentRecords, d => d.Name == "Test Department");
        }
        [Fact]
        public async Task AddDepartmentAsync_ItemWithSameName_DepartmentNotAdded()
        {
            // Arrange
            var fakeRepository = new Mock<IDepartmentRepository>();
            var departments = new List<Department>
    {
        new Department { Id = 1, Name = "Department 1" },
        new Department { Id = 2, Name = "Department 2" }
    };
            fakeRepository.Setup(repo => repo.GetAllDepartmentsAsync()).ReturnsAsync(departments);


            var viewModel = new DepartmentsViewModel();
            viewModel.IdepartmentRepository = fakeRepository.Object;
            viewModel.Name = "Department 1";
            await viewModel.GetAllDepartmentsAsync();
            var expected = viewModel.DepartmentRecords.Count();

            // Act
            await viewModel.AddDepartmentAsync();


            await viewModel.GetAllDepartmentsAsync();
            var actual = viewModel.DepartmentRecords.Count();

            // Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public async Task DeleteDepartmentAsync_ItemSelected_DepartmentRemoved()
        {
            // Arrange
            var fakeRepository = new Mock<IDepartmentRepository>();
            var viewModel = new DepartmentsViewModel();
            viewModel.IdepartmentRepository = fakeRepository.Object;

            // Create a department to be selected
            var departmentToDelete = new Department { Id = 1, Name = "DepartmentToDelete" };
            viewModel.DepartmentRecords = new ObservableCollection<Department> { departmentToDelete };
            viewModel.SelectedItem = departmentToDelete;

            // Configure the repository mock to return true for successful deletion
            fakeRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Department>())).ReturnsAsync(true);

            // Act
            await viewModel.DeleteDepartmentAsync();

            // Assert
            Assert.DoesNotContain(departmentToDelete, viewModel.DepartmentRecords);
        }
        [Fact]
        public async Task UpdateDepartmentAsync_ValidData_DepartmentUpdated()
        {
            // Arrange
            var fakeRepository = new Mock<IDepartmentRepository>();
            var viewModel = new DepartmentsViewModel();
            viewModel.IdepartmentRepository = fakeRepository.Object;    

            var departments = new List<Department>
    {
        new Department { Id = 4, Name = "Servis" },
        new Department { Id = 5, Name = "Montaza" }
    };

            // Set up repository to return the list of departments
            fakeRepository.Setup(repo => repo.GetAllDepartmentsAsync()).ReturnsAsync(departments);

            // Create a department to be updated
            var departmentToUpdate = departments.First();
            viewModel.DepartmentRecords = new ObservableCollection<Department>(departments);
            viewModel.SelectedDepartment = departmentToUpdate;
            viewModel.Id = departmentToUpdate.Id;
            viewModel.Name = "Updated Department";

            // Configure the repository mock to return true for successful update
            fakeRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Department>())).ReturnsAsync(true);

            // Act
            await viewModel.UpdateDepartmentAsync();

            // Assert
            Assert.Contains(viewModel.DepartmentRecords, d => d.Name == "Updated Department");
        }


        [Fact]
        public async Task UpdateDepartmentAsync_DepartmentNameExists_ShowMessageBox()
        {
            // Arrange
            var fakeRepository = new Mock<IDepartmentRepository>();
            var viewModel = new DepartmentsViewModel();
            viewModel.IdepartmentRepository = fakeRepository.Object;

            // Create a department with the same name as the updated department
            var existingDepartment = new Department { Id = 2, Name = "Servis" };
            viewModel.DepartmentRecords = new ObservableCollection<Department> { existingDepartment };
            viewModel.Name = "Servis";

            // Act
            await viewModel.UpdateDepartmentAsync();

            
            Assert.Contains(viewModel.DepartmentRecords, d => d.Name == "Servis");
        }

    }
}
