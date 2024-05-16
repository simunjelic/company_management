using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using Praksa_projectV1.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Praksa_projectV1.Commands;
using Microsoft.Identity.Client.NativeInterop;
using System.Security.Permissions;

namespace Praksa_projectV1.ViewModels
{
    public class AdminPanelViewModel : ViewModelBase
    {
        public IPermissonRepository permissonRepository;
        public IAsyncCommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public IAsyncCommand AddCommand { get; }
        public string ModuleName = "Admin panel";


        public AdminPanelViewModel()
        {
            permissonRepository = new PermissonRepository();
            DeleteCommand = new AsyncCommand(DeleteAsync, CanDeleteAsync);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            ShowPermissionRecords = new ObservableCollection<string>(GetAvailableActions());
            AddCommand = new AsyncCommand(AddAsync, CanAddAsync);
            OnLoadAsync();


        }
        public async Task OnLoadAsync()
        {
            await GetAllPermissionsAsync();
            await GetAllModulesAndRoles();
        }

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {
            AdminPanelEditView adminPanelEditView = new AdminPanelEditView();
            adminPanelEditView.DataContext = this;
            adminPanelEditView.Title = "Dodaj novu dozvolu";
            ResetData();
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            adminPanelEditView.Show();
        }

        private bool CanAddAsync()
        {
            return true;
        }

        public async Task AddAsync()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null))
            {
                Permission permission = new Permission();
                permission.ModuleId = Module.Id;
                permission.RoleId = Role.Id;
                permission.ActionId = (int)AvailableAction;


                if (!PermissionRecords.Any(i => i.ModuleId == permission.ModuleId && i.RoleId == permission.RoleId && i.ActionId == permission.ActionId))
                {
                    bool check = await PermissonRepository.AddAsync(permission);

                    if (check)
                    {
                        permission.Action = AvailableAction.ToString();
                        permission.Module = Module;
                        permission.Role = Role;
                        PermissionRecords.Add(permission);
                        PermissionFilterRecords.Add(permission);
                        MessageBox.Show("Nova dozvola dodana", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (LoggedUserData.RolesId.Contains((int)permission.RoleId))
                        {
                           if(permission.ActionId == 0)
                                PermissionAccess.CreatePermission.Add(permission);
                           else if(permission.ActionId == 1)
                                PermissionAccess.UpdatePermission.Add(permission);
                           else if (permission.ActionId == 2)
                                PermissionAccess.DeletePermission.Add(permission);
                           else if (permission.ActionId == 3)
                                PermissionAccess.ReadPermission.Add(permission);
                        }
                    }
                    else MessageBox.Show("Greška pri dodvanu nove dozvole", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else MessageBox.Show("Dozvola već postoji", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else MessageBox.Show("Popunite sva bolja označena crveno.");
        }

        private bool CanDeleteAsync()
        {
            return CanDeletePermission("Admin panel");
        }

        public async Task DeleteAsync()
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati dozvolu: " + SelectedItem.Module.Name + " " + SelectedItem.Role.RoleName + " " + SelectedItem.Action + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool check = await permissonRepository.RemoveAsync(SelectedItem);
                    if (check)
                    {
                        if (LoggedUserData.RolesId.Contains(SelectedItem.RoleId?? 0))
                        {
                            if (SelectedItem.ActionId == 0)
                                PermissionAccess.CreatePermission.RemoveAll(item => item.ModuleId == SelectedItem.ModuleId && item.RoleId == SelectedItem.RoleId);
                            else if (SelectedItem.ActionId == 1)
                                PermissionAccess.UpdatePermission.RemoveAll(item => item.ModuleId == SelectedItem.ModuleId && item.RoleId == SelectedItem.RoleId);
                            else if (SelectedItem.ActionId == 2)
                                PermissionAccess.DeletePermission.RemoveAll(item => item.ModuleId == SelectedItem.ModuleId && item.RoleId == SelectedItem.RoleId);
                            else if (SelectedItem.ActionId == 3)
                                PermissionAccess.ReadPermission.RemoveAll(item => item.ModuleId == SelectedItem.ModuleId && item.RoleId == SelectedItem.RoleId);
                        }
                        PermissionRecords.Remove(SelectedItem);
                        PermissionFilterRecords.Remove(SelectedItem);
                        MessageBox.Show("Dozvola uspješno obrisana");
                        
                    }
                    else MessageBox.Show("Greška pri brisanju dozvole");


                    SelectedItem = null;
                }

            }
            else MessageBox.Show("Odaberite redak koji želite urediti");
        }

       

        public List<string> GetAvailableActions()
        {
            var values = Enum.GetValues(typeof(AvailableActions));
            var actions = new List<AvailableActions>();
            foreach (var action in values)
            {
                actions.Add((AvailableActions)action);
            }
            return actions.Select(x => x.Describe()).ToList();

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

        private ObservableCollection<Permission> _permissionRecords;
        public ObservableCollection<Permission> PermissionRecords
        {
            get
            {
                return _permissionRecords;
            }
            set
            {
                _permissionRecords = value;
                OnPropertyChanged(nameof(PermissionRecords));
            }
        }
        private ObservableCollection<Permission> _permissionFilterRecords;
        public ObservableCollection<Permission> PermissionFilterRecords
        {
            get
            {
                return _permissionFilterRecords;
            }
            set
            {
                _permissionFilterRecords = value;
                OnPropertyChanged(nameof(PermissionFilterRecords));
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
        private ObservableCollection<Module> _moduleRecords;
        public ObservableCollection<Module> ModuleRecords
        {
            get
            {
                return _moduleRecords;
            }
            set
            {
                _moduleRecords = value;
                OnPropertyChanged(nameof(ModuleRecords));
            }
        }

        private Permission _selectedItem;
        public Permission? SelectedItem
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

        private Module _module;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Module Module
        {
            get { return _module; }
            set
            {
                if (_module != value)
                {
                    Validate(nameof(Module), value);
                    _module = value;
                    OnPropertyChanged(nameof(Module));
                }
            }
        }
        private ObservableCollection<string> _showPermissionRecords;


        public ObservableCollection<string> ShowPermissionRecords
        {
            get { return _showPermissionRecords; }
            set
            {
                _showPermissionRecords = value;
                OnPropertyChanged(nameof(ShowPermissionRecords));
            }
        }
        private string _selectedPermission;
        //
        public string SelectedPermission
        {
            get { return _selectedPermission; }
            set
            {
                _selectedPermission = value;
                OnPropertyChanged(nameof(SelectedPermission));
            }
        }
        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                FilterData(); // Call method to filter data when search query changes
            }
        }
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        private AvailableActions _availableAction;
        public AvailableActions AvailableAction
        {
            get
            {
                return _availableAction;
            }
            set
            {
                _availableAction = value;
                Validate(nameof(AvailableAction), value);
                OnPropertyChanged("AvailableAction");
            }
        }

        private async void FilterData()
        {
            //if(!string.IsNullOrWhiteSpace(SearchQuery))
            PermissionFilterRecords = new ObservableCollection<Permission>(PermissionRecords.Where(i =>
                                                                i.Module.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                i.Action.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                               i.Role.RoleName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0));



        }



        public async Task GetAllPermissionsAsync()
        {
            var permissions = await permissonRepository.GetAllPermissions();
            PermissionRecords = new ObservableCollection<Permission>(permissions);
            PermissionFilterRecords = new ObservableCollection<Permission>(permissions);

        }
        public async Task GetAllModulesAndRoles()
        {
            var roles = await permissonRepository.GetAllRolesAsync();
            RoleRecords = new ObservableCollection<Role>(roles);
            var modules = await permissonRepository.GetAllModules();
            ModuleRecords = new ObservableCollection<Module>(modules);

        }
        private void ResetData()
        {
            SelectedItem = null;
            SelectedPermission = null;
            Role = null;
            Module = null;

        }
    }





}
