using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Praksa_projectV1.Models;
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
            SetupExceptionHandling();
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

        private static void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            Current.DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void LogUnhandledException(Exception exception, string source)
        {
            using (var dbContext = new Context())
            {

                dbContext.ExceptionLogs.Add(new ExceptionLog
                {
                    Message = "User: " + LoggedUserData.Username + " Exception: " + exception.ToString(),
                    StackTrace = exception.StackTrace,
                    Source = source,
                    Timestamp = DateTime.UtcNow
                });
                dbContext.SaveChanges();
            }
        }
    }

}
