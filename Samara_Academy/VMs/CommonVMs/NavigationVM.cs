using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.VMs.ClassVMs;
using Samara_Academy.VMs.LogVMs;
using Samara_Academy.VMs.ProfileVMs;
using Samara_Academy.VMs.StudentVMs;
using Samara_Academy.VMs.TeacherVMs;
using Samara_Academy.VMs.UserVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class NavigationVM : VMBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand NavigateCommand { get; }

        public string CurrentUserId { get; set; }

        public NavigationVM()
        {
            NavigateCommand = new RelayCommand(Navigate);
            CurrentView = new DashboardVM(this);

        }

        private void Navigate(object parameter)
        {

            switch (parameter)
            {
                case "DashboardVM":
                    CurrentView = new DashboardVM(this);
                    break;

                case "StudentVM":
                    CurrentView = new StudentVM(this);
                    break;

                case "ClassVM":
                    CurrentView = new ClassVM(this);
                    break;

                case "TeacherVM":
                    CurrentView = new TeacherVM(this);
                    break;

                case "LogVM":
                    CurrentView = new LogVM(this);
                    break;

                case "UserVM":
                    CurrentView = new UserVM(this);
                    break;

                case "ProfileVM":
                    CurrentView = new ProfileVM(this);
                    break;

                case "LogoutVM":
                    SharedResources.UserID = "";
                    SharedResources.LoggedUser = new User();
                    LoginWindow window = new LoginWindow();

                    window.Show();

                    Application.Current.Windows[0]?.Close();

                    break;
            }

        }
    }
}
