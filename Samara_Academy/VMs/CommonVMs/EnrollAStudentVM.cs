using Samara_Academy.DatabaseManagers;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.CommonViews;
using Samara_Academy.Views.DialogBoxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class EnrollAStudentVM : VMBase
    {
        private CancellationTokenSource _cancellationTokenSource;

        private ObservableCollection<Class> _enrolledClasses;
        private bool _isLoading;
        private string _studentID;
        private string _classID;

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

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Class> EnrolledClasses
        {
            get => _enrolledClasses;
            set
            {
                _enrolledClasses = value;
                OnPropertyChanged();
            }
        }

        public string StudentID
        {
            get => _studentID;
            set
            {
                _studentID = value;
                OnPropertyChanged();
            }
        }
        public string ClassID
        {
            get => _classID;
            set
            {
                _classID = value;
                OnPropertyChanged();
            }
        }

        public ICommand EnrollCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SearchClassCommand { get; }
        public ICommand AddtoEnrollCommand { get; }
        public ICommand RemoveFromEnrollCommand { get; }
        public EnrollAStudentVM(string studentID = null)
        {
            EnrollCommand = new RelayCommand(Enroll);
            ClearCommand = new RelayCommand(Clear);
            AddtoEnrollCommand = new RelayCommand(AddtoEnroll);
            RemoveFromEnrollCommand = new RelayCommand(RemoveFromEnroll);
            SearchClassCommand = new RelayCommand(SearchClass);

            StudentID = studentID;
            EnrolledClasses = new ObservableCollection<Class>();
            ClassID = "";

        }
        private async void AddtoEnroll(object parameter)
        {
            if (StringFormatValidator.IsValidClassID(ClassID))
            {
                IsLoading = true;

                Class clz = new Class();

                bool isReturned = false;

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                var token = _cancellationTokenSource.Token;

                try
                {
                    await Task.Run(() =>
                    {
                        foreach (Class cls in EnrolledClasses)
                        {
                            if (cls.ClassID.Equals(ClassID))
                            {
                                IsLoading = false;

                                token.ThrowIfCancellationRequested();


                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dialogBox = new InfoDialog(DisplayMessages.SameClassID);
                                    dialogBox.ShowDialog();

                                });

                                isReturned = true;
                                return;
                            }
                        }

                        if (isReturned)
                        {
                            return;
                        }
                        token.ThrowIfCancellationRequested();


                        clz = new ClassManager().GetClassByID(ClassID);
                        clz.InsertedBy = SharedResources.UserID;

                        if (isReturned)
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(clz.Name))
                        {
                            token.ThrowIfCancellationRequested();


                            IsLoading = false;
                            var dialogBox = new InfoDialog(DisplayMessages.InvalidClassID);
                            dialogBox.ShowDialog();

                            return;
                        }

                        

                    }, token);
                }
                catch { }
                finally
                {
                    IsLoading = false;
                }
                if (!string.IsNullOrEmpty(clz.Name))
                {
                    EnrolledClasses.Add(clz); 
                }
                    
                ClassID = "";


            }
            else
            {
                var dialogBox = new InfoDialog(DisplayMessages.InvalidClassID);
                dialogBox.ShowDialog();
            }
        }
        private void Clear(object parameter)
        {
            _cancellationTokenSource?.Cancel();

            EnrolledClasses.Clear();
            ClassID = "";
        }
        private void RemoveFromEnroll(object parameter)
        {
            if (parameter is Class cls)
            {
                EnrolledClasses.Remove(cls);
            }
        }
        private void SearchClass(object parameter)
        {
            SeacrchClassView window = new SeacrchClassView();
            window.ShowDialog();
        }
        private async void Enroll(object parameter)
        {
            if (EnrolledClasses.Count == 0)
            {
                var dialogBox = new InfoDialog(DisplayMessages.ClassesNotSelected);
                dialogBox.ShowDialog();
                return;
            }

            IsLoading = true;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;

            bool success = false;

            try
            {
                await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    success = new EnrollmentManager().AddEnrollment(EnrolledClasses, DateTime.Now.ToString("yyyy-MM-dd"), StudentID, SharedResources.UserID);

                }, token);

                if (success)
                {
                    token.ThrowIfCancellationRequested();

                    Clear(null);
                }
            }
            catch
            {

            }
            finally
            {
                IsLoading = false;

            }

        }

    }
}
