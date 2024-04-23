using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Praksa_projectV1.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
    }
}
