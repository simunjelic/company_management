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
    /// Interaction logic for UserRolesView.xaml
    /// </summary>
    public partial class UserRolesView : Window
    {
        public UserRolesView()
        {
            InitializeComponent();
            this.DataContext = new UserViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserViewModel userViewModel = new();
            this.DataContext = userViewModel;
        }
    }
}
