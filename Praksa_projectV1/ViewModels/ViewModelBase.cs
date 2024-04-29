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
            var principal = Thread.CurrentPrincipal;
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {

                List<Permission> permList = PermissonRepository.getPermissionByModule(modul,3);
                if (permList != null)
                {
                    foreach (Permission permItem in permList)
                    {
                        if (principal.IsInRole(permItem.Role.RoleName))
                            return true;
                    }
                }
                else return false;
            }
            return false;
        }
        public bool CanDeletePermission(string modul)
        {
            var principal = Thread.CurrentPrincipal;
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {

                List<Permission> permList = PermissonRepository.getPermissionByModule(modul,2);
                if (permList != null)
                {
                    foreach (Permission permItem in permList)
                    {
                        if (principal.IsInRole(permItem.Role.RoleName))
                            return true;
                    }
                }
                else return false;
            }
            return false;
        }
        public bool CanUpdatePermission(string modul)
        {
            var principal = Thread.CurrentPrincipal;
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {

                List<Permission> permList = PermissonRepository.getPermissionByModule(modul, 1);
                if (permList != null)
                {
                    foreach (Permission permItem in permList)
                    {
                        if (principal.IsInRole(permItem.Role.RoleName))
                            return true;
                    }
                }
                else return false;
            }
            return false;
        }
        public bool CanCreatePermission(string modul)
        {
            var principal = Thread.CurrentPrincipal;
            if (principal is GenericPrincipal genericPrincipal && genericPrincipal.Identity != null)
            {
                
                List<Permission> permList = PermissonRepository.getPermissionByModule(modul, 0);
                if (permList != null)
                {
                    foreach (Permission permItem in permList)
                    {
                        if (principal.IsInRole(permItem.Role.RoleName))
                            return true;
                    }
                }
                else return false;
            }
            return false;
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
       
       
    }
}
