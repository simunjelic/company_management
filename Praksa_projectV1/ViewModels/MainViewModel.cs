using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Praksa_projectV1.Models;
using Praksa_projectV1.DataAccess;
using System.Threading;

namespace Praksa_projectV1.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private IUserRepository userRepository;

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

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();
            LoadCurrentUserData();
        }

        private void LoadCurrentUserData()
        {
            
            
                CurrentUserAccount.Username = Thread.CurrentPrincipal.Identity.Name;
                CurrentUserAccount.DisplayName = $"Welcome {Thread.CurrentPrincipal.Identity.Name} ;)";
                CurrentUserAccount.ProfilePicture = null;
            
        }

    }
}
