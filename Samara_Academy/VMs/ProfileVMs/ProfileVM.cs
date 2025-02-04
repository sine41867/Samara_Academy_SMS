using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.TeacherVMs;
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
    public class ProfileVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private bool _isModifyMode;

        private string _dateOfRegistration;

        private User _user;

        private string _insertedBy;
        private string _userId;
        private string _name;
        private string _mobile;
        private string _userRole;
        private string _classId;
        private string _saveButtonVisibility;
        private string _modifyButtonVisibility;

        public string InsertedBy
        {
            get => _insertedBy;
            set
            {
                _insertedBy = value;
                OnPropertyChanged();
            }
        }
        public string SaveButtonVisibility
        {
            get => _saveButtonVisibility;
            set
            {
                _saveButtonVisibility = value;
                OnPropertyChanged();
            }
        }
        public string ModifyButtonVisibility
        {
            get => _modifyButtonVisibility;
            set
            {
                _modifyButtonVisibility = value;
                OnPropertyChanged();
            }
        }
        public string UserID
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }
        public bool IsModifyMode
        {
            get => _isModifyMode;
            set
            {
                _isModifyMode = value;
                OnPropertyChanged();
                if (_isModifyMode)
                {
                    ModifyButtonVisibility = "Collapsed";
                    SaveButtonVisibility = "Visible";
                }
                else
                {
                    SaveButtonVisibility = "Collapsed";
                    ModifyButtonVisibility = "Visible";
                }
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
        public bool IsLoadingFailed
        {
            get => _isLoadingFailed;
            set
            {
                _isLoadingFailed = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Mobile
        {
            get => _mobile;
            set
            {
                _mobile = value;
                OnPropertyChanged();
            }
        }
        public string Role
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnPropertyChanged();
            }
        }
        public string DateOfRegistration
        {
            get => _dateOfRegistration;
            set
            {
                _dateOfRegistration = value;
                OnPropertyChanged();
            }
        }

        public ICommand ModifyCommand { get; }
      
        public ICommand CancelCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand UpdateCommand { get; }
        public ProfileVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
            UserID = SharedResources.UserID;
            ReloadCommand = new RelayCommand(LoadData);
            ModifyCommand = new RelayCommand(Modify);
            CancelCommand = new RelayCommand(Cancel);
            UpdateCommand = new RelayCommand(Update);
            ChangePasswordCommand = new RelayCommand(ChangePassword);

            IsModifyMode = false;
            IsLoadingFailed = false;
            IsLoading = true;
            LoadData();

        }

        private async void LoadData(object parameter = null)
        {
            IsLoading = true;
            IsLoadingFailed = false;

            await Task.Run(() =>
            {

                _user = new UserManager().GetUserByID(UserID);

                if (_user == null)
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }

                Name = _user.Name;
                Mobile = _user.Mobile;
                Role = _user.UserRole;
                DateOfRegistration = _user.DateOfRegistration;
                UserID = _user.UserID;
                InsertedBy = _user.InsertedBy;
            });

            IsLoading = false;

        }

        private async void ChangePassword(object parameter)
        {
            _navigationVM.CurrentView = new ChangeOwnPasswordVM(_navigationVM);
        }
        private async void Update(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmUpdateUser);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            User user = new User()
            {
                UserID = UserID,
                Mobile = Mobile
            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new UserManager().UpdateUser(user);

            });

            if (success)
            {
                Cancel(null);
            }

            IsLoading = false;

        }
        private void Modify(object parameter)
        {
            IsModifyMode = true;
        }

        private void Cancel(object parameter)
        {
            IsModifyMode = false;
        }

        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Mobile))
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
