using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Praksa_projectV1.DataAccess;
using System.Security.Principal;
using Praksa_projectV1.Models;
using Microsoft.IdentityModel.Tokens;
using Praksa_projectV1.Enums;

namespace Praksa_projectV1.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public PermissonRepository PermissonRepository = new PermissonRepository();





        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        Dictionary<string, List<string>> Erorrs = new Dictionary<string, List<string>>();
        public bool HasErrors => Erorrs.Count > 0;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public IEnumerable GetErrors(string? propertyName)
        {
            if (Erorrs.ContainsKey(propertyName))
            {
                return Erorrs[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }
        public void Validate(string propertyName, object propertyValue)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(propertyValue, new ValidationContext(this) { MemberName = propertyName }, results);

            if (results.Any())
            {
                try
                {
                    Erorrs.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                }
                catch { }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                Erorrs.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

        }

        public bool CanReadPermission(string modul)
        {
            if (PermissionAccess.ReadPermission.IsNullOrEmpty())
            {
                PermissionAccess.ReadPermission = PermissonRepository.GetUserRoles((int)AvailableActions.Čitaj, LoggedUserData.RolesId);
                return PermissionAccess.ReadPermission.Any(i => i.Module.Name == modul);
            }
            else
            {
                return PermissionAccess.ReadPermission.Any(i => i.Module.Name == modul);
            }

        }
        public bool CanDeletePermission(string modul)
        {
            if (PermissionAccess.DeletePermission.IsNullOrEmpty())
            {
                PermissionAccess.DeletePermission = PermissonRepository.GetUserRoles((int)AvailableActions.Obriši, LoggedUserData.RolesId);
                return PermissionAccess.DeletePermission.Any(i => i.Module.Name == modul);
            }
            else
            {
                return PermissionAccess.DeletePermission.Any(i => i.Module.Name == modul);
            }
        }
        public bool CanUpdatePermission(string modul)
        {
            if (PermissionAccess.UpdatePermission.IsNullOrEmpty())
            {
                PermissionAccess.UpdatePermission = PermissonRepository.GetUserRoles((int)AvailableActions.Uredi, LoggedUserData.RolesId);
                return PermissionAccess.UpdatePermission.Any(i => i.Module.Name == modul);
            }
            else
            {
                return PermissionAccess.UpdatePermission.Any(i => i.Module.Name == modul);
            }
        }
        public bool CanCreatePermission(string modul)
        {
            if (PermissionAccess.CreatePermission.IsNullOrEmpty())
            {
                PermissionAccess.CreatePermission = PermissonRepository.GetUserRoles((int)AvailableActions.Dodaj, LoggedUserData.RolesId);
                return PermissionAccess.CreatePermission.Any(i => i.Module.Name == modul);
            }
            else
            {
                return PermissionAccess.CreatePermission.Any(i => i.Module.Name == modul);
            }
        }

        private bool _isUpdateButtonVisible = true;

        public bool IsUpdateButtonVisible
        {
            get { return _isUpdateButtonVisible; }
            set
            {
                if (_isUpdateButtonVisible != value)
                {
                    _isUpdateButtonVisible = value;
                    OnPropertyChanged(nameof(IsUpdateButtonVisible));
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
                    OnPropertyChanged(nameof(IsAddButtonVisible));
                }
            }
        }


    }
}
