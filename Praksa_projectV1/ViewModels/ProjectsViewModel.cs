using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Enums;
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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Type = Praksa_projectV1.Models.Type;

namespace Praksa_projectV1.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        ProjectRepository ProjectRepository;
        EmployeeRepository EmployeeRepository { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddWindowCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand ShowProjectTeamWindowCommand { get; }
        public ICommand AddMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }
        private readonly string ModuleName = "Projekti";



        public ProjectsViewModel()
        {
            ProjectRepository = new ProjectRepository();
            EmployeeRepository = new EmployeeRepository();
            gatAllProjects();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
            ShowAddWindowCommand = new ViewModelCommand(ShowAddWindow, CanShowAddWindow);
            AddCommand = new ViewModelCommand(AddProject, CanAddProject);
            UpdateCommand = new ViewModelCommand(UpdateProject, CanUpdateProject);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            ShowProjectTeamWindowCommand = new ViewModelCommand(ShowProjectTeamWindow, CanShowProjectTeamWindow);
            AddMemberCommand = new ViewModelCommand(AddMemberAsync, CanAddMember);
            DeleteMemberCommand = new ViewModelCommand(DeleteMember, CanDeleteMember);


        }

        private bool CanDeleteMember(object obj)
        {
            if (SelectedEmployee != null && CanDeletePermission(ModuleName))
                return true;
            return false;
        }

        private async void DeleteMember(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite ukoloniti: " + SelectedEmployee.Employee.Name + " " + SelectedEmployee.Employee.Name + " sa projekta " + SelectedEmployee.Project.Name, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var check = await ProjectRepository.DeleteEmployeeFromProjectAsync(SelectedEmployee);
                if (check)
                {
                    TeamRecords.Remove(SelectedEmployee);
                    MessageBox.Show("Zaposlenik ukoljen sa projekta.");
                }
                SelectedEmployee = null;
            }
        }

        private bool CanAddMember(object obj)
        {
            if (SelectedNewEmployee != null && CanCreatePermission(ModuleName))
                return true;
            return false;
        }

        private async void AddMemberAsync(object obj)
        {
            EmployeeProject teamMember = new EmployeeProject();
            teamMember.ProjectId = SelectedItem.Id;
            teamMember.EmployeeId = SelectedNewEmployee.Id;
            if (IsManager) teamMember.Manager = "Da";
            else teamMember.Manager = "Ne";
            var check = await ProjectRepository.AddMemberToProject(teamMember);

            if (check)
            {
                teamMember.Project = ProjectRecords.Where(i => i.Id == SelectedItem.Id).FirstOrDefault();
                teamMember.Employee = SelectedNewEmployee;
                TeamRecords.Add(teamMember);
                IsManager = false;
                SelectedNewEmployee = null;
            }
        }

        private bool CanShowProjectTeamWindow(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void ShowProjectTeamWindow(object obj)
        {

            ProjectTeamView projectTeamView = new ProjectTeamView();
            projectTeamView.DataContext = this;
            projectTeamView.Title = SelectedItem.Name;
            GetTeamAsync();
            GetAllEmployees();
            projectTeamView.Show();

        }

        private bool CanUpdateProject(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null);
        }

        private async void UpdateProject(object obj)
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

        private bool CanAddProject(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null) && CanCreatePermission(ModuleName);
        }

        private void AddProject(object obj)
        {

            Project newProject = new Project();

            if (ValidationData())
            {
                newProject = PopulateData(newProject);
                ProjectRepository.Add(newProject);
                MessageBox.Show("Dodan novi projekt");
                newProject.Location = Location;
                newProject.Type = Type;
                ProjectRecords.Add(newProject);
                ResetData();


            }



        }

        private bool CanShowUpdateWindow(object obj)
        {
            if (SelectedItem != null && CanUpdatePermission(ModuleName))
                return true;
            return false;
        }

        private void ShowUpdateWindow(object obj)
        {

            ProjectEditView projectEditView = new ProjectEditView();
            projectEditView.DataContext = this;
            projectEditView.Title = "Uredi projekt";
            _isUpdateButtonVisible = true;
            _isAddButtonVisible = false;
            getLocationsAndTypes();
            FillUpdateForm();
            projectEditView.Show();
        }

        private bool CanShowAddWindow(object obj)
        {
            return CanCreatePermission(ModuleName);
        }

        private void ShowAddWindow(object obj)
        {

            ProjectEditView projectEditView = new ProjectEditView();
            projectEditView.DataContext = this;
            projectEditView.Title = "Dodaj projekt";
            _isUpdateButtonVisible = false;
            _isAddButtonVisible = true;
            getLocationsAndTypes();
            ResetData();
            projectEditView.Show();
        }

        private bool CanDelete(object obj)
        {
            if (SelectedItem != null && CanDeletePermission(ModuleName))
                return true;
            return false;
        }

        private async void Delete(object obj)
        {
            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati ovaj projekt: " + SelectedItem.Name + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var check = await ProjectRepository.DeleteByIdAsync(SelectedItem.Id);
                if(check)
                 ProjectRecords.Remove(SelectedItem);
                else MessageBox.Show("Nije moguće obrisati projekt.");



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
        [Required(ErrorMessage = "Polje ne može biti prazno.")]
        public string Name
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

        private async void FilterData()
        {
            //if(!string.IsNullOrWhiteSpace(SearchQuery))
            ProjectRecords = new ObservableCollection<Project>(await ProjectRepository.FilterData(SearchQuery));
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
            if (Name != null)
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
        private ObservableCollection<Employee> _employeeRecords;
        public ObservableCollection<Employee> EmployeeRecords
        {
            get
            {
                return _employeeRecords;
            }
            set
            {
                _employeeRecords = value;
                OnPropertyChanged(nameof(EmployeeRecords));
            }
        }
        private ObservableCollection<Employee> _employeeShow;
        public ObservableCollection<Employee> EmployeeShow
        {
            get
            {
                return _employeeShow;
            }
            set
            {
                _employeeShow = value;
                OnPropertyChanged(nameof(EmployeeShow));
            }
        }
        private ObservableCollection<EmployeeProject> _teamRecords;
        public ObservableCollection<EmployeeProject> TeamRecords
        {
            get
            {
                return _teamRecords;
            }
            set
            {
                _teamRecords = value;
                OnPropertyChanged(nameof(TeamRecords));
            }
        }
        private EmployeeProject? _selectedEmployee = null;
        public EmployeeProject? SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                if (_selectedEmployee != value)
                {
                    _selectedEmployee = value;
                    OnPropertyChanged(nameof(SelectedEmployee));
                }
            }
        }
        private Employee _selectedNewEmployee;
        public Employee SelectedNewEmployee
        {
            get { return _selectedNewEmployee; }
            set
            {
                if (_selectedNewEmployee != value)
                {
                    _selectedNewEmployee = value;
                    OnPropertyChanged(nameof(SelectedNewEmployee));
                }
            }
        }
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    FilterSuggestions();
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
        private bool _isComboBoxDropDownOpen;
        public bool IsComboBoxDropDownOpen
        {
            get { return _isComboBoxDropDownOpen; }
            set
            {
                _isComboBoxDropDownOpen = value;
                OnPropertyChanged(nameof(IsComboBoxDropDownOpen));
            }
        }
        private bool _isManager;

        public bool IsManager
        {
            get { return _isManager; }
            set
            {
                if (_isManager != value)
                {
                    _isManager = value;
                    // Notify property changed if you're using a ViewModel
                    OnPropertyChanged(nameof(IsManager));
                }
            }
        }
        private void FilterSuggestions()
        {

            var filteredSuggestions = EmployeeRecords.Where(item =>
                                     (item.Name + " " + item.Surname).Contains(SearchText) ||
                                      (item.Id.ToString() == SearchText));
            EmployeeShow = new ObservableCollection<Employee>(filteredSuggestions);


        }


        public async Task GetTeamAsync()
        {

            var team = await ProjectRepository.GetTeam(SelectedItem.Id);
            TeamRecords = new ObservableCollection<EmployeeProject>(team);
        }
        public void GetAllEmployees()
        {
            var employes = EmployeeRepository.GetAll();
            EmployeeRecords = new ObservableCollection<Employee>(employes);
            EmployeeShow = new ObservableCollection<Employee>(employes);
        }
    }
}
