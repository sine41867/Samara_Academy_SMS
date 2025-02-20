using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class LoginVM:VMBase
    {
        private string _userId;
        private string _password;
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {

                _isLoading = value;
                OnPropertyChanged();

            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginVM()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private async void ExecuteLogin(object parameter)
        {
            
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password))
            {
                var dialogBox = new InfoDialog("Fill all the fields...");
                dialogBox.ShowDialog();
                return;
            }

            IsLoading = true;
            User user = new User();
            
            await Task.Run(() => user = new UserManager().GetFullUserByID(UserId));

            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                IsLoading = false;
                var dialogBox = new InfoDialog("Invalid username or password.");
                dialogBox.ShowDialog();
                return;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);

            if (isPasswordValid && !user.IsDisable)
            { 
                SharedResources.UserID = user.UserID;

                SharedResources.LoggedUser = user;

                Window mainWindow = new MainWindow();
                IsLoading = false;

                mainWindow.Show();


                Application.Current.Windows[0]?.Close();
            }
            else if (isPasswordValid && user.IsDisable)
            {
                IsLoading = false;
                var dialogBox = new InfoDialog("Your account is deactivated.");
                dialogBox.ShowDialog();
            }

            else
            {
                IsLoading = false;
                
                var dialogBox = new InfoDialog("Invalid username or password.");
                dialogBox.ShowDialog();
            }
            
        }


    }
}
