using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Praksa1.Tests
{
    public class LoginTests
    {
        [Fact]
        public async Task ExecuteLoginCommandAsync_ValidCredentials_ShouldAuthenticateUser()
        {
            // Arrange
            var viewModel = new Praksa_projectV1.ViewModels.LoginViewModel();
            LoggedUserData.Username = string.Empty;
            viewModel.Username = "admin";
            viewModel.Password = CreateSecureString("admin");

            // Act
            await viewModel.ExecuteLoginCommandAsync();

            // Assert
            Assert.NotEmpty(LoggedUserData.Username);
            Assert.False(viewModel.IsViewVisible);
            Assert.Null(viewModel.ErrorMessage);
            // Add more assertions as needed
        }

        [Fact]
        public async Task ExecuteLoginCommandAsync_InvalidCredentials_ShouldShowErrorMessage()
        {
            // Arrange
            var viewModel = new Praksa_projectV1.ViewModels.LoginViewModel();
            LoggedUserData.Username = string.Empty;
            viewModel.Username = "invalidUsername";
            viewModel.Password = CreateSecureString("invalidPassword");

            // Act
            await viewModel.ExecuteLoginCommandAsync();

            // Assert
            Assert.Empty(LoggedUserData.Username);
            Assert.True(viewModel.IsViewVisible);
            Assert.NotNull(viewModel.ErrorMessage);
            // Add more assertions as needed
        }
        [Fact]
        public async Task ExecuteLoginCommandAsync_InvalidData_ShouldShowErrorMessage()
        {
            // Arrange
            var viewModel = new Praksa_projectV1.ViewModels.LoginViewModel();
            LoggedUserData.Username = string.Empty;
            viewModel.Username = "";
            viewModel.Password = CreateSecureString("");

            // Act
            await viewModel.ExecuteLoginCommandAsync();

            // Assert
            Assert.Empty(LoggedUserData.Username);
            Assert.True(viewModel.IsViewVisible);
            Assert.NotNull(viewModel.ErrorMessage);
            // Add more assertions as needed
        }

        private SecureString CreateSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (char c in value)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
    }
}
