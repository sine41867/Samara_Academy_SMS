using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Samara_Academy.VMs.UserVMs
{
    public class AddUserVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private bool _isLoading;

        private string _name;
        private string _mobile;
        private string _userId;
        private DateTime _dateOfRegistration;
        private string _password;
        private string _userRole;
        
        private ObservableCollection<string> _userRoles;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
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

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
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
        public string UserID
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }
        public DateTime DateOfRegistration
        {
            get => _dateOfRegistration;
            set
            {
                _dateOfRegistration = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserRoles
        {
            get => _userRoles;
            set
            {
                _userRoles = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand GoBackCommand { get; }
        public AddUserVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
            AddCommand = new RelayCommand(Add);
            ClearCommand = new RelayCommand(Clear);
            GoBackCommand = new RelayCommand(GoBack);

            UserRoles = new ObservableCollection<string>(["Admin", "Assistant"]);
            Role = UserRoles[1];
            DateOfRegistration = DateTime.Now;
            
            IsLoading = false;


        }

        private async void Add(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmAddUser);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }


            User user = new User()
            {
                Name = Name,
                Mobile = Mobile,
                UserID = UserID,
                Password = BCrypt.Net.BCrypt.HashPassword(Password),
                DateOfRegistration = DateOfRegistration.ToString("yyyy-MM-dd"),
                UserRole = Role,
                InsertedBy = SharedResources.UserID

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new UserManager().AddUser(user);

            });

            if (success)
            {
                Clear(null);
            }

            IsLoading = false;

        }
        private void Clear(object parameter)
        {
            Name = "";
            Mobile = "";
            UserID = "";
            DateOfRegistration = DateTime.Now;
            Password = "";
            Role = UserRoles[1];

        }

        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new UserVM(_navigationVM);

        }
        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Mobile) || string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(DateOfRegistration.ToString()) || string.IsNullOrEmpty(Role))
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
