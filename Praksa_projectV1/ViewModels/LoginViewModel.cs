using Praksa_projectV1.DataAccess;
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
    public class LoginViewModel: ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private IUserRepository userRepository;

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
        public ICommand LoginCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        //Constructor
        public LoginViewModel()
        {
            userRepository=new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 ||
                Password == null || Password.Length < 3)
                validData = false;
            else
                validData = true;
            return validData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            try { 
            var isValidUser = userRepository.AuthenticateUser(new System.Net.NetworkCredential(Username, Password));
            if(isValidUser)
            {
                var list = userRepository.GetUserRoles(Username).ToList();
                string[] array = new string[list.Count];
                int index = 0;
                foreach (string item in list)
                {
                    array[index++] = item;
                   
                }
                // Create a new identity with the username and roles
                var identity = new GenericIdentity(Username);
                var principal = new GenericPrincipal(identity, array);

                // Set the current principal
                Thread.CurrentPrincipal = principal;
                IsViewVisible = false;
            }else
            {
                ErrorMessage = "*Neispravno korisničko ime ili lozinka";
            }
            }catch (Exception e)
            {
                
            }
        }
    }
    
}
