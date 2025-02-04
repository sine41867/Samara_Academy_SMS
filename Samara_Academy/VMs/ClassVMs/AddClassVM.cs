using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.Views.CommonViews;

namespace Samara_Academy.VMs.ClassVMs
{
    public class AddClassVM : VMBase
    {
        private readonly NavigationVM _navigationVM;

        private List<string> _days;

        private bool _isLoading;
        private bool _isTeacherIDSelected;

        private string _name;
        private string _time;
        private string _teacherId;
        private string _selectedTeacherId;
        private string _day;
        private float _fee;
        private float _margin;

        private DateTime _dateOfRegistration;


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
        public DateTime DateOfRegistration
        {
            get => _dateOfRegistration;
            set
            {
                _dateOfRegistration = value;
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

        public ICommand SearchTeachersCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand SelectTeacherIDCommand { get; }

        public ICommand ClearTeacherIDCommand { get; }
        public ICommand GoBackCommand { get; }
        public AddClassVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
            SearchTeachersCommand = new RelayCommand(SearchTeacher);
            AddCommand = new RelayCommand(Add);
            ClearCommand = new RelayCommand(Clear);
            SelectTeacherIDCommand = new RelayCommand(SelectTeacher);
            ClearTeacherIDCommand = new RelayCommand(ClearTeacher);
            GoBackCommand = new RelayCommand(GoBack);

            Days = new List<string>()
            {
               "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"

            };

            DateOfRegistration = DateTime.Now;
            TeacherID = "";
            SelectedTeacherID = "";
            IsTeacherIDSelected = false;
            IsLoading = false;
            Day = Days[0];


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

        private async void Add(object parameter)
        {
            if (IsEmptyFields())
            {
                var dialogBox = new InfoDialog(DisplayMessages.FillAllFields);
                dialogBox.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmAddClass);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            Class cls = new Class()
            {
                Name = Name,
                Day = Day,
                Fee = Fee,
                Margin = Margin,
                Time = Time,
                TeacherID = SelectedTeacherID,
                RegisteredDate = DateOfRegistration.ToString("yyyy-MM-dd"),
                InsertedBy = SharedResources.UserID

            };
            IsLoading = true;
            bool success = false;
            await Task.Run(() =>
            {
                success = new ClassManager().AddClass(cls);

            });

            if (success)
            {
                Clear(null);
            }

            IsLoading = false;

        }
        private void SearchTeacher(object parameter)
        {
            SearchTeacherView window = new SearchTeacherView();
            window.ShowDialog();
        }

        private void Clear(object parameter)
        {
            Name = "";
            Time = "";
            Fee = 0;
            Margin = 0;
            DateOfRegistration = DateTime.Now;
            TeacherID = "";
            SelectedTeacherID = "";
            IsTeacherIDSelected = false;
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

