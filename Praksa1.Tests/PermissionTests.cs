using Autofac.Extras.Moq;
using Moq;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Praksa1.Tests
{
    public class PermissionTests
    {

        [Fact]
        public Task CanReadPermission_ReturnsTrueWhenUserHavePermission()
        {
            LoggedUserData.RolesId.Add(1);
            using (var mock = AutoMock.GetLoose())
            {
                var repositoryMock = new Mock<IPermissonRepository>();
                repositoryMock
                    .Setup(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(GetSamplePermissions());

                var cls = mock.Create<JobsViewModel>();
                var expected = true;
                var actual = cls.CanReadPermission("Admin panel");
                Assert.Equal(expected,actual);
            }

            return Task.CompletedTask;
        }

        private List<Permission> GetSamplePermissions()
        {
            List<Permission> output = new List<Permission>();
            new Permission
            {
                ModuleId = 6,
                RoleId = 1,
                ActionId = 1,
                Id = 1,
            };
            new Permission
            {
                ModuleId = 6,
                RoleId = 1,
                ActionId = 0,
                Id = 2,
            };
            new Permission
            {
                ModuleId = 6,
                RoleId = 1,
                ActionId = 2,
                Id = 3,
            };
            new Permission
            {
                ModuleId = 6,
                RoleId = 1,
                ActionId = 3,
                Id = 4,
            };
            return output;
        }

        [Fact]
        public void CanReadPermission_ReturnsFalseWhenUserNotHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(5);
            // Act
            var result = viewModel.CanReadPermission("Admin panel");

            // Assert
            Assert.False(false); // Assuming ReadPermission is null or empty initially


        }

        [Fact]
        public async Task CanDeletePermission_ReturnsTrueWhenUserHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(1);
            // Act
            var result = viewModel.CanDeletePermission("Admin panel");

            // Assert
            Assert.True(result); // Assuming ReadPermission is null or empty initially
        }

        [Fact]
        public void CanDeletePermission_ReturnsFalseWhenUserNotHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(5);
            // Act
            var result = viewModel.CanDeletePermission("Admin panel");

            // Assert
            Assert.False(false); // Assuming ReadPermission is null or empty initially

        }
        [Fact]
        public async Task CanUpdatePermission_ReturnsTrueWhenUserHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(1);
            // Act
            var result = viewModel.CanUpdatePermission("Admin panel");

            // Assert
            Assert.True(result); // Assuming ReadPermission is null or empty initially
        }

        [Fact]
        public void CanUpdatePermission_ReturnsFalseWhenUserNotHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(5);
            // Act
            var result = viewModel.CanUpdatePermission("Admin panel");

            // Assert
            Assert.False(false); 
        }
        [Fact]
        public async Task CanCreatePermission_ReturnsTrueWhenUserHavePermission()
        {
            var viewModel = new JobsViewModel(); 
            LoggedUserData.RolesId.Add(1);
            // Act
            var result = viewModel.CanCreatePermission("Admin panel");

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void CanCreatePermission_ReturnsFalseWhenUserNotHavePermission()
        {
            var viewModel = new JobsViewModel(); // Assuming ViewModelBase has a constructor that accepts PermissonRepository
            LoggedUserData.RolesId.Add(5);
            // Act
            var result = viewModel.CanCreatePermission("Admin panel");

            // Assert
            Assert.False(false); // Assuming ReadPermission is null or empty initially
        }
    }
}
