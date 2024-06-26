﻿using Praksa_projectV1.Commands;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private UserRepository userRepository;

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        //-> Commands
        public IAsyncCommand LoginCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        //Constructor
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new AsyncCommand(ExecuteLoginCommandAsync, CanExecuteLoginCommandAsync);
        }

        private bool CanExecuteLoginCommandAsync()
        {
            return true;
        }

        public async Task ExecuteLoginCommandAsync()
        {
            try
            {
                if (!(string.IsNullOrWhiteSpace(Username) || Username.Length < 1 ||
                Password == null || Password.Length < 1))
                {
                    var isValidUser = await userRepository.AuthenticateUserAsync(new System.Net.NetworkCredential(Username, Password));
                    if (isValidUser != null)
                    {
                        LoggedUserData.Username = Username;
                        LoggedUserData.Id = isValidUser.Id;
                        var list = await userRepository.GetUserRolesAsync(isValidUser.Id);
                        string[] roles = new string[list.Count()];
                        int index = 0;
                        foreach (UserRole item in list)
                        {
                            roles[index++] = item.RoleId.ToString();
                            LoggedUserData.Roles.Add(item.Role.RoleName);
                            LoggedUserData.RolesId.Add(item.RoleId);

                        }
                        // Create a new identity with the username and roles
                        var identity = new GenericIdentity(Username);
                        var principal = new GenericPrincipal(identity, roles);

                        // Set the current principal
                        Thread.CurrentPrincipal = principal;
                        IsViewVisible = false;
                    }
                    else

                        ErrorMessage = "*Neispravno korisničko ime ili lozinka";
                }
                else ErrorMessage = "*Unesi ime i lozinka";
            }
            catch (Exception ex) 
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
            }

        }

    }

}
