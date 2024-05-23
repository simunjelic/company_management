using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Praksa_projectV1.Commands;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class LocationViewModel : ViewModelBase
    {
        public readonly string ModuleName = "Lokacije";
        public ILocationRepository LocationRepository { get; }
        public IAsyncCommand LoadedCommand { get; }
        public IAsyncCommand DeleteCommand { get; }
        public ICommand ShowWindowCommand { get; }
        public IAsyncCommand AddLocationCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }




        public LocationViewModel()
        {
            LoadedCommand = new AsyncCommand(LoadedCommandExecute);
            LocationRepository = new LocationRepository();
            DeleteCommand = new AsyncCommand(DeleteExecute, CanDeleteCommand);
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            AddLocationCommand = new AsyncCommand(AddLocation, CanAddLocation);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateEmployeeCommand = new AsyncCommand(UpdateEmployee, CanUpdateEmployee);
        }

        private bool CanShowUpdateWindow(object obj)
        {
            return CanUpdatePermission(ModuleName) && SelectedItem != null;
        }

        private void ShowUpdateWindow(object obj)
        {
            LocationEditView locationEditView = new LocationEditView();
            locationEditView.Title = "Uredi lokaciju";
            locationEditView.DataContext = this;
            IsAddButtonVisible = false;
            IsUpdateButtonVisible = true;
            Name = SelectedItem!.Name;
            Address = SelectedItem.Address;
            City = SelectedItem.City;
            Country = SelectedItem.Country;
            locationEditView.Show();
        }

        private bool CanUpdateEmployee()
        {
            return true;
        }

        private async Task UpdateEmployee()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null))
            {
                
                SelectedItem!.Name = Name;
                SelectedItem.Address ??= Address;
                SelectedItem.City ??= City;
                SelectedItem.Country ??= Country;
                bool check = await LocationRepository.EditAsync(SelectedItem);
                if (check)
                {
                    
                    await GetAllLocationsAsync();
                    ResetData();
                    System.Windows.Forms.MessageBox.Show("Lokacija uspješno uređena.", "Uspijeh", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else System.Windows.Forms.MessageBox.Show("Greška prilikom uređivanja podatka.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else System.Windows.Forms.MessageBox.Show("Popuni polje označeno crvenom bojom.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private bool CanAddLocation()
        {
            return true;
        }

        private async Task AddLocation()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null))
            {
                Location location = new();
                location.Name = Name;
                location.Address ??= Address;
                location.City ??= City;
                location.Country ??= Country;
                bool check = await LocationRepository.AddAsync(location);
                if (check)
                {
                    LocationsRecords.Add(location);
                    ResetData();
                    System.Windows.Forms.MessageBox.Show("Lokacija uspješno dodana.", "Uspijeh", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else System.Windows.Forms.MessageBox.Show("Greška prilikom dodavanja podatka.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else System.Windows.Forms.MessageBox.Show("Popuni polje označeno crvenom bojom.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }




        private bool CanShowWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowWindow(object obj)
        {
            LocationEditView locationView = new LocationEditView();
            locationView.DataContext = this;
            ResetData();
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            locationView.Show();

        }

        private bool CanDeleteCommand()
        {
            return CanDeletePermission(ModuleName);
        }

        private async Task DeleteExecute()
        {
            if (SelectedItem != null)
            {
                var result = System.Windows.MessageBox.Show("Jeste li sigurni da želite izbrisati lokaciju: " + SelectedItem.Name, "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool check = await LocationRepository.DeleteAsync(SelectedItem);
                    if (check)
                    {
                        LocationsRecords.Remove(SelectedItem);
                        System.Windows.Forms.MessageBox.Show("Lokacija obrisana", "Uspijeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SelectedItem = null;


                    }
                    else System.Windows.Forms.MessageBox.Show("Greška prilikom brisanja podatka", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else System.Windows.Forms.MessageBox.Show("Odaberite lokaciju", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private async Task LoadedCommandExecute()
        {
            await GetAllLocationsAsync();
        }



        private ObservableCollection<Location>? _locationsRecords;
        public ObservableCollection<Location>? LocationsRecords
        {
            get
            {
                return _locationsRecords;
            }
            set
            {
                _locationsRecords = value;
                OnPropertyChanged(nameof(LocationsRecords));
            }
        }

        private async Task GetAllLocationsAsync()
        {
            var locations = await LocationRepository.GetAllLocationsAsync();
            LocationsRecords = new ObservableCollection<Location>(locations);
        }
        private Location? _selectedItem;
        public Location? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value!;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private string? _name;
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
        private string? _address;
        public string? Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }
        private int? _id;
        public int? Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        private string? _city;
        public string? City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
                OnPropertyChanged("City");
            }
        }
        private string? _country;
        public string? Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
                OnPropertyChanged("Country");
            }
        }
        public void ResetData()
        {
            SelectedItem = null;
            Name = null;
            Address = null;
            City = null;
            Country = null;
        }

    }
}
