using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Enums;
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
using Type = Praksa_projectV1.Models.Type;

namespace Praksa_projectV1.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        ProjectRepository ProjectRepository;
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }



        public ProjectsViewModel()
        {
            ProjectRepository = new ProjectRepository();
            gatAllProjects();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddCommand = new ViewModelCommand(AddEmployee, CanAddEmployee);
            UpdateCommand = new ViewModelCommand(UpdateEmployee, CanUpdateEmployee);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);


        }



        private bool CanUpdateEmployee(object obj)
        {
            return true;
        }

        private async void UpdateEmployee(object obj)
        {
            if (ValidationData())
            {
                SelectedItem = PopulateData(SelectedItem);
                _ = ProjectRepository.UpdateProjectAsync(SelectedItem);
                int index = -1;
                index = ProjectRecords.IndexOf(ProjectRecords.Where(x => x.Id == Id).Single());
                ProjectRecords[index] = await ProjectRepository.GetProjectByIdAsync(Id);
                ResetData();
            }

        }

        private bool CanAddEmployee(object obj)
        {
            return true;
        }

        private void AddEmployee(object obj)
        {

            Project newProject = new Project();

            if (ValidationData())
            {
                newProject = PopulateData(newProject);
                ProjectRepository.Add(newProject);
                MessageBox.Show("New project added");
                newProject.Location = Location;
                newProject.Type = Type;
                ProjectRecords.Add(newProject);
                ResetData();


            }



        }
        public void ResetData()
        {
            Id = -1;
            Location = null;
            Type = null;
            Name = string.Empty;
            Status = null;
            StartDate = null;
            EndDate = null;

        }
        private void FillUpdateForm()
        {
            try
            {
                Id = SelectedItem.Id;
                if (SelectedItem.Location != null)
                {
                    Location = LocationRecords.Where(i => i.Id == SelectedItem.LocationId).Single();
                }
                if (SelectedItem.Type != null)
                {
                    Type = TypeRecords.Where(i => i.Id == SelectedItem.TypeId).Single();
                }
                Name = SelectedItem.Name;
                if (SelectedItem.Status != null)
                    Status = (Status)Enum.Parse(typeof(Status), SelectedItem.Status);
                if (SelectedItem.StartDate != null)
                {
                    DateOnly dateOnly = (DateOnly)SelectedItem.StartDate;
                    StartDate = dateOnly.ToDateTime(TimeOnly.Parse("10:00 PM"));
                }
                if (SelectedItem.EndDate != null)
                {
                    DateOnly dateOnly = (DateOnly)SelectedItem.EndDate;
                    EndDate = dateOnly.ToDateTime(TimeOnly.Parse("10:00 PM"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Project PopulateData(Project newProject)
        {
            if(Name != null)
            newProject.Name = Name;
            if (Status != null)
            {
                newProject.Status = Status.ToString();
            }
            if (Location != null)
            {
                newProject.LocationId = Location.Id;
            }
            if (Type != null)
            {
                newProject.TypeId = Type.Id;
            }
            if (StartDate != null)
            {
                newProject.StartDate = new DateOnly(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
            }
            if (EndDate != null)
            {
                newProject.EndDate = new DateOnly(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day);
            }
            return newProject;
        }

        private bool ValidationData()
        {
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Unesi ime projekta");
                return false;
            }
            if (StartDate != null && StartDate != null)
            {
                if (StartDate >= EndDate)
                {
                    MessageBox.Show("Unesi ispravne datume");
                    return false;
                }
            }
            return true;
        }
        private bool CanShowUpdateWindow(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void ShowUpdateWindow(object obj)
        {
            ProjectEditView projectEditView = new ProjectEditView();
            projectEditView.DataContext = this;
            projectEditView.Title = "Edit project";
            _isUpdateButtonVisible = true;
            _isAddButtonVisible = false;
            getLocationsAndTypes();
            FillUpdateForm();
            projectEditView.Show();
        }

        private bool CanShowAddWindow(object obj)
        {
            return true;
        }

        private void ShowAddWindow(object obj)
        {
            ProjectEditView projectEditView = new ProjectEditView();
            projectEditView.DataContext = this;
            projectEditView.Title = "Add project";
            _isUpdateButtonVisible = false;
            _isAddButtonVisible = true;
            getLocationsAndTypes();
            ResetData();
            projectEditView.Show();
        }

        private bool CanDelete(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void Delete(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Project with name: " + SelectedItem.Name + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ProjectRepository.DeleteById(SelectedItem.Id);
                ProjectRecords.Remove(SelectedItem);



            }
            SelectedItem = null;
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

        private Project _selectedItem;
        public Project? SelectedItem
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
        private ObservableCollection<Location> _locationRecords;
        public ObservableCollection<Location> LocationRecords
        {
            get
            {
                return _locationRecords;
            }
            set
            {
                _locationRecords = value;
                OnPropertyChanged(nameof(LocationRecords));
            }
        }
        private ObservableCollection<Type> _typeRecords;
        public ObservableCollection<Type> TypeRecords
        {
            get
            {
                return _typeRecords;
            }
            set
            {
                _typeRecords = value;
                OnPropertyChanged(nameof(Type));
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
        private Enum _status;
        public Enum? Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        private Location _location;
        public Location? Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }
        private Type _type;
        public Type? Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }



        public void gatAllProjects()
        {
            ProjectRecords = new ObservableCollection<Project>(ProjectRepository.GetAll());

        }
        public void getLocationsAndTypes()
        {
            LocationRecords = new ObservableCollection<Location>(ProjectRepository.GetAllLocations());
            TypeRecords = new ObservableCollection<Type>(ProjectRepository.GetAllTypes());

        }
    }

}
