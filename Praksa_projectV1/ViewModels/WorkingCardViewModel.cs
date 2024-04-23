using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class WorkingCardViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        WorkingCardRepository cardRespository { get; set; }
        ProjectRepository ProjectRepository { get; set; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddCommand { get; }

        public WorkingCardViewModel()
        {
            cardRespository = new();
            gettAllDataFromCard();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddCommand = new ViewModelCommand(Add, CanAdd);


        }

        private bool CanAdd(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private void Add(object obj)
        {
            MessageBox.Show(Hours);
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
        
        private string _hours;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;

                Validate(nameof(Hours), value);
                OnPropertyChanged(nameof(Hours));
            }
        }
        
        private Activity _selectedActivity;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Activity SelectedActivity
        {
            get
            {
                return _selectedActivity;
            }
            set
            {
                _selectedActivity = value;
                Validate(nameof(SelectedActivity), value);
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }
        private Project _selectedProject;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Project SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                _selectedProject = value;
                Validate(nameof(SelectedProject), value);
                OnPropertyChanged(nameof(SelectedProject));
            }
        }

        Dictionary<string, List<string>> Erorrs = new Dictionary<string, List<string>>();
        public bool HasErrors => Erorrs.Count > 0;

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
            Hours = null;
            SelectedActivity = null;
            SelectedProject = null;

        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (Erorrs.ContainsKey(propertyName))
            {
                return Erorrs[propertyName];
            }else
            {
                return Enumerable.Empty<string>();
            }
        }
        public void Validate(string propertyName, object propertyValue)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(propertyValue, new ValidationContext(this) { MemberName = propertyName}, results);

            if(results.Any())
            {
                try { 
                Erorrs.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                } catch { }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }else
            {
                Erorrs.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

        }
    }
}
