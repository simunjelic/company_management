using Praksa_projectV1.Commands;
using Praksa_projectV1.DataAccess;
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
        public IAsyncCommand ShowUserRolesWindowCommand { get; }
        public IAsyncCommand DeleteRoleCommand { get; }
        public IAsyncCommand AddRoleCommand { get; }
        public IAsyncCommand ShowAddWindowCommand { get; }
        public IAsyncCommand AddUserCommand { get; }
        public IAsyncCommand ShowUpdateWindowCommand { get; }
        public IAsyncCommand UpdateUserCommand { get; }
        public IAsyncCommand ShowUpdatePasswordWindowCommand { get; }
        public IAsyncCommand UpdateUserPasswordCommand { get; }
        public IAsyncCommand DeleteCommand { get; }




        public UserViewModel()
        {
            UserRepository = new UserRepository();
            GetAllUsersAsync();
            ShowUserRolesWindowCommand = new AsyncCommand(ShowUserRolesAsync, CanShowUserRolesAsync);
            DeleteRoleCommand = new AsyncCommand(DeleteRoleAsync, CanDeleteRoleAsync);
            AddRoleCommand = new AsyncCommand(AddRoleAsync, CanAddRoleAsync);
            ShowAddWindowCommand = new AsyncCommand(ShowAddWindowAsync, CanShowAddWindowAsync);
            AddUserCommand = new AsyncCommand(AddUserAsync, CanAddUserAsync);
            ShowUpdateWindowCommand = new AsyncCommand(ShowUpdateWindowAsync, CanShowUpdateWindowAsync);
            UpdateUserCommand = new AsyncCommand(UpdateUserAsync, CanUpdateUserAsync);
            ShowUpdatePasswordWindowCommand = new AsyncCommand(ShowUpdatePasswordWindowAsync, CanShowUpdatePasswordWindowAsync);
            UpdateUserPasswordCommand = new AsyncCommand(UpdateUserPasswordAsync, CanUpdateUserPasswordAsync);
            DeleteCommand = new AsyncCommand(ExecuteDeleteAsync, CanDeleteAsync);
        }

        private bool CanAddRoleAsync()
        {
            return CanCreatePermissionAsync(ModuleName);
        }

        private async Task AddRoleAsync()
        {
           if(SelectedRole != null)
            {
                if (!UserRolesRecords.Any(i => i.Role.RoleName == SelectedRole.RoleName))
                {
                    var result = MessageBox.Show("Jeste li sigurni da želite dodati korisniku novu ulogu: " + SelectedRole.RoleName, "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        UserRole newUserRole = new UserRole();
                        newUserRole.RoleId = SelectedRole.Id;
                        newUserRole.UserId = SelectedItem.Id;
                        bool check = await UserRepository.AddUserRoleAsync(newUserRole);
                        if (check)
                        {
                            newUserRole.Role = SelectedRole;
                            newUserRole.User = SelectedItem;
                            //GetUserRoles(SelectedItem.Id);
                            UserRolesRecords.Add(newUserRole);
                            SelectedRole = null;


                            //GetAllUsersAsync();
                            MessageBox.Show("Nova uloga dodana korisniku");




                        }
                        else MessageBox.Show("Greška pri dodavanu nove uloge korisniku.");
                    }

                }
                else MessageBox.Show("Korisnik već ima odabranu ulogu.");
            }
        }

        private bool CanDeleteRoleAsync()
        {
            return CanDeletePermission(ModuleName);
        }

        private async Task DeleteRoleAsync()
        {
            if (SelectedUserRole != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati korisniku " + SelectedUserRole.User.Username + " ulogu: " + SelectedUserRole.Role.RoleName + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool check = await UserRepository.RemoveUserRoleAsync(SelectedUserRole);
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
            else MessageBox.Show("Odaberite ulogu za obrisati.");
        }

        private bool CanAddUserAsync()
        {
            return true;
        }

        private async Task AddUserAsync()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null) && IsSecureStringLongerThan6(Password))
            {
                if (SecureStringEquals(Password, CheckPassword))
                {

                    var check = await UserRepository.AddUser(new System.Net.NetworkCredential(Username, Password));
                    if (check)
                    {
                        MessageBox.Show("Korisnik dodan");
                        GetAllUsersAsync();
                        ResetData();
                    }
                    else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
                }
                else MessageBox.Show("Lozinke nisu jednake.");
            }
            else MessageBox.Show("Unesite ispravne podatke");
        }

        private bool CanUpdateUserAsync()
        {
            return true;
        }

        private async Task UpdateUserAsync()
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                if (!UsersRecords.Any(i => i.Username == Username && i.Id != Id))
                {

                    var check = await UserRepository.EditUserUsername(Username, Id);
                    if (check)
                    {
                        MessageBox.Show("Korisničko ime uređeno.");
                        GetAllUsersAsync();
                        ResetData();
                    }
                    else MessageBox.Show("Greška prilikom uređivanja.");
                }
                else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
            }
            else MessageBox.Show("Neispravni podatci! Popunite polja označena crveno.");
        }

        private bool CanDeleteAsync()
        {
            return CanDeletePermission(ModuleName);
        }

        private async Task ExecuteDeleteAsync()
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati korsnika: " + SelectedItem.Username + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool check = await UserRepository.RemoveByIdAsync(SelectedItem.Id);
                    if (check)
                    {
                        UsersRecords.Remove(SelectedItem);
                        MessageBox.Show("Korisnik uspješno obrisan.");
                        ResetData();
                    }
                    else MessageBox.Show("Greška pri brisanju korisnika.");


                    SelectedItem = null;
                }
            }
            else MessageBox.Show("Odaberite korisnika.");
        }

        private bool CanShowUserRolesAsync()
        {
            return true;
        }

        private async Task ShowUserRolesAsync()
        {
            if (SelectedItem != null)
            {
                UserRolesView userRolesView = new UserRolesView();
                GetUserRoles(SelectedItem.Id);
                GetRoles();
                userRolesView.DataContext = this;
                userRolesView.Title = "Uloge";
                userRolesView.Show();
            }
            else MessageBox.Show("Odaberi korisnika.");
        }

        private bool CanShowAddWindowAsync()
        {
            return CanCreatePermissionAsync(ModuleName);
        }

        private async Task ShowAddWindowAsync()
        {
            UserAddView userEditView = new UserAddView();
            userEditView.DataContext = this;
            userEditView.Title = "Dodaj korisnika";
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            ResetData();
            userEditView.Show();
        }

        private bool CanShowUpdateWindowAsync()
        {
            return CanUpdatePermission(ModuleName);
        }

        private async Task ShowUpdateWindowAsync()
        {
            if (SelectedItem != null)
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
            else MessageBox.Show("Odaberite korisnika.");
        }

        private bool CanShowUpdatePasswordWindowAsync()
        {
            return CanUpdatePermission(ModuleName);
        }

        private async Task ShowUpdatePasswordWindowAsync()
        {
            if (SelectedItem != null)
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
            else MessageBox.Show("Odaberite korisnika kojem želite promjeniti šifru");
        }

        private bool CanUpdateUserPasswordAsync()
        {
            return true;
        }

        private async Task UpdateUserPasswordAsync()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null) && IsSecureStringLongerThan6(Password))
            {
                if (SecureStringEquals(Password, CheckPassword))
                {
                    if (!UsersRecords.Any(i => i.Username == Username && i.Id != Id))
                    {

                        var check = await UserRepository.EditUserAsync(new System.Net.NetworkCredential(Username, Password), Id);
                        if (check)
                        {
                            MessageBox.Show("Lozinka uređeno.");
                            GetAllUsersAsync();
                            ResetData();
                        }
                        else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
                    }
                    else MessageBox.Show("Korisnik sa unesenim imenom već postoji.");
                }
                else MessageBox.Show("Lozinke nisu jednake.");

            }
            else MessageBox.Show("Unos nije valjan");
        }

        private void ResetData()
        {
            Username = string.Empty;
            Password = null;
            CheckPassword = null;
            SelectedItem = null;
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
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
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
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
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

        public async Task GetAllUsersAsync()
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
