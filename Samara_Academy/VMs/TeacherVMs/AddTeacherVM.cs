using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.TeacherVMs
{
    public class AddTeacherVM : VMBase
    {
        private readonly NavigationVM _navigationVM;

        private bool _isLoading;

        private string _name;
        private string _mobile;
        private string _whatsapp;
        private DateTime _dateOfRegistration;
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
        public string WhatsApp
        {
            get => _whatsapp;
            set
            {
                _whatsapp = value;
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

        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand GoBackCommand { get; }
        public AddTeacherVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
            AddCommand = new RelayCommand(Add);
            ClearCommand = new RelayCommand(Clear);
            GoBackCommand = new RelayCommand(GoBack);

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

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmAddTeacher);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            Teacher teacher = new Teacher()
            {
                Name = Name,
                Mobile = Mobile,
                WhatsApp = WhatsApp,
                RegisteredDate = DateOfRegistration.ToString("yyyy-MM-dd"),
                InsertedBy = SharedResources.UserID

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new TeacherManager().AddTeacher(teacher);

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
            WhatsApp = "";
            DateOfRegistration = DateTime.Now;
        }

        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new TeacherVM(_navigationVM);

        }
        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Mobile) || string.IsNullOrEmpty(WhatsApp) || string.IsNullOrEmpty(DateOfRegistration.ToString()))
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
