using Autofac.Extras.Moq;
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
using System.Windows;
using System.Windows.Controls;

namespace Praksa1.Tests
{
    public class ProjectsTests
    {
        [Fact]
        public async Task AddProjectAsync_ValidData_ProjectAddedSuccessfully()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeRepository = new Mock<IProjectRepository>();
                var cls = mock.Create<ProjectsViewModel>();

                // Set up input data
                cls.Location = new Location { Id = 1, Name = "Alfa Therm" };
                cls.Type = new Praksa_projectV1.Models.Type { Id = 1 , Name = "Projektiranje"};
                cls.Name = "Novi projekt";
                cls.ProjectRecords = new System.Collections.ObjectModel.ObservableCollection<Project>();
                var expected = cls.ProjectRecords.Count();

                // Set up the repository to return true for successful addition
                fakeRepository.Setup(repo => repo.AddAsync(It.IsAny<Project>())).ReturnsAsync(true);

                cls.ProjectRepository = fakeRepository.Object;

                // Act
                await cls.AddProjectAsync();

                // Assert
                // Verify that the repository's AddAsync method was called with the expected project data
                //fakeRepository.Verify(repo => repo.AddAsync(It.IsAny<Project>()));

                // Verify that the project was added to the ViewModel's ProjectRecords collection
                Assert.Contains(cls.ProjectRecords, p => p.Name == "Novi projekt");
            }
        }
        [Fact]
        public void ValidationData_ValidData_ReturnsTrue()
        {
            // Arrange
            var viewModel = new ProjectsViewModel();
            viewModel.Name = "Test Project";
            viewModel.StartDate = new DateTime(2021, 1, 1);
            viewModel.EndDate = new DateTime(2022, 1, 1);

            // Act
            var result = viewModel.ValidationData();

            // Assert
            Assert.True(result);
            // Ensure that no message box is shown
        }
        [Fact]
        public void ValidationData_ValidData_ReturnsFalse()
        {
            // Arrange
            var viewModel = new ProjectsViewModel();
            viewModel.Name = "Test Project";
            viewModel.StartDate = new DateTime(2021, 1, 1);
            viewModel.EndDate = new DateTime(2020, 1, 1);

            // Act
            var result = viewModel.ValidationData();

            // Assert
            Assert.False(result);
            // Ensure that no message box is shown
        }
        [Fact]
        public async Task DeleteProjectAsync_ValidData_ProjectDeletedSuccessfully()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeRepository = new Mock<IProjectRepository>();
                var cls = mock.Create<ProjectsViewModel>();

                // Set up input data
                cls.SelectedItem = new Project { Id = 1, Name = "Test Project" };
                cls.ProjectRecords = new ObservableCollection<Project> { cls.SelectedItem };
                var expected = cls.ProjectRecords.Count();

                // Set up the repository to return true for successful deletion
                fakeRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Project>())).ReturnsAsync(true);
                cls.ProjectRepository = fakeRepository.Object;

                // Act
                await cls.DeleteProjectAsync();

                // Assert

                // Verify that the project was removed from the ViewModel's ProjectRecords collection
                Assert.DoesNotContain(cls.ProjectRecords, p => p.Name == "Test Project");
            }
        }
        [Fact]
        public async Task UpdateProjectAsync_ValidData_ProjectUpdatedSuccessfully()
        {
            // Arrange
            var fakeRepository = new Mock<IProjectRepository>();
            var viewModel = new ProjectsViewModel();

            // Set up input data
            viewModel.Location = new Location { Id = 1, Name = "Alfa Therm" };
            viewModel.Type = new Praksa_projectV1.Models.Type { Id = 1, Name = "Projektiranje" };
            viewModel.Name = "Prvi projekt";
            viewModel.ProjectRecords = new ObservableCollection<Project>()
    {
                new Project { Id = 1, Name = "Old Project", Location = new Location { Id = 1, Name = "Old Location" }, Type = new Praksa_projectV1.Models.Type { Id = 1, Name = "Old Type" } }
    };

            viewModel.Id = 1;

            // Set up the repository to return true for successful update
            fakeRepository.Setup(repo => repo.UpdateProjectAsync(It.IsAny<Project>())).ReturnsAsync(true);
            viewModel.ProjectRepository = fakeRepository.Object;

            // Act
            await viewModel.UpdateProjectAsync();

            // Assert

            // Verify that the project was updated in the ViewModel's ProjectRecords collection
            Assert.Contains(viewModel.ProjectRecords, p => p.Name == "Prvi projekt");
            Assert.DoesNotContain(viewModel.ProjectRecords, p => p.Name == "Old Project");
        }



    }
}
