using Microsoft.Extensions.Primitives;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Validation;
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
using System.Windows.Media.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Praksa_projectV1.ViewModels
{
    public class WorkingCardViewModel : ViewModelBase
    {
        WorkingCardRepository cardRespository { get; set; }
        UserRepository userRepository { get; set; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RefreshDateCommand { get; }
        public string ModuleName = "Radna karta";

        public WorkingCardViewModel()
        {
            // Access StartDate and EndDate from Application resources
            if (Application.Current.Resources["StartDate"] is DateTime startDate)
            {
                StartDate = startDate;
            }
            else
            {
                // Handle case where StartDate resource is not found
                StartDate = DateTime.Today.AddMonths(-1);
            }

            if (Application.Current.Resources["EndDate"] is DateTime endDate)
            {
                EndDate = endDate;
            }
            else
            {
                // Handle case where EndDate resource is not found
                EndDate = DateTime.Today;
            }
        
        cardRespository = new();
            userRepository = new();
            gettAllDataFromCard();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddCommand = new ViewModelCommand(Add, CanAdd);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateCommand = new ViewModelCommand(UpdateRecord, CanUpdateRecord);
            RefreshDateCommand = new ViewModelCommand(RefreshDate, CanRefreshDate);
            gettAllProjectsAndActivities();


        }

        private bool CanRefreshDate(object obj)
        {
            if (StartDate <= EndDate) return true;
            return false;

        }

        private async void RefreshDate(object obj)
        {
            if (IsValidDateWithinLast100Years((DateTime)StartDate) && IsValidDateWithinLast100Years((DateTime)EndDate))
            {
                Application.Current.Resources["StartDate"] = StartDate;
                Application.Current.Resources["EndDate"] = EndDate;
                gettAllDataFromCard();
            }
            else MessageBox.Show("Datumi nisu ispravni.");
        }

        private bool CanUpdateRecord(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private async void UpdateRecord(object obj)
        {
            WorkingCard updateCard = new();
            updateCard.EmployeeId = SelectedItem.EmployeeId;
            updateCard.Id = Id;
            updateCard.ProjectId = SelectedProject.Id;
            updateCard.ActivityId = SelectedActivity.Id;
            updateCard.Hours = decimal.Parse(Hours);
            updateCard.Description = Description;
            updateCard.Date = new DateOnly(SelectedDate.Value.Year, SelectedDate.Value.Month, SelectedDate.Value.Day);
            
            
            bool check = cardRespository.Edit(updateCard);
            if (check)
            {
                gettAllDataFromCard();

                ResetData();
                MessageBox.Show("Podatak uspješno uređen.");
            }
            else
            {
                MessageBox.Show("Greška pri uređivanju podataka.");
            }

        }

        private bool CanShowUpdateWindow(object obj)
        {
            if (SelectedItem != null && CanUpdatePermission(ModuleName)) return true;
            else return false;
        }

        private void ShowUpdateWindow(object obj)
        {
            WorkingCardEdit workingCardEdit = new WorkingCardEdit();
            workingCardEdit.DataContext = this;
            workingCardEdit.Title = "Radna karta: uređivanje zapisa";
            SelectedDate = SelectedItem.Date.HasValue ? new DateTime(SelectedItem.Date.Value.Year, SelectedItem.Date.Value.Month, SelectedItem.Date.Value.Day) : (DateTime?)null;
            SelectedActivity = ActivityRecords.Where(i => i.Id == SelectedItem.ActivityId).Single();
            SelectedProject = ProjectRecords.Where(i => i.Id == SelectedItem.ProjectId).Single();
            Description = SelectedItem.Description;
            Hours = SelectedItem.Hours.ToString();
            Id = SelectedItem.Id;
            IsAddButtonVisible = false;
            IsUpdateButtonVisible = true;
            workingCardEdit.Show();
        }

        private bool CanAdd(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private async void Add(object obj)
        {
            WorkingCard newCard = new WorkingCard();
            newCard.ProjectId = SelectedProject.Id;
            newCard.ActivityId = SelectedActivity.Id;
            newCard.Hours = decimal.Parse(Hours);
            newCard.Description = Description;
            newCard.Date = new DateOnly(SelectedDate.Value.Year, SelectedDate.Value.Month, SelectedDate.Value.Day);
            var username = Thread.CurrentPrincipal?.Identity.Name.ToString();
            var employee = userRepository.getEmployeeByUsername(username);
            if (employee != null)
            {
                newCard.EmployeeId = employee.Id;

                var check = await cardRespository.Add(newCard);
                if (check)
                {
                    MessageBox.Show("Sati dodani.");
                    gettAllDataFromCard();
                    ResetData();
                }
            }
            else
            {
                MessageBox.Show("Greška pri unosu sati");
            }
            
        }

        

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {
            WorkingCardEdit workingCardEdit = new WorkingCardEdit();
            workingCardEdit.DataContext = this;
            workingCardEdit.Title = "Radna karta: Novi zapis";
            
            ResetData();
            IsAddButtonVisible = true;
            IsUpdateButtonVisible = false;
            workingCardEdit.Show();
        }


        private bool CanDelete(object obj)
        {
            if (SelectedItem != null && CanDeletePermission(ModuleName)) return true;
            return false;
        }

        private async void Delete(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ovu aktivnost: " + SelectedItem.Activity.Name + ". Datum: " + SelectedItem.Date + "", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool check = await cardRespository.DeleteByIdAsync(SelectedItem.Id);
                if (check)
                {
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
        private DateTime? _selectedDate = DateTime.Today;
        public DateTime? SelectedDate
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
        private DateTime? _startDate = DateTime.Today.AddMonths(-1);
        [StartDateBeforeEndDate("EndDate", ErrorMessage = "Datum od mora biti prije datuma do.")]
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                
                    _startDate = value;
                Validate(nameof(StartDate), value);
                OnPropertyChanged(nameof(StartDate));
                
            }
        }
        private DateTime? _endDate = DateTime.Today;
        [EndDateAfterStartDate("StartDate", ErrorMessage = "Datum od mora biti prije datuma do.")]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {

                _endDate = value;
                Validate(nameof(EndDate), value);
                OnPropertyChanged(nameof(EndDate));
                
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
        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;


                OnPropertyChanged(nameof(Description));
            }
        }


        private Activity _selectedActivity;
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Activity? SelectedActivity
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


        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public Project? SelectedProject
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
        private ObservableCollection<MonthlySummary> _hoursByMonth;

        public ObservableCollection<MonthlySummary>? HoursByMonth
        {
            get
            {
                return _hoursByMonth;
            }
            set
            {
                _hoursByMonth = value;
                
                OnPropertyChanged(nameof(HoursByMonth));
            }
        }



        public async void gettAllDataFromCard()
        {
            var card =  await cardRespository.GetByStartAndEndDate(new DateOnly(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day), new DateOnly(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day));
            var hours = await cardRespository.GetSummarizedDataByMonth(new DateOnly(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day), new DateOnly(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day));
            HoursByMonth = new ObservableCollection<MonthlySummary>(hours);
            card = card.OrderByDescending(c => c.Date);
            CardRecords = new ObservableCollection<WorkingCard>(card);

        }
        public async void gettAllProjectsAndActivities()
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
            Description = null;

        }
        public static bool IsValidDateWithinLast100Years(DateTime date)
        {
            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Calculate the earliest valid date (100 years ago from now)
            DateTime earliestValidDate = currentDate.AddYears(-100);

            // Check if the year, month, and day values are within valid ranges
            if (date < earliestValidDate || date > currentDate)
            {
                // Date falls outside the last 100 years
                return false;
            }
            // Check if the month value is within valid range (1-12)
            else if (date.Month < 1 || date.Month > 12)
            {
                // Month is invalid
                return false;
            }
            // Check if the day value is within valid range for the given month
            else if (date.Day < 1 || date.Day > DateTime.DaysInMonth(date.Year, date.Month))
            {
                // Day is invalid
                return false;
            }

            // Date is valid
            return true;
        }


    }
}
