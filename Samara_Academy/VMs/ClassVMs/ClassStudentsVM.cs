using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.ClassVMs
{
    public class ClassStudentsVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private CancellationTokenSource _cancellationTokenSource;

        private string _classId;
        private bool _isLoading;
        private int _totalPages;
        private int _totalRecords;
        private int _currentPage = 1;
        private int _pageSize = 12;
        private List<string> _headings;
        private Dictionary<string, string> _headingKeyPairs;
        private string _searchText;
        private string _searchBy;

        public string ClassID
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadData();
            }
        }

        public string SearchBy
        {
            get => _searchBy;
            set
            {
                _searchBy = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(SearchText))
                {
                    LoadData();
                }

            }
        }

        private DataTable _students;
        public DataTable Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged();
            }
        }

        public List<string> Headings
        {
            get => _headings;
            set
            {
                _headings = value;
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

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();

                LoadData();
            }
        }

        public int TotalRecords
        {
            get => _totalRecords;
            set
            {
                _totalRecords = value;
                OnPropertyChanged();

            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand RemoveEnrollmentCommand { get; }
        public ICommand PageNavigationCommand { get; }

        public ClassStudentsVM(NavigationVM navigationVM, string classId)
        {

            _navigationVM = navigationVM;
            ClassID = classId;

            GoBackCommand = new RelayCommand(GoBack);
            RemoveEnrollmentCommand = new RelayCommand(RemoveEnrollment);
            PageNavigationCommand = new RelayCommand(PageNavigation);

            
            Students = new DataTable();
            Headings = new List<string>();
            _headingKeyPairs = new Dictionary<string, string>();
            
            TotalPages = 1;
            TotalRecords = 0;
            SearchBy = "";


            IsLoading = true;
            LoadHeadings();
            LoadData();
            

        }
        private void PageNavigation(object parameter)
        {
            string direction = parameter.ToString();

            if (direction == "Next" && CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
            else if (direction == "Previous" && CurrentPage > 1)
            {
                CurrentPage--;
            }
            else if (direction == "First")
            {
                CurrentPage = 1;
            }
            else if (direction == "Last")
            {
                CurrentPage = TotalPages;
            }
            else if (int.TryParse(direction, out int page) && page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }
        private async void LoadData()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            
            IsLoading = true;

            
            try
            {
                int offset = (_currentPage - 1) * _pageSize;

                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    Students = new StudentManager().StudentsByClassID(_pageSize, offset, ClassID, SearchText, _headingKeyPairs[SearchBy]);
                    token.ThrowIfCancellationRequested();

                    if (Students == null)
                    {
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "ClassStudentVM", ClassID);
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Students.Rows.Count;
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();
                        TotalRecords = new StudentManager().StudentsCountByClassID(ClassID);
                    }

                    if (TotalRecords == -1)
                    {
                        token.ThrowIfCancellationRequested();
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "ClassStudentVM", ClassID);
                    }

                    TotalPages = (int)Math.Ceiling((double)TotalRecords / _pageSize);
                    if (TotalPages == 0)
                    {
                        TotalPages = 1;
                    }
                }, token);
            }
            catch { }

            finally
            {
                IsLoading = false;
            }
            
        }
        private async void RemoveEnrollment(object parameter)
        {

            if (parameter != null)
            {
                string studentID = ((DataRowView)parameter)["student_id"].ToString();

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
                    success = new EnrollmentManager().DeleteEnrollment(studentID, ClassID);

                });

                if (success)
                {
                    LoadData();
                }

            }
        }
        private void GoBack(object parameter)
        {
            _cancellationTokenSource?.Cancel();

            _navigationVM.CurrentView = new DetailsClassVM(_navigationVM, ClassID);
        }

        private void LoadHeadings()
        {

            _headingKeyPairs.Add("Student ID", "tbl_student.student_id");
            _headingKeyPairs.Add("Name", "name");
            _headingKeyPairs.Add("Mobile", "mobile");
            _headingKeyPairs.Add("WhatsApp", "whatsapp");
            _headingKeyPairs.Add("Enrolled Date", "registered_date");
            _headingKeyPairs.Add("Enrolled By", "tbl_enrollment.user_id");

            Headings = _headingKeyPairs.Keys.ToList();

            SearchBy = Headings[0];
        }
    }
}
