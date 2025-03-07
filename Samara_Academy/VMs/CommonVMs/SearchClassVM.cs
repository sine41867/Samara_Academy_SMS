﻿using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class SearchClassVM:VMBase
    {
        private CancellationTokenSource _cancellationTokenSource;

        private bool _isLoading;
        private bool _isLoadingFailed;
        private List<string> _headings;
        private string _searchText;
        private string _searchBy;
        private string _selectedClassID;

        public string SelectedClassID
        {
            get => _selectedClassID;
            set
            {
                _selectedClassID = value;
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
        public bool IsLoadingFailed
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }


        public ICommand CopyClassCommand { get; }
        public ICommand SearchClassCommand { get; }
        public ICommand CancelCommand { get; }

        public SearchClassVM()
        {


            CopyClassCommand = new RelayCommand(CopyClass);
            
            SearchClassCommand = new RelayCommand(SearchClass);
            CancelCommand = new RelayCommand(Cancel);

            Classes = new DataTable();
            Headings = Resources.ClassHeadings;

            SearchBy = Headings[0];

            IsLoadingFailed = false;
            IsLoading = false;
            

        }
        private void CopyClass(object parameter)
        {
            if (parameter != null)
            {
                SelectedClassID = ((DataRowView)parameter)["class_id"].ToString();
                Clipboard.SetText(SelectedClassID);

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
                    Classes = new ClassManager().Classes(SearchText, Resources.ClassHeadingKeyPairs[SearchBy]);
                    token.ThrowIfCancellationRequested();
                    if (Classes == null)
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
            SelectedClassID = "";
            Classes = new DataTable();
        }

    }
}