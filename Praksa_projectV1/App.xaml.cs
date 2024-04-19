using System.Configuration;
using System.Data;
using System.Windows;
using Praksa_projectV1.Views;

namespace Praksa_projectV1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            loginView.IsVisibleChanged += (s, ev) =>
            {
                if (loginView.IsVisible == false && loginView.IsLoaded)
                {
                    try
                    {
                        var mainView = new MainView();
                        mainView.Show();
                        loginView.Close();
                    }catch(Exception ex) { }
                }
            };
        }
    }

}
