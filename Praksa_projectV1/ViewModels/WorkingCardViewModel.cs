using Microsoft.Extensions.Primitives;
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
using System.Windows.Media.Animation;

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

        public WorkingCardViewModel()
        {
            cardRespository = new();
            userRepository = new();
            gettAllDataFromCard();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddCommand = new ViewModelCommand(Add, CanAdd);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateCommand = new ViewModelCommand(UpdateRecord, CanUpdateRecord);
            gettAllProjectsAndActivities();


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
                int index = -1;
                index = CardRecords.IndexOf(CardRecords.Where(x => x.Id == Id).Single());
                CardRecords.RemoveAt(index);
                var newCard = await cardRespository.FindByIdAsync(Id);
                index = 0;
                while (index < CardRecords.Count && CardRecords[index].Date >= newCard.Date)
                {
                    index++;
                }
                // Insert the new card record at the correct position
                CardRecords.Insert(index, newCard);

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
            if (SelectedItem != null) return true;
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
            var employee = await userRepository.getEmployeeByUsernameAsync(username);
            if (employee != null)
                newCard.EmployeeId = employee.Id;

            var check = await cardRespository.Add(newCard);
            if (check)
            {
                MessageBox.Show("Sati dodani.");
                newCard.Activity = SelectedActivity;
                newCard.Project = SelectedProject;
                int index = 0;
                // Find the correct index to insert the new card record
                while (index < CardRecords.Count && CardRecords[index].Date >= newCard.Date)
                {
                    index++;
                }
                // Insert the new card record at the correct position
                CardRecords.Insert(index, newCard);
                ResetData();
            }
            else
            {
                MessageBox.Show("Greška pri unosu sati");
            }
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



        public async void gettAllDataFromCard()
        {
            var card = await cardRespository.GetAllData();
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


    }
}
