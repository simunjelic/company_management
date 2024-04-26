using Microsoft.IdentityModel.Tokens;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{

    class RoleViewModel : ViewModelBase
    {
        public ICommand DeleteCommand { get; }
        public string ModuleName = "Uloge";
        public ICommand UpdateCommand { get; }
        public ICommand AddRoleCommand { get; }
        public RoleViewModel()
        {

            PermissonRepository = new PermissonRepository();
            GetAllRoles();
            DeleteCommand = new ViewModelCommand(DeleteRole, CanDeleteRole);
            UpdateCommand = new ViewModelCommand(UpdateRole, CanUpdateRole);
            AddRoleCommand = new ViewModelCommand(AddRole, CanAddRole);


        }

        private bool CanAddRole(object obj)
        {
            if (CanCreatePermission(ModuleName) && !string.IsNullOrWhiteSpace(Role))
                return true;
            return false;
        }

        private async void AddRole(object obj)
        {
            if (RoleRecords.FirstOrDefault(i => i.RoleName == Role) == null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite dodati novu ulogu: " + Role, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Role newRole = new Role();
                    newRole.RoleName = Role;
                    bool check = await PermissonRepository.AddRole(newRole);
                    if (check)
                    {
                        GetAllRoles();
                        MessageBox.Show("Nova uloga dodana");
                        SelectedItem = null;
                        Role = null;
                    }
                    else MessageBox.Show("Greška pri dodavanu nove uloge.");
                }




            }
            else MessageBox.Show("Postoji već dozvola sa imenom " + Role);
        }

        private bool CanUpdateRole(object obj)
        {
            if (SelectedItem != null && CanUpdatePermission(ModuleName) && SelectedItem.RoleName != Role)
                return true;
            return false;
        }

        private async void UpdateRole(object obj)
        {
            if (RoleRecords.FirstOrDefault(i => i.RoleName == Role) == null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite promjeniti naziv iz: " + SelectedItem.RoleName + " u " + Role + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedItem.RoleName = Role;
                    bool check = await PermissonRepository.UpdateRole(SelectedItem);
                    if (check)
                    {
                        GetAllRoles();
                        MessageBox.Show("Naziv promjenjen");

                    }
                    else MessageBox.Show("Greška pri mjenjaju naziva");


                    SelectedItem = null;
                    Role = null;
                }
            }
            else MessageBox.Show("Postoji već dozvola sa imenom " + Role);
        }

        private bool CanDeleteRole(object obj)
        {
            if (SelectedItem != null && CanDeletePermission(ModuleName))
                return true;
            return false;
        }

        private async void DeleteRole(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ulogu: " + SelectedItem.RoleName + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool check = await PermissonRepository.RemoveRole(SelectedItem);
                if (check)
                {
                    RoleRecords.Remove(SelectedItem);
                    MessageBox.Show("Uloga uspješno obrisana");
                }
                else MessageBox.Show("Nije moguće obrisati ulogu");


                SelectedItem = null;
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
        private Role _selectedItem;
        public Role? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    if (SelectedItem != null)
                        Role = SelectedItem.RoleName;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
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

        private string _role;

        public string? Role
        {
            get { return _role; }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    Validate(nameof(Role), value);
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public async Task GetAllRoles()
        {
            var roles = await PermissonRepository.GetAllRoles();
            RoleRecords = new ObservableCollection<Role>(roles);

        }
    }
}
