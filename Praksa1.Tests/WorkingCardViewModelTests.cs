using Moq;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Praksa1.Tests
{

    public class WorkingCardViewModelTests
    {
        private readonly Mock<IWorkingCardRepository> _mockCardRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly WorkingCardViewModel _viewModel;

        public WorkingCardViewModelTests()
        {
            _mockCardRepository = new Mock<IWorkingCardRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();

            _viewModel = new WorkingCardViewModel(isTest: true);
            _viewModel.cardRespository = _mockCardRepository.Object;
            _viewModel.userRepository = _mockUserRepository.Object;
            _viewModel.ProjectRepository = _mockProjectRepository.Object;
            
        }

        [Fact]
        public async Task AddAsync_ShouldAddWorkingCard_WhenValid()
        {
            // Arrange
            _viewModel.SelectedProject = new Project { Id = 1 };
            _viewModel.SelectedActivity = new Activity { Id = 1 };
            _viewModel.Hours = "8";
            _viewModel.Description = "Test description";
            _viewModel.SelectedDate = DateTime.Today;
            LoggedUserData.Username = "testuser";

            var employee = new Employee { Id = 1, UserId = 1};
            _mockUserRepository.Setup(repo => repo.getEmployeeByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(employee);

            _mockCardRepository.Setup(repo => repo.Add(It.IsAny<WorkingCard>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.AddAsync();

            // Assert
            _mockCardRepository.Verify(repo => repo.Add(It.IsAny<WorkingCard>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRecordAsync_ShouldUpdateWorkingCard_WhenValid()
        {
            // Arrange
            _viewModel.SelectedItem = new WorkingCard { EmployeeId = 1, Id = 1, ProjectId = 1, ActivityId = 1, Hours = 8, Description = "Test", Date = new DateOnly(2023, 5, 14) };
            _viewModel.SelectedProject = new Project { Id = 1 };
            _viewModel.SelectedActivity = new Activity { Id = 1 };
            _viewModel.Hours = "8";
            _viewModel.Description = "Updated description";
            _viewModel.SelectedDate = DateTime.Today;

            _mockCardRepository.Setup(repo => repo.EditAsync(It.IsAny<WorkingCard>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.UpdateRecordAsync();

            // Assert
            _mockCardRepository.Verify(repo => repo.EditAsync(It.IsAny<WorkingCard>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteWorkingCard_WhenSelectedItemIsNotNull()
        {
            // Arrange
            var selectedItem = new WorkingCard { Id = 1, Activity = new Activity { Name = "Test Activity" }, Date = new DateOnly(2023, 5, 14) };
            _viewModel.SelectedItem = selectedItem;

            _mockCardRepository.Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.DeleteAsync();

            // Assert
            _mockCardRepository.Verify(repo => repo.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
        }

        // Additional tests can be added here to cover other scenarios and methods
    }
}

