using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
   
    public class WorkersViewModel: ViewModelBase
    {
        EmpolyeeRepository EmpolyeeRepository { get; }
        public ICommand ShowWindowCommand { get; }
        public ICommand DeleteCommand { get; }


        public WorkersViewModel()
        {
            EmpolyeeRepository = new EmpolyeeRepository();
            GetAllWorkers();
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);
        }

        private bool CanDelete(object obj)
        {
            if(SelectedItem!=null)
                return true;
            return false;
        }

        private void Delete(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Employee with name: "+SelectedItem.Name +" "+SelectedItem.Surname+"?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                EmpolyeeRepository.DeleteById(SelectedItem.Id);
                WorkersRecords.Remove(SelectedItem);
                
            }
            SelectedItem = null;
        }

        private bool CanShowWindow(object obj)
        {
            return true;
        }

        private void ShowWindow(object obj)
        {

            MessageBox.Show("Delete all jobs associated with the selected department.");
            
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
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        private ObservableCollection<Employee> _workersRecords;
        public ObservableCollection<Employee> WorkersRecords
        {
            get
            {
                return _workersRecords;
            }
            set
            {
                _workersRecords = value;
                OnPropertyChanged(nameof(WorkersRecords));
            }
        }
        private Employee _selectedItem;
    public Employee? SelectedItem
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

        public void GetAllWorkers()
        {
            var employes = EmpolyeeRepository.GetAll();

         

            WorkersRecords = new ObservableCollection<Employee>(employes);

        }
    }
}
