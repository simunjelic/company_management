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
    public class WorkingCardViewModel : ViewModelBase
    {
        WorkingCardRepository cardRespository { get; set; }
        ProjectRepository ProjectRepository { get; set; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }

        public WorkingCardViewModel()
        {
            cardRespository = new();
            gettAllDataFromCard();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);


        }

        private bool CanShowAddWindow(object obj)
        {
            return true;
        }

        private void ShowAddWindow(object obj)
        {
            WorkingCardEdit workingCardEdit = new WorkingCardEdit();
            workingCardEdit.DataContext = this;
            workingCardEdit.Title = "Radna karta: Novi zapis";
            gettAllProjectsAndLocations();
            ResetData();
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            workingCardEdit.Show();
        }

        
        private bool CanDelete(object obj)
        {
            if (SelectedItem != null) return true;
            return false;
        }

        private async void Delete(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ovu aktivnost: " + SelectedItem.Activity.Name + ". Datum: "+ SelectedItem.Date+ "", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool check = await cardRespository.DeleteByIdAsync(SelectedItem.Id);
                if (check) { 
                    CardRecords.Remove(SelectedItem);
                    MessageBox.Show("Podatak obrisan.");
                }
                else MessageBox.Show("Nije moguće obrisati podatak.");



            }
            SelectedItem = null;
        }

        private ObservableCollection<Project> _projectRecords;
        public ObservableCollection<Project> ProjectRecords
        {
            get
            {
                return _projectRecords;
            }
            set
            {
                _projectRecords = value;
                OnPropertyChanged(nameof(ProjectRecords));
            }
        }
        private ObservableCollection<Activity> _activityRecords;
        public ObservableCollection<Activity> ActivityRecords
        {
            get
            {
                return _activityRecords;
            }
            set
            {
                _activityRecords = value;
                OnPropertyChanged(nameof(ActivityRecords));
            }
        }
        private ObservableCollection<WorkingCard> _cardRecords;
        public ObservableCollection<WorkingCard> CardRecords
        {
            get
            {
                return _cardRecords;
            }
            set
            {
                _cardRecords = value;
                OnPropertyChanged(nameof(CardRecords));
            }
        }
        private WorkingCard _selectedItem;
        public WorkingCard? SelectedItem
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
        private bool _isAddButtonVisible = true;
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
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public async void gettAllDataFromCard()
        {
            var card = await cardRespository.GetAllData();
            CardRecords = new ObservableCollection<WorkingCard>(card);

        }
        public async void gettAllProjectsAndLocations()
        {
            ProjectRecords = new ObservableCollection<Project>(ProjectRepository.GetAll());
            var Activities = await WorkingCardRepository.GetAllActivties();
            ActivityRecords = new ObservableCollection<Activity>(Activities);
        }
        private async void ResetData()
        {
            SelectedItem = null;
            SelectedDate = DateTime.Today;

        }

    }
}
