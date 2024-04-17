using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.ViewModels
{
   
    public class WorkersViewModel: ViewModelBase
    {
        EmpolyeeRepository EmpolyeeRepository { get; }


        public WorkersViewModel()
        {
            EmpolyeeRepository = new EmpolyeeRepository();
            GetAllWorkers();
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

        public void GetAllWorkers()
        {
            var employes = EmpolyeeRepository.GetAll();

         

            WorkersRecords = new ObservableCollection<Employee>(employes);

        }
    }
}
