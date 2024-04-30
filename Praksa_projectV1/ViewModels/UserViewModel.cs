﻿using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Validation;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    class UserViewModel : ViewModelBase
    {
        public readonly string ModuleName = "Korisnici";
        UserRepository UserRepository { get; set; }
        public ICommand ShowUserRolesWindowCommand { get; }
        public ICommand DeleteRoleCommand { get; }
        public ICommand AddRoleCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand ShowUpdatePasswordWindowCommand { get; }
        public ICommand UpdateUserPasswordCommand { get; }




        public UserViewModel()
        {
            UserRepository = new UserRepository();
            GetAllUsersAsync();
            ShowUserRolesWindowCommand = new ViewModelCommand(ShowUserRoles, CanShowUserRoles);
            DeleteRoleCommand = new ViewModelCommand(DeleteRole, CanDeleteRole);
            AddRoleCommand = new ViewModelCommand(AddRole, CanAddRole);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddUserCommand = new ViewModelCommand(AddUser, CanAddUser);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateUserCommand = new ViewModelCommand(UpdateUser, CanUpdateUser);
            ShowUpdatePasswordWindowCommand = new ViewModelCommand(ShowUpdatePasswordWindow, CanShowUpdatePasswordWindow);
            UpdateUserPasswordCommand = new ViewModelCommand(UpdateUserPassword, CanUpdateUserPassword);
        }

        private bool CanUpdateUserPassword(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null) && IsSecureStringLongerThan6(Password);
        }

        private async void UpdateUserPassword(object obj)
        {
            if (SecureStringEquals(Password, CheckPassword))
            {
                if (!UsersRecords.Any(i => i.Username == Username && i.Id != Id))
                {

                    var check = await UserRepository.EditUser(new System.Net.NetworkCredential(Username, Password), Id);
                    if (check)
                    {
                        MessageBox.Show("Lozinka uređeno.");
                        GetAllUsersAsync();
                        ResetData();
                    }
                    else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
                }
                else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
            } else MessageBox.Show("Lozinke nisu jednake.");
        }

        private bool CanShowUpdatePasswordWindow(object obj)
        {
            return CanUpdatePermission(ModuleName);
        }

        private async void ShowUpdatePasswordWindow(object obj)
        {
            UserAddView userEditView = new UserAddView();
            userEditView.DataContext = this;
            userEditView.Title = "Uredi lozinku";
            IsAddButtonVisible = false;
            IsUpdateButtonVisible = true;
            Password = null;
            CheckPassword = null;
            Username = SelectedItem.Username;
            Id = SelectedItem.Id;

            userEditView.Show();
        }

        private bool CanUpdateUser(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private async void UpdateUser(object obj)
        {
            if (!UsersRecords.Any(i => i.Username == Username && i.Id != Id))
            {

                var check = await UserRepository.EditUserUsername(Username,Id);
                if (check)
                {
                    MessageBox.Show("Korisničko ime uređeno.");
                    GetAllUsersAsync();
                    ResetData();
                }
                else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
            }
            else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
        }

        private bool CanShowUpdateWindow(object obj)
        {
            return CanUpdatePermission(ModuleName) && SelectedItem != null;
        }

        private void ShowUpdateWindow(object obj)
        {
            UserChangeUsernameView userEditView = new UserChangeUsernameView();
            userEditView.DataContext = this;
            userEditView.Title = "Uredi koriničko ime";
            IsAddButtonVisible = false;
            IsUpdateButtonVisible = true;
            Username = SelectedItem.Username;
            Id = SelectedItem.Id;
            
            userEditView.Show();
        }

        private bool CanAddUser(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null) && IsSecureStringLongerThan6(Password);
        }

        

        private async void AddUser(object obj)
        {
            if (SecureStringEquals(Password, CheckPassword))
                {

                var check = await UserRepository.AddUser(new System.Net.NetworkCredential(Username, Password));
                if(check)
                {
                    MessageBox.Show("Korisnik dodan");
                    GetAllUsersAsync();
                }else  MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
            } else MessageBox.Show("Lozinke nisu jednake.");
        }

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {
            UserAddView userEditView = new UserAddView();
            userEditView.DataContext = this;
            userEditView.Title = "Dodaj korisnika";
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            ResetData();
            userEditView.Show();
        }

        private void ResetData()
        {
            Username = string.Empty;
            Password = null;
            CheckPassword = null;
            SelectedItem = null;
        }

        private bool CanAddRole(object obj)
        {
            if (SelectedRole != null && CanCreatePermission(ModuleName)) return true;
            return false;
        }
        private async void AddRole(object obj)
        {
            if (UserRolesRecords.FirstOrDefault(i => i.Role.RoleName == SelectedRole.RoleName) == null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite dodati novu ulogu: " + SelectedRole.RoleName, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UserRole newUserRole = new UserRole();
                    newUserRole.RoleId = SelectedRole.Id;
                    newUserRole.UserId = SelectedItem.Id;
                    bool check = await UserRepository.AddUserRole(newUserRole);
                    if (check)
                    {

                        //GetUserRoles(SelectedItem.Id);
                        UserRolesRecords.Remove(newUserRole);

                        //GetAllUsersAsync();
                        MessageBox.Show("Nova uloga dodana");




                    }
                    else MessageBox.Show("Greška pri dodavanu nove uloge.");
                }

            }
            else MessageBox.Show("Korisnik već ima odabranu ulogu.");

        }

        private bool CanDeleteRole(object obj)
        {
            if (SelectedUserRole != null && CanReadPermission(ModuleName)) return true;
            return false;
        }

        private async void DeleteRole(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati korisniku " + SelectedUserRole.User.Username + " ulogu: " + SelectedUserRole.Role.RoleName + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool check = await UserRepository.RemoveUserRole(SelectedUserRole);
                if (check)
                {

                    UserRolesRecords.Remove(SelectedUserRole);
                    MessageBox.Show("Uloga uspješno obrisana");
                    //GetAllUsersAsync();
                }
                else MessageBox.Show("Nije moguće obrisati ulogu");


                SelectedUserRole = null;
            }
        }

        private bool CanShowUserRoles(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void ShowUserRoles(object obj)
        {
            UserRolesView userRolesView = new UserRolesView();
            GetUserRoles(SelectedItem.Id);
            GetRoles();
            userRolesView.DataContext = this;
            userRolesView.Title = "Uloge";
            userRolesView.Show();



        }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                FilterData();
            }
        }

        private async void FilterData()
        {
            UsersRecords = new ObservableCollection<User>(await UserRepository.FilterData(SearchQuery));
        }
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private ObservableCollection<User> _usersRecords;
        public ObservableCollection<User> UsersRecords
        {
            get
            {
                return _usersRecords;
            }
            set
            {
                _usersRecords = value;
                OnPropertyChanged(nameof(UsersRecords));
            }
        }
        private ObservableCollection<UserRole> _userRolesRecords;
        public ObservableCollection<UserRole> UserRolesRecords
        {
            get
            {
                return _userRolesRecords;
            }
            set
            {
                _userRolesRecords = value;
                OnPropertyChanged(nameof(UserRolesRecords));
            }
        }

        private UserRole _selectedUserRole;
        public UserRole SelectedUserRole
        {
            get
            {
                return _selectedUserRole;
            }
            set
            {
                _selectedUserRole = value;
                OnPropertyChanged(nameof(SelectedUserRole));
            }
        }


        private ObservableCollection<User> _usersShowRecords;
        public ObservableCollection<User> UsersShowRecords
        {
            get
            {
                return _usersShowRecords;
            }
            set
            {
                _usersShowRecords = value;
                OnPropertyChanged(nameof(UsersShowRecords));
            }
        }
        private ObservableCollection<Role> _roleRecords;
        public ObservableCollection<Role> RoleRecords
        {
            get
            {
                return _roleRecords;
            }
            set
            {
                _roleRecords = value;
                OnPropertyChanged(nameof(RoleRecords));
            }
        }
        private User _selectedItem;
        public User? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        private Role _selectedRole;
        public Role? SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                if (_selectedRole != value)
                {
                    _selectedRole = value;
                    OnPropertyChanged(nameof(SelectedRole));
                }
            }
        }
        private string _username;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string Username
        {
            get { return _username; }

            set
            {
                if (_username != value)
                {
                    _username = value;
                    Validate(nameof(Username), value);
                    OnPropertyChanged(nameof(Username));
                }
            }
        }
        private SecureString _password = new SecureString();
        [SecureStringAttribute(6, ErrorMessage = "Polje mora sadržavati barem 6 znakova.")]
        public SecureString? Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                Validate(nameof(Password), value);
                OnPropertyChanged(nameof(Password));
            }
        }
        private SecureString _checkPassword = new SecureString();
        [SecureStringAttribute(6, ErrorMessage = "Polje mora sadržavati barem 6 znakova.")]
        public SecureString? CheckPassword
        {
            get
            {
                return _checkPassword;
            }
            set
            {
                _checkPassword = value;
                Validate(nameof(CheckPassword), value);
                OnPropertyChanged(nameof(CheckPassword));
            }
        }

        public async void GetAllUsersAsync()
        {
            var users = await UserRepository.getAllUsersAsync();
            UsersRecords = new ObservableCollection<User>(users);
            UsersShowRecords = new ObservableCollection<User>(users);
        }
        public async Task GetUserRoles(int id)
        {

            IEnumerable<UserRole> team = await UserRepository.GetUserRolesObject(id);
            UserRolesRecords = new ObservableCollection<UserRole>(team);
        }
        public async Task GetRoles()
        {

            IEnumerable<Role> team = await UserRepository.GetRoles();
            RoleRecords = new ObservableCollection<Role>(team);
        }
        private bool SecureStringEquals(SecureString secureString1, SecureString secureString2)
        {
            if (secureString1 == null)
            {
                throw new ArgumentNullException("s1");
            }
            if (secureString2 == null)
            {
                throw new ArgumentNullException("s2");
            }

            if (secureString1.Length != secureString2.Length)
            {
                return false;
            }

            IntPtr ss_bstr1_ptr = IntPtr.Zero;
            IntPtr ss_bstr2_ptr = IntPtr.Zero;

            try
            {
                ss_bstr1_ptr = Marshal.SecureStringToBSTR(secureString1);
                ss_bstr2_ptr = Marshal.SecureStringToBSTR(secureString2);

                String str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                String str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                return str1.Equals(str2);
            }
            finally
            {
                if (ss_bstr1_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                }

                if (ss_bstr2_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                }
            }
        }
        private bool IsSecureStringLongerThan6(SecureString secureString)
        {
            if (secureString == null)
            {
                return false; // SecureString is null, cannot be longer than 6 characters
            }

            IntPtr ptr = IntPtr.Zero;
            try
            {
                // Convert SecureString to plain text
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                string plainText = Marshal.PtrToStringUni(ptr);

                // Check if plain text is longer than 6 characters
                return plainText.Length >= 6;
            }
            finally
            {
                // Free allocated memory
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }
        public static SecureString ConvertToSecureString(string str)
        {
            SecureString secureStr = new SecureString();
            foreach (char c in str)
            {
                secureStr.AppendChar(c);
            }
            secureStr.MakeReadOnly(); // Make the SecureString read-only for security
            return secureStr;
        }
    }
}
