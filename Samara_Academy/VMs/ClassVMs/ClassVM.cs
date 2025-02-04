using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.VMs.CommonVMs;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Samara_Academy.VMs.ClassVMs
{
    public class ClassVM : VMBase
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

        private DataTable _classes;
        public DataTable Classes
        {
            get => _classes;
            set
            {
                _classes = value;
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

        public ICommand AddClassCommand { get; }
        public ICommand DetailsClassCommand { get; }

        public ICommand PageNavigationCommand { get; }

        public ClassVM(NavigationVM navigationVM)
        {

            _navigationVM = navigationVM;

            AddClassCommand = new RelayCommand(AddClass);
            PageNavigationCommand = new RelayCommand(PageNavigation);
            DetailsClassCommand = new RelayCommand(DetailsClass);

            Classes = new DataTable();
            Headings = new List<string>();
            _headingKeyPairs = new Dictionary<string, string>();
            TotalPages = 1;
            TotalRecords = 0;
            SearchBy = "";


            IsLoading = true;
            LoadHeadings();
            LoadData();


        }
        private void DetailsClass(object parameter)
        {
            if (parameter != null)
            {
                string classID = ((DataRowView)parameter)["class_id"].ToString();
                _navigationVM.CurrentView = new DetailsClassVM(_navigationVM, classID);
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

            int offset = (_currentPage - 1) * _pageSize;


            try
            {
                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    Classes = new ClassManager().Classes(_pageSize, offset, SearchText, _headingKeyPairs[SearchBy]);

                    if (Classes == null)
                    {
                        token.ThrowIfCancellationRequested();

                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "ClassVM");
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Classes.Rows.Count;
                    }
                    else
                    {
                        TotalRecords = new ClassManager().ClassesCount();
                    }

                    if (TotalRecords == -1)
                    {
                        token.ThrowIfCancellationRequested();

                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "ClassVM");
                    }

                    TotalPages = (int)Math.Ceiling((double)TotalRecords / _pageSize);
                    if (TotalPages == 0)
                    {
                        TotalPages = 1;
                    }
                },token);
            }
            catch { }

            finally
            {
                IsLoading = false;
            }


        }
        private void AddClass(object parameter)
        {
            _cancellationTokenSource?.Cancel();

            _navigationVM.CurrentView = new AddClassVM(_navigationVM);
        }


        private void LoadHeadings()
        {
            _headingKeyPairs.Add("Class ID", "class_id");
            _headingKeyPairs.Add("Name", "name");
            _headingKeyPairs.Add("Fee", "fee");
            _headingKeyPairs.Add("Time", "time");
            _headingKeyPairs.Add("Day", "day");
            _headingKeyPairs.Add("Registered Date", "registered_date");

            Headings = _headingKeyPairs.Keys.ToList();

            SearchBy = Headings[0];
        }

    }
}