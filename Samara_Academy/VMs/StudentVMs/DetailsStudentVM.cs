using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.StudentVMs
{
    public class DetailsStudentVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private ObservableCollection<Class> _classes;
        private ObservableCollection<Class> _enrolledClasses;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private bool _isModifyMode;
        private bool _isLeaveMode;

        private string _dateOfRegistration;
        private string _dateOfModified;


        private Student _student;

        private string _insertedBy;
        private string _studentID;
        private string _name;
        private string _mobile;
        private string _whatsapp;
        private string _saveButtonVisibility;
        private string _modifyButtonVisibility;
        private string _leaveButtonVisibility;
        private string _leaveColumnVisibility;


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
        public string LeaveColumnVisibility
        {
            get => _leaveColumnVisibility;
            set
            {
                _leaveColumnVisibility = value;
                OnPropertyChanged();
            }
        }
        public string LeaveButtonVisibility
        {
            get => _leaveButtonVisibility;
            set
            {
                _leaveButtonVisibility = value;
                OnPropertyChanged();
            }
        }
        public string StudentID
        {
            get => _studentID;
            set
            {
                _studentID = value;
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
                if(_isModifyMode)
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
        public bool IsLeaveMode
        {
            get => _isLeaveMode;
            set
            {
                _isLeaveMode = value;
                OnPropertyChanged();
                if (_isLeaveMode)
                {
                    LeaveButtonVisibility = "Collapsed";
                    LeaveColumnVisibility = "Visible";
                }
                else
                {
                    LeaveColumnVisibility = "Collapsed";
                    LeaveButtonVisibility = "Visible";
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
        public string WhatsApp
        {
            get => _whatsapp;
            set
            {
                _whatsapp = value;
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
        public string DateOfModified
        {
            get => _dateOfModified;
            set
            {
                _dateOfModified = value;
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

        public ICommand ModifyCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ReloadClassesCommand { get; }
        
        public ICommand CancelCommand { get; }
        public ICommand CancelLeaveCommand { get; }
        public ICommand LeaveCommand { get; }
        public ICommand EnrollCommand { get; }

        public ICommand RemoveEnrollmentCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand GoBackCommand { get; }
        public DetailsStudentVM(NavigationVM navigationVM, string studentID)
        {
            _navigationVM = navigationVM;
            StudentID = studentID;

            ModifyCommand = new RelayCommand(Modify);
            ReloadCommand = new RelayCommand(LoadData);
            ReloadClassesCommand = new RelayCommand(LoadClasses);
            CancelCommand = new RelayCommand(Cancel);
            EnrollCommand = new RelayCommand(Enroll);
            CancelLeaveCommand = new RelayCommand(CancelLeave);
            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommand = new RelayCommand(Update);
            LeaveCommand = new RelayCommand(Leave);
            RemoveCommand = new RelayCommand(Remove);
            RemoveEnrollmentCommand = new RelayCommand(RemoveEnrollment);

            EnrolledClasses = new ObservableCollection<Class>();
            IsModifyMode = false;
            IsLeaveMode = false;
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

                _student = new StudentManager().GetStudentByID(StudentID);

                if (_student == null)
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }

                EnrolledClasses = new EnrollmentManager().GetEnrollmentsByStudentID(StudentID);

                if(EnrolledClasses == null)
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }

                Name = _student.Name;
                Mobile = _student.Mobile;
                WhatsApp = _student.WhatsApp;
                DateOfRegistration = _student.RegisteredDate;
                DateOfModified = _student.LastModifiedDate;
                InsertedBy = _student.InsertedBy;



                IsLoading = false;

                
            });

            IsLoading = false;

        }
        private async void LoadClasses(object parameter = null)
        {
            IsLoading = true;

            await Task.Run(() =>
            {


                _classes = new EnrollmentManager().GetEnrollmentsByStudentID(StudentID);

                if (_classes == null)
                {
                    IsLoading = false;
                    return;
                }

                EnrolledClasses = _classes;

                IsLoading = false;


            });

            IsLoading = false;

        }

        private async void Leave(object parameter)
        {
            if(EnrolledClasses.Count > 0)
            {
                IsLeaveMode = true;
            }
            
        }

        private async void RemoveEnrollment(object parameter)
        {
            
            if (parameter is Class cls)
            {
                Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmLeaveStudentFromClass);
                dialog.ShowDialog();

                if (dialog.DialogResult == false)
                {
                    return;
                }

                IsLoading = true;
                bool success = false;

                await Task.Run(() =>
                {
                    success = new EnrollmentManager().DeleteEnrollment(StudentID, cls.ClassID);

                });

                if (success)
                {
                    EnrolledClasses.Remove(cls);
                    CancelLeave(null);
                }

                IsLoading = false;

                
            }
        }

        private async void Remove(object parameter)
        {
            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmRemoveStudent);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            IsLoading = true;
            bool success = false;

            await Task.Run(() =>
            {
                success = new StudentManager().DeleteStudent(StudentID);

            });

            if (success)
            {
                _navigationVM.CurrentView = new StudentVM(_navigationVM);
            }

            IsLoading = false;

        }
        private async void Update(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmUpdateStudent);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            Student student = new Student()
            {
                StudentID = StudentID,
                Name = Name,
                Mobile = Mobile,
                WhatsApp = WhatsApp,

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new StudentManager().UpdateStudent(student);

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
        private void CancelLeave(object parameter)
        {
            IsLeaveMode = false;
        }

        private void Enroll(object parameter)
        {
            EnrollStudentsView window = new EnrollStudentsView(StudentID);
            window.ShowDialog();

        }
        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new StudentVM(_navigationVM);

        }
        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Mobile) || string.IsNullOrEmpty(WhatsApp))
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
