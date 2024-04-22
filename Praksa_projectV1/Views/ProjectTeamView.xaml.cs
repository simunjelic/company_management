using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Praksa_projectV1.Views
{
    /// <summary>
    /// Interaction logic for ProjectTeamView.xaml
    /// </summary>
    public partial class ProjectTeamView : Window
    {
        public ProjectTeamView()
        {
            InitializeComponent();
            DataContext = new ProjectsViewModel();

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                // Get the selected item
                var selectedEmployee = (EmployeeProject)grid.SelectedItem;
                if (selectedEmployee != null)
                {
                    // Set the selected item in the view model
                    if (DataContext is ProjectsViewModel viewModel)
                    {
                        viewModel.SelectedEmployee = selectedEmployee;
                    }
                }
            }
        }

      
    }
}
