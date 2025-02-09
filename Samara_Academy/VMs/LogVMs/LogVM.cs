using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;

using Samara_Academy.VMs.CommonVMs;

using System.Data;

using System.Windows.Input;

namespace Samara_Academy.VMs.LogVMs
{
    public class LogVM:VMBase
    {
        private readonly NavigationVM _navigationVM;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isLoading;
        private int _totalPages;
        private int _totalRecords;
        private int _currentPage = 1;
        private int _pageSize = 12;
        private List<string> _headings;
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

        private DataTable _logs;
        public DataTable Logs
        {
            get => _logs;
            set
            {
                _logs = value;
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

        public ICommand PageNavigationCommand { get; }

        public LogVM(NavigationVM navigationVM)
        {

            _navigationVM = navigationVM;

            PageNavigationCommand = new RelayCommand(PageNavigation);

            Logs = new DataTable();
            Headings = Resources.LogHeadings;
            TotalPages = 1;
            TotalRecords = 0;
            SearchBy = Headings[0];


            IsLoading = true;
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

                    Logs = new LogManager().Logs(_pageSize, offset, SearchText, Resources.LogHeadingKeyPairs[SearchBy]);

                    if (Logs == null)
                    {
                        token.ThrowIfCancellationRequested();

                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "LogVM");
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Logs.Rows.Count;
                    }
                    else
                    {
                        TotalRecords = new LogManager().LogsCount();
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
        

    }
}
