using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.UserVMs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Samara_Academy.VMs.ProfileVMs
{
    public class ChangeOwnPasswordVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private bool _isLoading;

        private string _newPassword;
        private string _confirmPassword;
        private string _currentPassword;

        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                OnPropertyChanged();
            }
        }
       
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand ChangeCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand GoBackCommand { get; }
        public ChangeOwnPasswordVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;

            ChangeCommand = new RelayCommand(Change);
            ClearCommand = new RelayCommand(Clear);
            GoBackCommand = new RelayCommand(GoBack);

            
            IsLoading = false;


        }

        private async void Change(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            if (ConfirmPassword!=NewPassword)
            {
                var dialogBox = new InfoDialog(DisplayMessages.PasswordDoNotMatch);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmChangePassword);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }
            
            IsLoading = true;
            bool success = false;

            User user = new User();

            await Task.Run(() => user = new UserManager().GetFullUserByID(SharedResources.UserID));

            if(string.IsNullOrEmpty(user.Password))
            {
                IsLoading = false;
                var dialogBox = new InfoDialog(DisplayMessages.ConnectionFailure);
                dialogBox.ShowDialog();
                return;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(CurrentPassword, user.Password);
            
            if (isPasswordValid)
            {
                await Task.Run(() => 
                {
                    success = new UserManager().UpdatePassword(SharedResources.UserID, BCrypt.Net.BCrypt.HashPassword(NewPassword));
                });
            }
            else
            {
                IsLoading = false;
                var dialogBox = new InfoDialog(DisplayMessages.InvalidPassword);
                dialogBox.ShowDialog();
                return;
            }

            if (success)
            {
                Clear(null);
            }

            IsLoading = false;

        }
        private void Clear(object parameter)
        {
            CurrentPassword = "";
            NewPassword = "";
            ConfirmPassword = "";
        }

        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new ProfileVM(_navigationVM);

        }
        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
