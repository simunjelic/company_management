﻿using Praksa_projectV1.ViewModels;
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
    /// Interaction logic for DepartmentsEditView.xaml
    /// </summary>
    public partial class DepartmentsEditView : Window
    {
        public DepartmentsEditView()
        {
            InitializeComponent();
            DepartmentsViewModel viewModel = new DepartmentsViewModel();
            DataContext = viewModel;
        }

    }
}