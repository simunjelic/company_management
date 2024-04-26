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
        public ICommand DeleteCommand { get;}
       public string ModuleName ="Uloge";
        public RoleViewModel()
        {
            
            PermissonRepository = new PermissonRepository();
            GetAllRoles();
            DeleteCommand = new ViewModelCommand(DeleteRole, CanDeleteRole);
            

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
                bool check = await  PermissonRepository.RemoveRole(SelectedItem);
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
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        private Role _role;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Role Role
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
