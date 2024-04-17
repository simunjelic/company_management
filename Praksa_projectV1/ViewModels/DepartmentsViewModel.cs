using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Praksa_projectV1.ViewModels
{
    public class DepartmentsViewModel: ViewModelBase
    {

        private DepartmentRepository departmentRepository;
        public ICommand DeleteDepartmentCommand { get; }
        public ICommand ShowWindowCommand { get; }
        public ICommand AddDepartmentCommand { get; }
        public ICommand ShowUpdateWindowCommand { get; }
        public ICommand UpdateDepartmentCommand {  get; }

        public DepartmentsViewModel()
        {
           departmentRepository = new DepartmentRepository();
            GetAllDepartments();
            DeleteDepartmentCommand = new ViewModelCommand(DeleteDepartment, CanDeleteDepartment);
            ShowWindowCommand = new ViewModelCommand(ShowWindow, CanShowWindow);
            AddDepartmentCommand = new ViewModelCommand(AddDepartment, CanAddDepartment);
            ShowUpdateWindowCommand = new ViewModelCommand(ShowUpdateWindow, CanShowUpdateWindow);
            UpdateDepartmentCommand = new ViewModelCommand(UpdateDepartment, CanUpdateDepartmentCommand);
        }

        private bool CanUpdateDepartmentCommand(object obj)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }else return true;
        }

        private void UpdateDepartment(object obj)
        {
            Department department = new Department();
            department.Id = this.Id;
            department.Name = Name; 
            if(SelectedDepartment != null)
                department.ParentDepartmentId = SelectedDepartment.Id; ;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to update this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                bool isSuccessful = departmentRepository.Update(department);
                if (isSuccessful)
                {
                    string message = "Success!";
                    int index = -1;
                    index = DepartmentRecords.IndexOf(DepartmentRecords.Where(x => x.Id == Id).Single());
                    department.ParentDepartment = SelectedDepartment;
                    DepartmentRecords[index] = department;



                    MessageBox.Show(message);

                }
            }
        }

        private bool CanShowUpdateWindow(object obj)
        {
            return true;
        }

        private void ShowUpdateWindow(object obj)
        {
            if (obj is int Id)
            {
                var department = departmentRepository.GetDepart(Id);
                if(department.ParentDepartmentId != null)
                  SelectedDepartment = (Department)DepartmentRecords.Where(x => x.Id == department.ParentDepartmentId).Single();
                Name = department.Name;
                this.Id = Id;
                DepartmentsEditView view = new DepartmentsEditView();
                view.Title = "Update department";
                view.DataContext = this;
                view.Show();
            }
        }

        private bool CanAddDepartment(object obj)
        {
            if(!string.IsNullOrEmpty(Name))
            {
                return true;
            }else
            {
                return false;
            }
        }

        private void AddDepartment(object obj)
        {
            Department newDepartment = new();
            newDepartment.Name = Name;
            if (SelectedDepartment != null)
                newDepartment.ParentDepartmentId = SelectedDepartment.Id;
            bool flag = departmentRepository.Add(newDepartment);
            if(flag)
            {
                MessageBox.Show("Department with name "+Name+" added.");
                if(SelectedDepartment != null)
                newDepartment.ParentDepartment = departmentRepository.GetDepart(newDepartment.ParentDepartmentId);
                DepartmentRecords.Add(newDepartment);
                ResetData();
            }
            else {
                MessageBox.Show("Department with same name exist.");
            }

        }

        private bool CanShowWindow(object obj)
        {
            return true;
        }

        private void ShowWindow(object obj)
        {
            DepartmentsEditView view = new DepartmentsEditView();
            view.Title = "Add new department";
            ResetData();
            view.DataContext = this;
            view.Show();
        }

        private bool CanDeleteDepartment(object obj)
        {
            return true;
        }

        private void DeleteDepartment(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this department?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);


            if (obj is int Id && result == MessageBoxResult.Yes)
            {
                bool res = departmentRepository.Remove(Id);
                if (res)
                    DepartmentRecords.Remove(DepartmentRecords.Where(x => x.Id == Id).Single());
                else
                    MessageBox.Show("Delete all jobs associated with the selected department.");
            }
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
        private string _parentDepartment;
        public string ParentDepartment
        {
            get
            {
                return _parentDepartment;
            }
            set
            {
                _parentDepartment = value;
                OnPropertyChanged("ParentDepartment");
            }
        }
        private Department _selectedDepartment;

        public Department SelectedDepartment
        {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged("SelectedDepartment");
            }

        }
        private ObservableCollection<Department> _departmentRecords;
        public ObservableCollection<Department> DepartmentRecords
        {
            get
            {
                return _departmentRecords;
            }
            set
            {
                _departmentRecords = value;
                OnPropertyChanged("DepartmentRecords");
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

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
   

    public void GetAllDepartments()
        {
            var departments = departmentRepository.GetAllDepartments();
            foreach (var department in departments)
            {
                if (department.ParentDepartmentId != null) { 
                    department.ParentDepartment = departments.Where(i => i.Id == department.ParentDepartmentId).FirstOrDefault();
                }
            }
            DepartmentRecords = new ObservableCollection<Department>(departments);

        }
        public void ResetData()
        {
            Name = string.Empty;
            SelectedDepartment = null;
        }
    }
}
