using LiveCharts;
using LiveCharts.Wpf;
using Samara_Academy.DatabaseManagers;
using Samara_Academy.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class DashboardVM : VMBase
    {
        private readonly NavigationVM _navigationVM;

        private SeriesCollection _sereiesCollection;
        private string[] _labels;
        private Func<double, string> _formatter;

        private string _userName;

        private int _teacherCount;
        private int _studentCount;
        private int _classCount;
        private int _enrollmentCount;

        public int TeacherCount
        {
            get => _teacherCount;
            set
            {
                _teacherCount = value;
                OnPropertyChanged();
            }
        }
        public int StudentCount
        {
            get => _studentCount;
            set
            {
                _studentCount = value;
                OnPropertyChanged();
            }
        }
        public int ClassCount
        {
            get => _classCount;
            set
            {
                _classCount = value;
                OnPropertyChanged();
            }
        }
        public int EnrollmentCount
        {
            get => _enrollmentCount;
            set
            {
                _enrollmentCount = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        public SeriesCollection SeriesCollection
        {
            get => _sereiesCollection;
            set
            {
                _sereiesCollection = value;
                OnPropertyChanged();
            }
        }

        public string[] Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter
        {
            get => _formatter;
            set
            {
                _formatter = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateCommand { get; }
        public DashboardVM(NavigationVM navigationVM)
        {
            _navigationVM = navigationVM;

            UpdateCommand = new RelayCommand(Update);
            Formatter = value => value.ToString("N");
            UserName = SharedResources.LoggedUser.Name;
            SeriesCollection = new SeriesCollection();

            Labels = SharedResources.Months;

            LoadChart();
            LoadCounts();
            
        }

        private async void LoadCounts()
        {
            await Task.Run(() => 
            {
                _teacherCount = new TeacherManager().TeachersCount();
                if (_teacherCount != 0)
                {
                    TeacherCount = _teacherCount;
                }
            });
            await Task.Run(() =>
            {
                StudentCount = new StudentManager().StudentsCount();
                if (_studentCount != 0)
                {
                    StudentCount = _studentCount;
                }
            });
            await Task.Run(() =>
            {
                ClassCount = new ClassManager().ClassesCount();
                if (_classCount != 0)
                {
                    ClassCount = _classCount;
                }
            });
            await Task.Run(() =>
            {
                EnrollmentCount = new EnrollmentManager().EnrollmentsCount();
                if (_enrollmentCount != 0)
                {
                    EnrollmentCount = _enrollmentCount;
                }
            });
        }

        private async void LoadChart()
        {
            Dictionary<string, int> StudentMonthlyCountsDict = new Resources().MonthlyCount;
            Dictionary<string, int> TeacherMonthlyCountsDict = new Resources().MonthlyCount;
            Dictionary<string, int> ClassMonthlyCountsDict = new Resources().MonthlyCount;
            Dictionary<string, int> EnrollmentMonthlyCountsDict = new Resources().MonthlyCount;


            await Task.Run(() => 
            {
                DataTable teacherData = new TeacherManager().TeachersCountMonthly();

                if(teacherData != null)
                {
                    foreach (DataRow row in teacherData.Rows)
                    {
                        string month = row["month"].ToString();

                        TeacherMonthlyCountsDict[month] = Convert.ToInt32(row["teacher_count"]);
                    }

                }
                
            });

            await Task.Run(() =>
            {
                DataTable studentData = new StudentManager().StudentsCountMonthly();

                if (studentData != null)
                {
                    foreach (DataRow row in studentData.Rows)
                    {
                        string month = row["month"].ToString();

                        StudentMonthlyCountsDict[month] = Convert.ToInt32(row["student_count"]);
                    }

                }

            });

            await Task.Run(() =>
            {
                DataTable classData = new ClassManager().ClassesCountMonthly();

                if (classData != null)
                {
                    foreach (DataRow row in classData.Rows)
                    {
                        string month = row["month"].ToString();

                        ClassMonthlyCountsDict[month] = Convert.ToInt32(row["class_count"]);
                    }

                }

            });

            await Task.Run(() =>
            {
                DataTable classData = new EnrollmentManager().EnrollmentsCountMonthly();

                if (classData != null)
                {
                    foreach (DataRow row in classData.Rows)
                    {
                        string month = row["month"].ToString();

                        EnrollmentMonthlyCountsDict[month] = Convert.ToInt32(row["enrollment_count"]);
                    }

                }

            });

            ChartValues<int> teacherMonthlyCount = new ChartValues<int>(TeacherMonthlyCountsDict.Values);
            ChartValues<int> studentMonthlyCount = new ChartValues<int>(StudentMonthlyCountsDict.Values);
            ChartValues<int> classMonthlyCount = new ChartValues<int>(ClassMonthlyCountsDict.Values);
            ChartValues<int> enrollmentMonthlyCount = new ChartValues<int>(EnrollmentMonthlyCountsDict.Values);;


            ColumnSeries columnSereiesTeacher = new ColumnSeries();
            ColumnSeries columnSereiesStudent = new ColumnSeries();
            ColumnSeries columnSereiesClass = new ColumnSeries();
            ColumnSeries columnSereiesEnrollment = new ColumnSeries();

            columnSereiesTeacher.Title = "Teachers";
            columnSereiesTeacher.Values = teacherMonthlyCount;

            columnSereiesStudent.Title = "Students";
            columnSereiesStudent.Values = studentMonthlyCount;

            columnSereiesClass.Title = "Classes";
            columnSereiesClass.Values = classMonthlyCount;

            columnSereiesEnrollment.Title = "Enrollments";
            columnSereiesEnrollment.Values = enrollmentMonthlyCount;

            SeriesCollection.Add(columnSereiesTeacher);
            SeriesCollection.Add(columnSereiesStudent);
            SeriesCollection.Add(columnSereiesClass);
            SeriesCollection.Add(columnSereiesEnrollment);

        }
        private void Update(object parameter)
        {
            ////SeriesCollection[1].Values.Add(48d);

        }
    }
}
