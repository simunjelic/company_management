using Moq;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Enums;
using Praksa_projectV1.Models;
using System.Collections.ObjectModel;
using Praksa_projectV1.ViewModels;
using Xunit;
namespace Praksa1.Tests
{
    public class AdminPanelTests
    {
        private readonly Mock<IPermissonRepository> _mockPermissonRepository;
        private readonly AdminPanelViewModel _viewModel;

        public AdminPanelTests()
        {
            _mockPermissonRepository = new Mock<IPermissonRepository>();
            _viewModel = new AdminPanelViewModel();
            _viewModel.permissonRepository = _mockPermissonRepository.Object;
        }

        [Fact]
        public async Task GetAllPermissionsAsync_ShouldPopulatePermissionRecords()
        {
            // Arrange
            var permissions = new List<Permission>
        {
            new Permission { Id = 1, ModuleId = 1, RoleId = 1, ActionId = 1 }
        };
            _mockPermissonRepository.Setup(repo => repo.GetAllPermissions()).ReturnsAsync(permissions);

            // Act
            await _viewModel.GetAllPermissionsAsync();

            // Assert
            Assert.NotEmpty(_viewModel.PermissionRecords);
            Assert.Equal(permissions.Count, _viewModel.PermissionRecords.Count);
            Assert.Equal(permissions.First().Id, _viewModel.PermissionRecords.First().Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewPermission_WhenValid()
        {
            // Arrange
            var permission = new Permission { ModuleId = 6, RoleId = 1, ActionId = 0};
            _viewModel.Module = new Module { Id = 6, Name = "Test Module" };
            _viewModel.Role = new Role { Id = 1, RoleName = "Test Role" };
            _viewModel.AvailableAction = AvailableActions.Dodaj;

            _mockPermissonRepository.Setup(repo => repo.AddAsync(It.IsAny<Permission>())).ReturnsAsync(true);

            // Ensure PermissionRecords is initialized
            if (_viewModel.PermissionRecords == null)
            {
                _viewModel.PermissionRecords = new ObservableCollection<Permission>();
            }

            // Act
            await _viewModel.AddAsync();

            // Assert
            Assert.Contains(_viewModel.PermissionRecords, p => p.ModuleId == permission.ModuleId && p.RoleId == permission.RoleId && p.ActionId == permission.ActionId);
        }


        [Fact]
        public async Task DeleteAsync_ShouldRemovePermission_WhenValid()
        {
            // Arrange
            var permission = new Permission { Id = 1, ModuleId = 1, RoleId = 1, ActionId = 1, Module = new Module { Name = "Test Module" }, Role = new Role { RoleName = "Test Role" }, Action = "Create" };
            _viewModel.PermissionRecords = new ObservableCollection<Permission> { permission };
            _viewModel.SelectedItem = permission;

            _mockPermissonRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Permission>())).ReturnsAsync(true);

            // Act
            await _viewModel.DeleteAsync();

            // Assert
            _mockPermissonRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Permission>()), Times.Once);
            Assert.DoesNotContain(_viewModel.PermissionRecords, p => p.Id == permission.Id);
        }
    }
}
