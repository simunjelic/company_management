using Microsoft.IdentityModel.Tokens;
using Praksa_projectV1.Commands;
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
        public IAsyncCommand DeleteCommand { get; }
        public string ModuleName = "Uloge";
        public IAsyncCommand UpdateCommand { get; }
        public IAsyncCommand AddRoleCommand { get; }
        public RoleViewModel()
        {

            PermissonRepository = new PermissonRepository();
            GetAllRolesAsync();
            DeleteCommand = new AsyncCommand(DeleteRoleAsync, CanDeleteRoleAsync);
            UpdateCommand = new AsyncCommand(UpdateRoleAsync, CanUpdateRoleAsync);
            AddRoleCommand = new AsyncCommand(AddRoleAsync, CanAddRoleAsync);


        }
       


        private bool CanAddRoleAsync()
        {
            return CanCreatePermission(ModuleName);
        }

        private async Task AddRoleAsync()
        {
            if (!string.IsNullOrWhiteSpace(Role))
            {
                if (!RoleRecords.Any(i => i.RoleName == Role))
                {
                    var result = MessageBox.Show("Jeste li sigurni da želite dodati novu ulogu: " + Role, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Role newRole = new Role();
                        newRole.RoleName = Role;
                        bool check = await PermissonRepository.AddRoleAsync(newRole);
                        if (check)
                        {
                            RoleRecords.Add(newRole);
                            MessageBox.Show("Nova uloga dodana");
                            
                        }
                        else MessageBox.Show("Greška pri dodavanu nove uloge.");
                    }
                    SelectedItem = null;
                    Role = null;
                }
                else MessageBox.Show("Postoji već dozvola sa imenom " + Role);

            }
            else MessageBox.Show("Naziv nesipravan.");
        }

        private bool CanUpdateRoleAsync()
        {
            return CanUpdatePermission(ModuleName);
        }

        private async Task UpdateRoleAsync()
        {
            if (SelectedItem != null)
            {
                if (!RoleRecords.Any(i => i.RoleName == Role))
                {
                    var result = MessageBox.Show("Jeste li sigurni da želite promjeniti naziv iz: " + SelectedItem.RoleName + " u " + Role + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SelectedItem.RoleName = Role;
                        bool check = await PermissonRepository.UpdateRoleAsync(SelectedItem);
                        if (check)
                        {
                            GetAllRolesAsync();
                            MessageBox.Show("Naziv promjenjen");

                        }
                        else MessageBox.Show("Greška pri mjenjaju naziva");


                        
                    }
                    SelectedItem = null;
                    Role = null;
                }
                else MessageBox.Show("Postoji već dozvola sa imenom " + Role);

            }
            else MessageBox.Show("Odaberite ulogu koju želite urediti.");
        }

        private bool CanDeleteRoleAsync()
        {
            return CanDeletePermission(ModuleName);
        }

        private async Task DeleteRoleAsync()
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ulogu: " + SelectedItem.RoleName + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool check = await PermissonRepository.RemoveRoleAsync(SelectedItem);
                    if (check)
                    {
                        RoleRecords.Remove(SelectedItem);
                        MessageBox.Show("Uloga uspješno obrisana");
                    }
                    else MessageBox.Show("Nije moguće obrisati ulogu");


                    
                }
                SelectedItem = null;
                Role = null;

            }
            else MessageBox.Show("Odaberite ulogu koju želite ukloniti");
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

        public async Task GetAllRolesAsync()
        {
            var roles = await PermissonRepository.GetAllRolesAsync();
            RoleRecords = new ObservableCollection<Role>(roles);

        }
    }
}
