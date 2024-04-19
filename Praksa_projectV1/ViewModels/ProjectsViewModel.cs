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
using Type = Praksa_projectV1.Models.Type;

namespace Praksa_projectV1.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        ProjectRepository ProjectRepository;
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }



        public ProjectsViewModel()
        {
            ProjectRepository = new ProjectRepository();
            gatAllProjects();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddEmployeeCommand = new ViewModelCommand(AddEmployee, CanAddEmployee);
            UpdateEmployeeCommand = new ViewModelCommand(UpdateEmployee, CanUpdateEmployee);


        }

        private bool CanUpdateEmployee(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void UpdateEmployee(object obj)
        {
            
        }

        private bool CanAddEmployee(object obj)
        {
            return true;
        }

        private void AddEmployee(object obj)
        {
          
            Project newProject = new Project();

            if (validationData())
            {
                newProject = populateData(newProject);
                ProjectRepository.Add(newProject);
                MessageBox.Show("New project added");
                newProject.Location = Location;
                newProject.Type = Type;
                ProjectRecords.Add(newProject);


            }



        }

        private Project populateData(Project newProject)
        {
            newProject.Name = Name;
            if(Status != null)
            {
                newProject.Status = Status.ToString();
            }
            if(Location != null)
            {
                newProject.LocationId = Location.Id;
            }if(Type != null)
            {
                newProject.TypeId = Type.Id;
            }
            if(StartDate != null)
            {
                newProject.StartDate = new DateOnly(StartDate.Year, StartDate.Month, StartDate.Day);
            }
            if (EndDate != null)
            {
                newProject.EndDate = new DateOnly(EndDate.Year, EndDate.Month, EndDate.Day);
            }
            return newProject;
        }

        private bool validationData()
        {
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Unesi ime projekta");
                return false;
            }
            if (StartDate != null && StartDate != null)
            {
                if(StartDate >= EndDate)
                {
                    MessageBox.Show("Unesi ispravne datume");
                    return false;
                }
            }
            return true;
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
            SelectedItem = new Project();
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
            var result = MessageBox.Show("Are you sure you want to delete this Project with name: " + SelectedItem.Name +"?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
               ProjectRepository.DeleteById(SelectedItem.Id);
               ProjectRecords.Remove(SelectedItem);



            }
            SelectedItem = null;
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
        public Enum Status
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
        public Location Location
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
        public Type Type
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
        private DateTime _startDate;
        public DateTime StartDate
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
        private DateTime _endDate;
        public DateTime EndDate
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
