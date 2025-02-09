using Microsoft.VisualBasic;
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

namespace Samara_Academy.VMs.UserVMs
{
    public class UserVM:VMBase
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

        private DataTable _users;
        public DataTable Users
        {
            get => _users;
            set
            {
                _users = value;
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

        public ICommand AddUserCommand { get; }
        public ICommand ChangeUserCommand { get; }
        public ICommand RemoveUserCommand { get; }
        public ICommand PageNavigationCommand { get; }

        public UserVM(NavigationVM navigationVM)
        {

            _navigationVM = navigationVM;

            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser);
            ChangeUserCommand = new RelayCommand(ChangeUser);
            PageNavigationCommand = new RelayCommand(PageNavigation);

            Users = new DataTable();
            Headings = Resources.UserHeadings;
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
                    Users = new UserManager().Users(_pageSize, offset, SearchText, Resources.UserHeadingKeyPairs[SearchBy]);
                    token.ThrowIfCancellationRequested();

                    if (Users == null)
                    {
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "UserVM");
                        return;
                    }

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        TotalRecords = Users.Rows.Count;
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();
                        TotalRecords = new UserManager().UsersCount();
                    }

                    if (TotalRecords == -1)
                    {
                        token.ThrowIfCancellationRequested();
                        _navigationVM.CurrentView = new BlankVM(_navigationVM, "UserVM");
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

        private void AddUser(object parameter)
        {
            _cancellationTokenSource?.Cancel();
            _navigationVM.CurrentView = new AddUserVM(_navigationVM);
        }
        private async void RemoveUser(object parameter)
        {
            if (parameter != null)
            {
                string userId = ((DataRowView)parameter)["user_id"].ToString();

                if(userId == SharedResources.UserID)
                {
                    Window dialog2 = new InfoDialog(DisplayMessages.OwnAccountDeleteError);
                    dialog2.ShowDialog();
                    return;
                }

                Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmRemoveUser);
                dialog.ShowDialog();

                if (dialog.DialogResult == false)
                {
                    return;
                }

                IsLoading = true;
                bool success = false;

                await Task.Run(() =>
                {
                    success = new UserManager().DeleteUser(userId);

                });

                if (success)
                {
                    LoadData();
                }

                IsLoading = false;
            }

        }
        private async void DisableUser(string userId)
        {
            if (userId == SharedResources.UserID)
            {
                Window dialog2 = new InfoDialog(DisplayMessages.OwnAccountDisableError);
                dialog2.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmDisableUser);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            IsLoading = true;
            bool success = false;

            await Task.Run(() =>
            {
                success = new UserManager().DisableUser(userId);

            });

            if (success)
            {
                LoadData();
            }

            IsLoading = false;

        }
        private async void EnableUser(string userId)
        {
            if (userId == SharedResources.UserID)
            {
                Window dialog2 = new InfoDialog(DisplayMessages.OwnAccountEnableError);
                dialog2.ShowDialog();
                return;
            }

            Window dialog = new ConfirmationDialog(DisplayMessages.ConfirmEnableUser);
            dialog.ShowDialog();

            if (dialog.DialogResult == false)
            {
                return;
            }

            IsLoading = true;
            bool success = false;

            await Task.Run(() =>
            {
                success = new UserManager().EnableUser(userId);

            });

            if (success)
            {
                LoadData();
            }

            IsLoading = false;

        }

        private async void ChangeUser(object parameter)
        {
            if (parameter != null)
            {
                string userId = ((DataRowView)parameter)["user_id"].ToString();
                bool isDisabled = (bool)((DataRowView)parameter)["is_disable"];

                if (isDisabled)
                {
                    EnableUser(userId);
                }
                else
                {
                    DisableUser(userId);
                }

                
            }

        }
       
    }
}
