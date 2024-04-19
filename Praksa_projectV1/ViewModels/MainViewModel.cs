using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Praksa_projectV1.Models;
using Praksa_projectV1.DataAccess;
using System.Threading;
using System.Printing;
using FontAwesome.Sharp;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private IUserRepository userRepository;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;
        JobsViewModel jobsViewModel;



        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }
        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
        //--> Commands
        public ICommand ShowProjectsViewCommand { get; }
        public ICommand ShowWorkersViewCommand { get; }
        public ICommand ShowJobsViewCommand { get; }
        public ICommand ShowDepartmentsViewCommand {  get; }


        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();
            //Initialize commands
            ShowProjectsViewCommand = new ViewModelCommand(ExecuteShowProjectsViewCommand);
            ShowWorkersViewCommand = new ViewModelCommand(ExecuteShowWorkersViewCommand);
            ShowJobsViewCommand = new ViewModelCommand(ExecuteShowJobsViewCommand);
            ShowDepartmentsViewCommand = new ViewModelCommand(ExecuteShowDepartmentsViewCommand);
            //Default view
            //ExecuteShowProjectsViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowDepartmentsViewCommand(object obj)
        {
            CurrentChildView = new DepartmentsViewModel();
            Caption = "Departments";
            Icon = IconChar.Building;
        }

        private void ExecuteShowJobsViewCommand(object obj)
        {
            
            CurrentChildView = new JobsViewModel();
            Caption = "Jobs";
            Icon = IconChar.Briefcase;
        }

       

        private void ExecuteShowWorkersViewCommand(object obj)
        {
            CurrentChildView = new WorkersViewModel();
            Caption = "Workers";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowProjectsViewCommand(object obj)
        {
            CurrentChildView = new ProjectsViewModel();
            Caption = "Projects";
            Icon = IconChar.Table;
        }

        private void LoadCurrentUserData()
        {
            
                if(Thread.CurrentPrincipal?.Identity.Name != null) { 
                CurrentUserAccount.Username = Thread.CurrentPrincipal.Identity.Name;
                CurrentUserAccount.DisplayName = Thread.CurrentPrincipal.Identity.Name.ToString();
                CurrentUserAccount.ProfilePicture = null;
            }
            else
            {
                CurrentUserAccount.DisplayName = "Invalid user, not logged in";
            }

        }

    }
}
