using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Samara_Academy.DatabaseManagers
{
    public class EnrollmentManager : DataBaseHelper
    {
        public EnrollmentManager()
        {
        }

        public DataTable EnrollmentsCountMonthly()
        {
            string year = DateTime.Now.Year.ToString();

            string query = $"SELECT DATE_FORMAT(enrolled_date, '%m') AS month, COUNT(*) AS enrollment_count FROM tbl_enrollment WHERE DATE_FORMAT(enrolled_date, '%Y') = '{year}' GROUP BY DATE_FORMAT(enrolled_date, '%Y-%m') ORDER BY DATE_FORMAT(enrolled_date, '%Y-%m');";

            DataTable count = this.ExecuteQuery(query);

            return count;
        }

        public int EnrollmentsCount()
        {

            string query = "Select Count(*) from tbl_enrollment";

            object result = this.ExecuteScalar(query);

            return Convert.ToInt32(result);


        }
        public bool AddEnrollment(ObservableCollection<Class> classes, string enrolledDate, string studentId, string insertedBy)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    try
                    {
                        using (MySqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                bool IsEnrolled = false;
                                string query = "INSERT INTO `tbl_enrollment` (`student_id`, `class_id`, `enrolled_date`, `user_id`) VALUES(@student_id, @class_id, @enrolled_date, @inserted_by)";
                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";

                                MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Enroll"),
                                        new MySqlParameter("@user_id", SharedResources.UserID)


                                    };

                                foreach (Class cls in classes)
                                {
                                    using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@student_id", studentId);
                                        cmd.Parameters.AddWithValue("@class_id", cls.ClassID);
                                        cmd.Parameters.AddWithValue("@enrolled_date", enrolledDate);
                                        cmd.Parameters.AddWithValue("@inserted_by", insertedBy);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            IsEnrolled = true;




                                        }
                                        catch (MySqlException ex)
                                        {
                                            if (ex.Number == 1062)
                                            {
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    var dialogBox = new InfoDialog($"Already enrolled to {cls.ClassID}.");
                                                    dialogBox.ShowDialog();

                                                });

                                                continue;

                                            }
                                            else
                                            {
                                                throw ex;
                                            }
                                        }


                                    }

                                    using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                    {

                                        if (logParameters != null)
                                        {
                                            command.Parameters.AddRange(logParameters);
                                            command.Parameters.Add(new MySqlParameter("@description", $"{studentId}, {cls.ClassID}"));
                                        }
                                        int logRows = command.ExecuteNonQuery();

                                        if (!(logRows > 0))
                                        {
                                            throw new Exception("Logging Error");
                                        }


                                    }
                                }

                                transaction.Commit();

                                if (IsEnrolled)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        var dialogBox = new InfoDialog(DisplayMessages.EnrollmentSuccess);
                                        dialogBox.ShowDialog();

                                    });
                                }



                                return true;

                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    var dialogBox = new InfoDialog($"An Error Occured...{ex}");
                                    dialogBox.ShowDialog();

                                });


                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {

                            var dialogBox = new InfoDialog($"An Error Occured...{ex}");
                            dialogBox.ShowDialog();

                        });

                        return false;

                    }

                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var dialogBox = new InfoDialog(DisplayMessages.ConnectionFailure);
                        dialogBox.ShowDialog();

                    });

                    return false;
                }
            }
        }

        public bool DeleteEnrollment(string studentID, string classID)
        {
            string query = "DELETE FROM tbl_enrollment WHERE `tbl_enrollment`.`student_id` = @student_id AND `tbl_enrollment`.`class_id` = @class_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@student_id", studentID),
                new MySqlParameter("@class_id", classID)
            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Leave"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", $"{studentID}, {classID}")

            };

            int rowsAffected = 0;

            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.StudentLeaveFromClassSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public ObservableCollection<Class> GetEnrollmentsByStudentID(string studentId)
        {
            DataTable results = new DataTable();
            string query = "Select tbl_enrollment.class_id as id, tbl_class.name as name from tbl_enrollment left join tbl_class on tbl_class.class_id = tbl_enrollment.class_id where tbl_enrollment.student_id = @studentId";
            
            MySqlParameter[] parameters = { new MySqlParameter("@studentId", studentId) };

            results = this.ExecuteQuery(query, parameters);

            if(results!=null)
            {
                ObservableCollection<Class> classes = new ObservableCollection<Class>();

                int i = 0;
                while(i < results.Rows.Count)
                {
                    Class cls = new Class()
                    {
                        ClassID = results.Rows[i]["id"].ToString(),
                        Name = results.Rows[i]["name"].ToString()
                    };

                    classes.Add(cls);

                    i++;
                    
                }

                return classes;
            }

            return null;


        }
    }
}
