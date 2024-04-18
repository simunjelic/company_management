﻿using Praksa_projectV1.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Praksa_projectV1.Views
{
    /// <summary>
    /// Interaction logic for ProjectsView.xaml
    /// </summary>
    public partial class ProjectsView : UserControl
    {
        public ProjectsView()
        {
            InitializeComponent();

            this.DataContext = new ProjectsViewModel();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                // Get the selected item
                var selectedItem = (Project)grid.SelectedItem;
                if (selectedItem != null)
                {
                    // Set the selected item in the view model
                    if (DataContext is ProjectsViewModel viewModel)
                    {
                        viewModel.SelectedItem = selectedItem;
                    }
                }
            }

        }
    }
}
