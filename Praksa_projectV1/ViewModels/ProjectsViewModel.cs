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
    public class ProjectsViewModel : ViewModelBase
    {
        ProjectRepository ProjectRepository;
        public ICommand DeleteCommand { get; }


        public ProjectsViewModel()
        {
            ProjectRepository = new ProjectRepository();
            gatAllProjects();
            DeleteCommand = new ViewModelCommand(Delete, CanDelete);


        }

        private bool CanDelete(object obj)
        {
            if (SelectedItem != null)
                return true;
            return false;
        }

        private void Delete(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Project with name: " + SelectedItem.Name +"?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
               ProjectRepository.DeleteById(SelectedItem.Id);
               ProjectRecords.Remove(SelectedItem);



            }
            SelectedItem = null;
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

        public void gatAllProjects()
        {
            ProjectRecords = new ObservableCollection<Project>(ProjectRepository.GetAll());

        }
    }
}
