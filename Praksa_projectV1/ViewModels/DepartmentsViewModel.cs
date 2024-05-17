using Praksa_projectV1.Commands;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class DepartmentsViewModel : ViewModelBase
    {

        private DepartmentRepository departmentRepository;
        public IDepartmentRepository IdepartmentRepository;
        public IAsyncCommand DeleteDepartmentCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public IAsyncCommand AddDepartmentCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public IAsyncCommand UpdateDepartmentCommand { get; }
        public IAsyncCommand LoadedCommand { get; }
        public string ModuleName = "Odjel";

        public DepartmentsViewModel()
        {
            departmentRepository = new DepartmentRepository();
            IdepartmentRepository = new DepartmentRepository();
            DeleteDepartmentCommand = new AsyncCommand(DeleteDepartmentAsync, CanDeleteDepartmentAsync);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddDepartmentCommand = new AsyncCommand(AddDepartmentAsync, CanAddDepartmentAsync);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateDepartmentCommand = new AsyncCommand(UpdateDepartmentAsync, CanUpdateDepartmentCommandAsync);
            LoadedCommand = new AsyncCommand(OnLoadAsync);
        }

        private async Task OnLoadAsync()
        {
          await GetAllDepartmentsAsync();
        }

        private bool CanShowUpdateWindow(object obj)
        {
            return CanUpdatePermission(ModuleName) && SelectedItem != null;
        }

        private void ShowUpdateWindow(object obj)
        {
                if (SelectedItem.ParentDepartmentId != null)
                    SelectedDepartment = (Department)DepartmentRecords.Where(x => x.Id == SelectedItem.ParentDepartmentId).Single();
                Name = SelectedItem.Name;
                Id = SelectedItem.Id;
                DepartmentsEditView view = new DepartmentsEditView();
                _isUpdateButtonVisible = true;
                _isAddButtonVisible = false;
                view.Title = "Uredi odjel.";
                view.DataContext = this;
                view.Show();
        }

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {
            DepartmentsEditView view = new DepartmentsEditView();
            view.Title = "Dodaj novi odjel.";
            ResetData();
            _isUpdateButtonVisible = false;
            _isAddButtonVisible = true;
            view.DataContext = this;
            view.Show();
        }

        private bool CanAddDepartmentAsync()
        {
            return true;
        }

        public async Task AddDepartmentAsync()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null))
            {
                if (!DepartmentRecords.Any(i => i.Name == Name))
                {
                    Department newDepartment = new();
                    newDepartment.Name = Name;
                    if (SelectedDepartment != null)
                        newDepartment.ParentDepartmentId = SelectedDepartment.Id;
                    bool flag = await IdepartmentRepository.AddAsync(newDepartment);
                    if (flag)
                    {
                        MessageBox.Show("Odjel s nazivom " + Name + " dodan.");
                        if (SelectedDepartment != null)
                            newDepartment.ParentDepartment = SelectedDepartment;
                        DepartmentRecords.Add(newDepartment);
                        ResetData();
                    }
                    else
                    {
                        MessageBox.Show("Greška prilikom dodavanja novog odjela.");
                    }
                }
                else MessageBox.Show("Postoji odjel sa istim imenom.");
            }
            else MessageBox.Show("Unesite naziv novog odjela.");
        }

        private bool CanUpdateDepartmentCommandAsync()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        public async Task UpdateDepartmentAsync()
        {
            if (!DepartmentRecords.Any(i => i.Id != Id && i.Name == Name))
            {
                Department department = new Department();
                department.Id = this.Id;
                department.Name = Name;
                if (SelectedDepartment != null)
                    department.ParentDepartmentId = SelectedDepartment.Id;
                MessageBoxResult result = MessageBox.Show("Jeste li sigurni da želite ažurirati ovaj zapis?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool isSuccessful = await IdepartmentRepository.UpdateAsync(department);
                    if (isSuccessful)
                    {
                        string message = "Podatak ažuriran!";
                        
                        int index = -1;
                        index = DepartmentRecords.IndexOf(DepartmentRecords.Where(x => x.Id == Id).Single());
                        department.ParentDepartment = SelectedDepartment;
                        DepartmentRecords[index] = department;
                        MessageBox.Show(message);
                        ResetData();

                    }
                    else MessageBox.Show("Greška prilikom ažuriranja.");
                }

            }
            else MessageBox.Show("Odabrano ime već postoji.");
        }

        private bool CanDeleteDepartmentAsync()
        {
            return CanDeletePermission(ModuleName);
        }

        public async Task DeleteDepartmentAsync()
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite izbrisati odjel " + SelectedItem.Name + " ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    bool res = await IdepartmentRepository.RemoveAsync(SelectedItem);
                    if (res)
                    {
                        DepartmentRecords.Remove(SelectedItem);
                        MessageBox.Show("Odjel obrisan.");
                    }


                    else
                        MessageBox.Show("Nije moguće izbrisati odjel.");
                }
            }
            else MessageBox.Show("Odaberite redak koji želite obrisati.");
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

        private string _name;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string? Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                Validate(nameof(Name), value);
                OnPropertyChanged("Name");
            }
        }
        private string _parentDepartment;
        public string ParentDepartment
        {
            get
            {
                return _parentDepartment;
            }
            set
            {
                _parentDepartment = value;
                OnPropertyChanged("ParentDepartment");
            }
        }
        private Department _selectedItem;

        public Department? SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }

        }

        private Department _selectedDepartment;

        public Department? SelectedDepartment
        {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged("SelectedDepartment");
            }

        }
        private ObservableCollection<Department> _departmentRecords;
        public ObservableCollection<Department> DepartmentRecords
        {
            get
            {
                return _departmentRecords;
            }
            set
            {
                _departmentRecords = value;
                OnPropertyChanged("DepartmentRecords");
            }
        }
        private bool _isViewVisible = true;
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

        private string _errorMessage;
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
        private bool _isUpdateButtonVisible = true; // Initially visible

        public bool IsUpdateButtonVisible
        {
            get { return _isUpdateButtonVisible; }
            set
            {
                if (_isUpdateButtonVisible != value)
                {
                    _isUpdateButtonVisible = value;
                    OnPropertyChanged(nameof(IsUpdateButtonVisible)); // Notify property changed
                }
            }
        }
        private bool _isAddButtonVisible = true; // Initially visible

        public bool IsAddButtonVisible
        {
            get { return _isAddButtonVisible; }
            set
            {
                if (_isAddButtonVisible != value)
                {
                    _isAddButtonVisible = value;
                    OnPropertyChanged(nameof(IsAddButtonVisible)); // Notify property changed
                }
            }
        }


        public async Task GetAllDepartmentsAsync()
        {
            var departments = await IdepartmentRepository.GetAllDepartmentsAsync();

            DepartmentRecords = new ObservableCollection<Department>(departments);

        }
        public void ResetData()
        {
            Name = null;
            SelectedDepartment = null;
        }
    }
}
