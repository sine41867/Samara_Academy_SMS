using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Samara_Academy.VMs.CommonVMs
{
    class SearchTeacherVM:VMBase
    {
        private CancellationTokenSource _cancellationTokenSource;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private List<string> _headings;
        private Dictionary<string, string> _headingKeyPairs;
        private string _searchText;
        private string _searchBy;
        private string _selectedTeacherID;

        public string SelectedTeacherID
        {
            get => _selectedTeacherID;
            set
            {
                _selectedTeacherID = value;
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

            }
        }

        public string SearchBy
        {
            get => _searchBy;
            set
            {
                _searchBy = value;
                OnPropertyChanged();

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
        public bool IsLoadingFailed
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }


        public ICommand CopyTeacherCommand { get; }
        public ICommand SearchTeacherCommand { get; }
        public ICommand CancelCommand { get; }

        public SearchTeacherVM()
        {
            CopyTeacherCommand = new RelayCommand(CopyClass);

            SearchTeacherCommand = new RelayCommand(SearchClass);
            CancelCommand = new RelayCommand(Cancel);

            Teachers = new DataTable();
            Headings = new List<string>();
            _headingKeyPairs = new Dictionary<string, string>();

            SearchBy = "";

            IsLoadingFailed = false;
            IsLoading = false;
            LoadHeadings();

        }
        private void CopyClass(object parameter)
        {
            if (parameter != null)
            {
                SelectedTeacherID = ((DataRowView)parameter)["teacher_id"].ToString();
                Clipboard.SetText(SelectedTeacherID);

            }


        }

        private async void SearchClass(object parameter)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;

            IsLoading = true;

            try
            {
                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    Teachers = new TeacherManager().Teachers(SearchText, _headingKeyPairs[SearchBy]);
                    token.ThrowIfCancellationRequested();
                    if (Teachers == null)
                    {
                        token.ThrowIfCancellationRequested();
                        IsLoadingFailed = true;
                    }
                }, token);

            }
            catch
            {

            }
            finally
            {
                IsLoading = false;

            }


        }
        private void Cancel(object parameter)
        {
            SelectedTeacherID = "";
            Teachers = new DataTable();
        }


        private void LoadHeadings()
        {
            _headingKeyPairs.Add("Teacher ID", "teacher_id");
            _headingKeyPairs.Add("Name", "name");
            _headingKeyPairs.Add("Mobile", "mobile");
            _headingKeyPairs.Add("WhatssApp", "whatsapp");
            _headingKeyPairs.Add("Registered Date", "registered_date");

            Headings = _headingKeyPairs.Keys.ToList();

            SearchBy = Headings[0];
        }

    }
}