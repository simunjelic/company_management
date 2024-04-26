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
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Controls;

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
        public ICommand ShowWorkingCardViewCommand { get; }
        public ICommand ShowAdminPanelViewCommand { get; }
        public ICommand ShowRoleViewCommand { get; }
        public ICommand ShowUserViewCommand { get; }


        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();
            //Initialize commands
            ShowProjectsViewCommand = new ViewModelCommand(ExecuteShowProjectsViewCommand);
            ShowWorkersViewCommand = new ViewModelCommand(ExecuteShowWorkersViewCommand);
            ShowJobsViewCommand = new ViewModelCommand(ExecuteShowJobsViewCommand);
            ShowDepartmentsViewCommand = new ViewModelCommand(ExecuteShowDepartmentsViewCommand);
            ShowWorkingCardViewCommand = new ViewModelCommand(ShowWorkingCard);
            ShowAdminPanelViewCommand = new ViewModelCommand(ShowAdminPanel, CanShowAdminPanel);
            ShowRoleViewCommand = new ViewModelCommand(ShowRoleView);
            ShowUserViewCommand = new ViewModelCommand(ExecuteShowUserView);
            //Default view
            //ExecuteShowProjectsViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowUserView(object obj)
        {
            var principal = Thread.CurrentPrincipal;
            
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {
               
                if (CanReadPermission("Korisnici"))
                {
                    CurrentChildView = new UserViewModel();
                    Caption = "Korisnici";
                    Icon = IconChar.User;
                }
                else
                {

                    MessageBox.Show("Nemate pravo pristupa", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            else
            {
                MessageBox.Show("Niste se pravilno prijavili", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowRoleView(object obj)
        {
            var principal = Thread.CurrentPrincipal;

            // Check if the principal is a GenericPrincipal and has an identity
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            { 
                // Now isAdmin will be true if the user has the "Admin" role, otherwise false
                if (CanReadPermission("Uloge"))
                {
                    CurrentChildView = new RoleViewModel();
                    Caption = "Uloge";
                    Icon = IconChar.UniversalAccess;
                }
                else
                {

                    MessageBox.Show("Nemate pravo pristupa", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            else
            {
                MessageBox.Show("Niste se pravilno prijavili", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool CanShowAdminPanel(object obj)
        {
            return true;
        }

        private void ShowAdminPanel(object obj)
        {
            var principal = Thread.CurrentPrincipal;

            // Check if the principal is a GenericPrincipal and has an identity
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {
                // Check if the user has the "Admin" role
                bool isAdmin = genericPrincipal.IsInRole("Admin");

                // Now isAdmin will be true if the user has the "Admin" role, otherwise false
                if (CanReadPermission("Admin panel"))
                {
                    CurrentChildView = new AdminPanelViewModel();
                    Caption = "Admin panel";
                    Icon = IconChar.Lock;
                }
                else
                {

                    MessageBox.Show("Nemate pravo pristupa", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            else
            {
                MessageBox.Show("Niste se pravilno prijavili", "Pristup odbijen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void ShowWorkingCard(object obj)
        {
            CurrentChildView = new WorkingCardViewModel();
            Caption = "Radna karta";
            Icon = IconChar.ClipboardCheck;
        }

        private void ExecuteShowDepartmentsViewCommand(object obj)
        {
            CurrentChildView = new DepartmentsViewModel();
            Caption = "Odjel";
            Icon = IconChar.Building;
        }

        private void ExecuteShowJobsViewCommand(object obj)
        {
            
            CurrentChildView = new JobsViewModel();
            Caption = "Radno mjesto";
            Icon = IconChar.Briefcase;
        }

       

        private void ExecuteShowWorkersViewCommand(object obj)
        {
            CurrentChildView = new WorkersViewModel();
            Caption = "Zaposlenici";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowProjectsViewCommand(object obj)
        {
            CurrentChildView = new ProjectsViewModel();
            Caption = "Projekti";
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
