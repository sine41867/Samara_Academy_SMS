using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.StudentVMs
{
    public class AddStudentVM : VMBase
    {
        private readonly NavigationVM _navigationVM;

        private ObservableCollection<Class> _enrolledClasses;

        private bool _isLoading;

        private string _name;
        private string _mobile;
        private string _whatsapp;
        private DateTime _dateOfRegistration;
        private string _classId;

        public string ClassID
        {
            get => _classId;
            set
            {
                _classId = value;
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

        public ObservableCollection<Class> EnrolledClasses
        {
            get => _enrolledClasses;
            set
            {
                _enrolledClasses = value;
                OnPropertyChanged();
            }
        }

        public ICommand EnrollCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand AddtoEnrollCommand { get; }

        public ICommand RemoveFromEnrollCommand { get; }
        public ICommand GoBackCommand { get; }
        public AddStudentVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
            EnrollCommand = new RelayCommand(Enroll);
            AddCommand = new RelayCommand(Add);
            ClearCommand = new RelayCommand(Clear);
            AddtoEnrollCommand = new RelayCommand(AddtoEnroll);
            RemoveFromEnrollCommand = new RelayCommand(RemoveFromEnroll);
            GoBackCommand = new RelayCommand(GoBack);

            EnrolledClasses = new ObservableCollection<Class>();
            DateOfRegistration = DateTime.Now;
            ClassID = "";
            IsLoading = false;


        }

        private async void AddtoEnroll(object parameter)
        {

            if (StringFormatValidator.IsValidClassID(ClassID))
            {
                IsLoading = true;

                Class clz = new Class();

                bool isReturned = false;

                await Task.Run(() =>
                {
                    foreach (Class cls in EnrolledClasses)
                    {
                        if (cls.ClassID.Equals(ClassID))
                        {
                            IsLoading = false;


                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                var dialogBox = new InfoDialog(DisplayMessages.SameClassID);
                                dialogBox.ShowDialog();

                            });

                            isReturned = true;
                            return;
                        }
                    }

                    if (isReturned)
                    {
                        return;
                    }

                    clz = new ClassManager().GetClassByID(ClassID);
                    clz.InsertedBy = SharedResources.UserID;

                });

                if (isReturned)
                {
                    return;
                }

                if (string.IsNullOrEmpty(clz.Name))
                {
                    IsLoading = false;
                    var dialogBox = new InfoDialog(DisplayMessages.InvalidClassID);
                    dialogBox.ShowDialog();

                    return;


                }

                EnrolledClasses.Add(clz);
                ClassID = "";
                IsLoading = false;

            }
            else
            {
                var dialogBox = new InfoDialog(DisplayMessages.InvalidClassID);
                dialogBox.ShowDialog();
            }
        }

        private void RemoveFromEnroll(object parameter)
        {
            if (parameter is Class cls)
            {
                EnrolledClasses.Remove(cls);
            }
        }
        private async void Add(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmAddStudent);
            dialog.ShowDialog();

            if(dialog.DialogResult == false)
            {
                return;
            }


            Student student = new Student()
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
                success = new StudentManager().AddStudent(student, EnrolledClasses);

            });

            if (success)
            {
                Clear(null);
            }

            IsLoading = false;

        }
        private void Enroll(object parameter)
        {
            SeacrchClassView window = new SeacrchClassView();
            window.ShowDialog();
        }

        private void Clear(object parameter)
        {
            Name = "";
            Mobile = "";
            WhatsApp = "";
            DateOfRegistration = DateTime.Now;
            ClassID = "";
            EnrolledClasses.Clear();
        }

        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new StudentVM(_navigationVM);

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
