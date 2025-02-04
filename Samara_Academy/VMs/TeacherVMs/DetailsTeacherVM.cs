using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
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

namespace Samara_Academy.VMs.TeacherVMs
{
    public class DetailsTeacherVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private ObservableCollection<Class> _classes;
        private ObservableCollection<Class> _teachingClasses;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private bool _isModifyMode;

        private string _dateOfRegistration;
        private string _dateOfModified;


        private Teacher _teacher;

        private string _insertedBy;
        private string _teacherID;
        private string _name;
        private string _mobile;
        private string _whatsapp;
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
        public string TeacherID
        {
            get => _teacherID;
            set
            {
                _teacherID = value;
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

        public ObservableCollection<Class> TeachingClasses
        {
            get => _teachingClasses;
            set
            {
                _teachingClasses = value;
                OnPropertyChanged();
            }
        }

        public ICommand ModifyCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ReloadClassesCommand { get; }

        public ICommand CancelCommand { get; }
        
        public ICommand RemoveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand GoBackCommand { get; }
        public DetailsTeacherVM(NavigationVM navigationVM, string teacherD)
        {
            _navigationVM = navigationVM;
            TeacherID = teacherD;

            ModifyCommand = new RelayCommand(Modify);
            ReloadCommand = new RelayCommand(LoadData);
            ReloadClassesCommand = new RelayCommand(LoadClasses);
            CancelCommand = new RelayCommand(Cancel);
            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommand = new RelayCommand(Update);
            RemoveCommand = new RelayCommand(Remove);

            TeachingClasses = new ObservableCollection<Class>();
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

                _teacher = new TeacherManager().GetTeacherByID(TeacherID);

                if (_teacher == null)
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }

                TeachingClasses = new ClassManager().GetClassesByTeacherID(TeacherID);

                if (TeachingClasses == null)
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }

                Name = _teacher.Name;
                Mobile = _teacher.Mobile;
                WhatsApp = _teacher.WhatsApp;
                DateOfRegistration = _teacher.RegisteredDate;
                DateOfModified = _teacher.LastModifiedDate;
                InsertedBy = _teacher.InsertedBy;




            });

            IsLoading = false;

        }
        private async void LoadClasses(object parameter = null)
        {
            IsLoading = true;

            await Task.Run(() =>
            {


                _classes = new ClassManager().GetClassesByTeacherID(TeacherID);

                if (_classes == null)
                {
                    IsLoading = false;
                    return;
                }

                TeachingClasses = _classes;



            });

            IsLoading = false;

        }

        private async void Remove(object parameter)
        {
            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmRemoveTeacher);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            IsLoading = true;
            bool success = false;

            await Task.Run(() =>
            {
                success = new TeacherManager().DeleteTeacher(TeacherID);

            });

            if (success)
            {
                _navigationVM.CurrentView = new TeacherVM(_navigationVM);
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

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmUpdateTeacher);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            Teacher teacher = new Teacher()
            {
                TeacherID = TeacherID,
                Name = Name,
                Mobile = Mobile,
                WhatsApp = WhatsApp,

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new TeacherManager().UpdateTeacher(teacher);

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
        
        private void GoBack(object parameter)
        {
            _navigationVM.CurrentView = new TeacherVM(_navigationVM);

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
