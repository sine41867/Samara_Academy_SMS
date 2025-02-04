using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.VMs.CommonVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Samara_Academy.VMs.StudentVMs
{
    public class StudentVM : VMBase
    {
        private readonly NavigationVM _navigationVM;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isLoading;
        private int _totalPages;
        private int _totalRecords;
        private int _currentPage = 1;
        private int _pageSize = 12;
        private List<string> _headings;
        private Dictionary<string, string> _headingKeyPairs;
        private string _searchText;
        private string _searchBy;

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

        public ICommand AddStudentCommand { get; }
        public ICommand DetailsStudentCommand { get; }
        public ICommand EnrollCommand { get; }
        public ICommand PageNavigationCommand { get; }

        public StudentVM(NavigationVM navigationVM)
        {

            _navigationVM = navigationVM;

            AddStudentCommand = new RelayCommand(AddStudent);
            DetailsStudentCommand = new RelayCommand(DetailsStudent);
            EnrollCommand = new RelayCommand(Enroll);
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
                    Students = new StudentManager().Students(_pageSize, offset, SearchText, _headingKeyPairs[SearchBy]);
                    token.ThrowIfCancellationRequested();

                    if (Students == null)
                    {
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "StudentVM");
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Students.Rows.Count;
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();
                        TotalRecords = new StudentManager().StudentsCount();
                    }

                    if (TotalRecords == -1)
                    {
                        token.ThrowIfCancellationRequested();
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "StudentVM");
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
        private void AddStudent(object parameter)
        {
            _cancellationTokenSource?.Cancel();
            _navigationVM.CurrentView = new AddStudentVM(_navigationVM);
        }

        private void DetailsStudent(object parameter)
        {
            if (parameter != null)
            {
                string studentID = ((DataRowView)parameter)["student_id"].ToString();
                _navigationVM.CurrentView = new DetailsStudentVM(_navigationVM, studentID);
            }
        }

        private void Enroll(object parameter)
        {
            if (parameter != null)
            {
                string studentID = ((DataRowView)parameter)["student_id"].ToString();
                EnrollStudentsView window = new EnrollStudentsView(studentID);
                window.ShowDialog();

            }
        }

        private void LoadHeadings()
        {

            _headingKeyPairs.Add("Student ID", "student_id");
            _headingKeyPairs.Add("Name", "name");
            _headingKeyPairs.Add("Mobile", "mobile");
            _headingKeyPairs.Add("WhatsApp", "whatsapp");
            _headingKeyPairs.Add("Registered Date", "registered_date");

            Headings = _headingKeyPairs.Keys.ToList();

            SearchBy = Headings[0];
        }

    }
}
