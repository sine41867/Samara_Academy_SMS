using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Samara_Academy.VMs.TeacherVMs
{
    public class TeacherVM : VMBase
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

        private DataTable _teachers;
        public DataTable Teachers
        {
            get => _teachers;
            set
            {
                _teachers = value;
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

        public ICommand AddTeacherCommand { get; }
        public ICommand DetailsTeacherCommand { get; }

        public ICommand PageNavigationCommand { get; }

        public TeacherVM(NavigationVM navigationVM)
        {

            _navigationVM = navigationVM;

            AddTeacherCommand = new RelayCommand(AddTeacher);
            PageNavigationCommand = new RelayCommand(PageNavigation);
            DetailsTeacherCommand = new RelayCommand(DetailsTeacher);

            Teachers = new DataTable();
            Headings = new List<string>();
            _headingKeyPairs = new Dictionary<string, string>();
            TotalPages = 1;
            TotalRecords = 0;
            SearchBy = "";

            IsLoading = true;
            LoadHeadings();
            LoadData();

        }

        private void DetailsTeacher(object parameter)
        {
            if (parameter != null)
            {
                string teacherID = ((DataRowView)parameter)["teacher_id"].ToString();
                _navigationVM.CurrentView = new DetailsTeacherVM(_navigationVM, teacherID);
            }

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

            IsLoading = true;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;


            try
            {
                int offset = (_currentPage - 1) * _pageSize;

                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    Teachers = new TeacherManager().Teachers(_pageSize, offset, SearchText, _headingKeyPairs[SearchBy]);

                    if (Teachers == null)
                    {
                        token.ThrowIfCancellationRequested();

                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "TeacherVM");
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Teachers.Rows.Count;
                    }
                    else
                    {
                        TotalRecords = new TeacherManager().TeachersCount();
                    }

                    if (TotalRecords == -1)
                    {
                        token.ThrowIfCancellationRequested();

                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "TeacherVM");
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
        private void AddTeacher(object parameter)
        {
            _cancellationTokenSource?.Cancel();

            _navigationVM.CurrentView = new AddTeacherVM(_navigationVM);
        }


        private void LoadHeadings()
        {
            _headingKeyPairs.Add("Teacher ID", "teacher_id");
            _headingKeyPairs.Add("Name", "name");
            _headingKeyPairs.Add("Mobile", "mobile");
            _headingKeyPairs.Add("WhatsApp", "whatsapp");
            _headingKeyPairs.Add("Registered Date", "registered_date");

            Headings = _headingKeyPairs.Keys.ToList();

            SearchBy = Headings[0];
        }

    }
}
