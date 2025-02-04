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

namespace Samara_Academy.VMs.ClassVMs
{
    public class DetailsClassVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private List<string> _days;
        private bool _isTeacherIDSelected;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private bool _isModifyMode;

        private string _dateOfRegistration;
        private string _dateOfModified;

        private Class _class;

        private string _insertedBy;
        private string _name;
        private string _classId;
        private string _time;
        private string _teacherId;
        private string _selectedTeacherId;
        private string _day;
        private float _fee;
        private float _margin;

        private string _saveButtonVisibility;
        private string _modifyButtonVisibility;
        public string ClassID
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged();
            }
        }
        public string InsertedBy
        {
            get => _insertedBy;
            set
            {
                _insertedBy = value;
                OnPropertyChanged();
            }
        }
        public string TeacherID
        {
            get => _teacherId;
            set
            {
                _teacherId = value;
                OnPropertyChanged();
            }
        }
        public string Day
        {
            get => _day;
            set
            {
                _day = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTeacherID
        {
            get => _selectedTeacherId;
            set
            {
                _selectedTeacherId = value;
                OnPropertyChanged();
            }
        }

        public bool IsTeacherIDSelected
        {
            get => _isTeacherIDSelected;
            set
            {
                _isTeacherIDSelected = value;
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
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        public float Fee
        {
            get => _fee;
            set
            {
                _fee = value;
                OnPropertyChanged();
            }
        }
        public float Margin
        {
            get => _margin;
            set
            {
                _margin = value;
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
        public bool IsLoadingFailed
        {
            get => _isLoadingFailed;
            set
            {
                _isLoadingFailed = value;
                OnPropertyChanged();
            }
        }
        public List<string> Days
        {
            get => _days;
            set
            {
                _days = value;
                OnPropertyChanged();
            }
        }

        public ICommand ModifyCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchTeachersCommand { get; }
        public ICommand ViewStudentsCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand SelectTeacherIDCommand { get; }
        public ICommand ClearTeacherIDCommand { get; }
        public DetailsClassVM(NavigationVM navigationVM, string classID)
        {
            _navigationVM = navigationVM;
            ClassID = classID;

            ModifyCommand = new RelayCommand(Modify);
            ReloadCommand = new RelayCommand(LoadData);
            CancelCommand = new RelayCommand(Cancel);
            SearchTeachersCommand = new RelayCommand(SearchTeacher);
            ViewStudentsCommand = new RelayCommand(ViewStudents);
            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommand = new RelayCommand(Update);
            RemoveCommand = new RelayCommand(Remove);
            SelectTeacherIDCommand = new RelayCommand(SelectTeacher);
            ClearTeacherIDCommand = new RelayCommand(ClearTeacher);

            Days = new List<string>()
            {
               "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"

            };

            IsTeacherIDSelected = true;
            IsModifyMode = false;
            IsLoadingFailed = false;
            IsLoading = true;
            LoadData();

        }
        private void SearchTeacher(object parameter)
        {
            ////SearchClass window = new SearchClass();
            ////window.ShowDialog();
            ///
            MessageBox.Show("Not Implemented Yet...");
        }
        private void ViewStudents(object parameter)
        {
            _navigationVM.CurrentView = new ClassStudentsVM(_navigationVM, ClassID);

        }

        private async void LoadData(object parameter = null)
        {
            IsLoading = true;
            IsLoadingFailed = false;

            await Task.Run(() =>
            {

                _class = new ClassManager().GetClassByID(ClassID);

                if (_class == null || string.IsNullOrEmpty(_class.ClassID))
                {
                    IsLoadingFailed = true;
                    IsLoading = false;
                    return;
                }
               
                Name = _class.Name;
                Fee = _class.Fee;
                Margin = _class.Margin;
                Day = _class.Day;
                Time = _class.Time;
                TeacherID = _class.TeacherID;
                SelectedTeacherID = TeacherID;
                IsTeacherIDSelected = true;
                DateOfRegistration = _class.RegisteredDate;
                DateOfModified = _class.LastModifiedDate;
                InsertedBy = _class.InsertedBy;



                IsLoading = false;


            });

            IsLoading = false;

        }
        private async void SelectTeacher(object parameter)
        {

            if (StringFormatValidator.IsValidTeacherID(TeacherID))
            {
                IsLoading = true;

                await Task.Run(() =>
                {
                    IsTeacherIDSelected = new TeacherManager().IsValidID(TeacherID);
                });

                if (IsTeacherIDSelected)
                {
                    SelectedTeacherID = TeacherID;
                }
                else
                {
                    var dialogBox = new InfoDialog(DisplayMessages.InvalidTeacherID);
                    dialogBox.ShowDialog();
                }

                IsLoading = false;

            }
            else
            {
                var dialogBox = new InfoDialog(DisplayMessages.InvalidTeacherID);
                dialogBox.ShowDialog();
            }
        }
        private void ClearTeacher(object parameter)
        {
            TeacherID = "";
            SelectedTeacherID = "";
            IsTeacherIDSelected = false;
        }
        private async void Remove(object parameter)
        {
            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmRemoveClass);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            IsLoading = true;
            bool success = false;

            await Task.Run(() =>
            {
                success = new ClassManager().DeleteClass(ClassID);

            });

            if (success)
            {
                _navigationVM.CurrentView = new ClassVM(_navigationVM);
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

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmUpdateClass);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            Class cls = new Class()
            {
                ClassID = ClassID,
                Name = Name,
                Fee = Fee,
                Margin = Margin,
                Day = Day,
                Time = Time,
                TeacherID = SelectedTeacherID

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new ClassManager().UpdateClass(cls);

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
            _navigationVM.CurrentView = new ClassVM(_navigationVM);

        }
        private bool IsEmptyFields()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Time) || string.IsNullOrEmpty(SelectedTeacherID) || string.IsNullOrEmpty(Fee.ToString()) || string.IsNullOrEmpty(Margin.ToString()) || string.IsNullOrEmpty(Day) || string.IsNullOrEmpty(DateOfRegistration.ToString()) || !IsTeacherIDSelected)
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
