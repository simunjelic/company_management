using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    class UserViewModel : ViewModelBase
    {
        UserRepository UserRepository { get; set; }
        public ICommand ShowUserRolesWindowCommand { get;}
        public ICommand DeleteRoleCommand { get;}
        public ICommand AddRoleCommand { get;}
        public string ModuleName = "Korisnici";

        public UserViewModel()
        {
            UserRepository = new UserRepository();
            GetAllUsersAsync();
            ShowUserRolesWindowCommand = new ViewModelCommand(ShowUserRoles, CanShowUserRoles);
            DeleteRoleCommand = new ViewModelCommand(DeleteRole,CanDeleteRole);
            AddRoleCommand = new ViewModelCommand(AddRole, CanAddRole);
        }

       

        private bool CanAddRole(object obj)
        {
            if (SelectedRole != null && CanCreatePermission(ModuleName)) return true;
            return false;
        }
        private async void AddRole(object obj)
        {
            if(UserRolesRecords.FirstOrDefault(i => i.Role.RoleName == SelectedRole.RoleName) == null)
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
                        
                            GetUserRoles(SelectedItem.Id);
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
            if(SelectedUserRole != null  && CanReadPermission(ModuleName)) return true;
            return false;
        }

        private async void DeleteRole(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati korisniku "+ SelectedUserRole.User.Username + " ulogu: " + SelectedUserRole.Role.RoleName + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
    }
}
