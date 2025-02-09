using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Samara_Academy.DatabaseManagers
{
    public class StudentManager : DataBaseHelper
    {
        public StudentManager()
        {

        }
        public bool AddStudent(Student student, ObservableCollection<Class> Classes = null)
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
                                string studentId = "";
                                string query1 = "INSERT INTO `tbl_student` ( `name`, `mobile`, `whatsapp`, `registered_date`, `user_id`, `last_modified_date`) VALUES (@name, @mobile, @whatsapp, @registered_date, @user_id, @last_modified_date)";
                                string query2 = "INSERT INTO `tbl_enrollment` (`student_id`, `class_id`, `enrolled_date`, `user_id`) VALUES(@student_id, @class_id, @enrolled_date, @inserted_by)";
                                string query3 = "SELECT student_id from tbl_student ORDER BY student_id DESC";

                                using (MySqlCommand cmd = new MySqlCommand(query1, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@name", student.Name);
                                    cmd.Parameters.AddWithValue("@mobile", student.Mobile);
                                    cmd.Parameters.AddWithValue("@whatsapp", student.WhatsApp);
                                    cmd.Parameters.AddWithValue("@registered_date", student.RegisteredDate);
                                    cmd.Parameters.AddWithValue("@user_id", student.InsertedBy);
                                    cmd.Parameters.AddWithValue("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"));

                                    cmd.ExecuteNonQuery();

                                }

                                using (MySqlCommand cmd = new MySqlCommand(query3, connection, transaction))
                                {

                                    studentId = cmd.ExecuteScalar().ToString();

                                }

                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";
                                
                                using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                {
                                    MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Insert"),
                                        new MySqlParameter("@user_id", SharedResources.UserID),
                                        new MySqlParameter("@description", studentId)

                                    };

                                    if (logParameters != null)
                                    {
                                        command.Parameters.AddRange(logParameters);
                                    }
                                    int logRows = command.ExecuteNonQuery();

                                    if (!(logRows > 0))
                                    {
                                        throw new Exception("Logging Error");
                                    }

                                }

                                if (Classes != null)
                                {
                                    MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Enroll"),
                                        new MySqlParameter("@user_id", SharedResources.UserID)
                                        

                                    };

                                    foreach (Class cls in Classes)
                                    {
                                        using (MySqlCommand cmd = new MySqlCommand(query2, connection, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@student_id", studentId);
                                            cmd.Parameters.AddWithValue("@class_id", cls.ClassID);
                                            cmd.Parameters.AddWithValue("@enrolled_date", student.RegisteredDate);
                                            cmd.Parameters.AddWithValue("@inserted_by", cls.InsertedBy);

                                            cmd.ExecuteNonQuery();

                                        }

                                        using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                        {

                                            if (logParameters != null)
                                            {
                                                command.Parameters.AddRange(logParameters);
                                                command.Parameters.Add(new MySqlParameter("@description", $"Student ID : {studentId}, Class ID : {cls.ClassID}"));
                                            }
                                            int logRows = command.ExecuteNonQuery();

                                            if (!(logRows > 0))
                                            {
                                                throw new Exception("Logging Error");
                                            }


                                        }

                                    }
                                }

                                transaction.Commit();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dialogBox = new InfoDialog(DisplayMessages.StudentAddedSuccess);
                                    dialogBox.ShowDialog();

                                });


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
        public bool UpdateStudent(Student student)
        {
            string query = "UPDATE `tbl_student` SET `name` = @name, `mobile` = @mobile, `whatsapp` = @whatsapp, `last_modified_date` = @last_modified_date WHERE `tbl_student`.`student_id` = @student_id";
            
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", student.Name),
                new MySqlParameter("@mobile", student.Mobile),
                new MySqlParameter("@whatsapp", student.WhatsApp),
                new MySqlParameter("@student_id", student.StudentID),
                new MySqlParameter("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"))

            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Update"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", student.StudentID)

            };
            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.StudentUpdatedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public int StudentsCount()
        {
            string query = "Select Count(*) from tbl_student";
            object result = this.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public Student GetStudentByID(string studentId)
        {
            DataTable result = new DataTable();
            string query = "SELECT * FROM tbl_student WHERE student_id = @studentId";

            MySqlParameter[] parameters = { new MySqlParameter("@studentId", studentId) };

            result = this.ExecuteQuery(query, parameters);

            if(result == null)
            {
                return null;
            }

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];

                return new Student()
                {
                    Name = row["name"].ToString(),
                    Mobile = row["mobile"].ToString(),
                    WhatsApp = row["whatsapp"].ToString(),
                    RegisteredDate = row["registered_date"].ToString(),
                    LastModifiedDate = row["last_modified_date"].ToString(),
                    InsertedBy = row["user_id"].ToString()
                };
            }
            else
            {
                return null;
            }

        }

        public DataTable Students(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_student LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_student WHERE {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable students = this.ExecuteQuery(query, parameters.ToArray());

            return students;
        }

        public int StudentsCountByClassID(string classId)
        {
            string query = $"Select Count(*) from tbl_enrollment where class_id = '{classId}'";
            object result = this.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }
        public DataTable StudentsByClassID(int pageSize, int offset, string classId,string searchText = "", string searchBy = "")
        {
            string query = $"SELECT tbl_enrollment.enrolled_date as enrolled_date, tbl_enrollment.user_id as enrolled_by, tbl_student.* FROM tbl_enrollment Left Join tbl_student on tbl_enrollment.student_id = tbl_student.student_id where tbl_enrollment.class_id = '{classId}' LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT tbl_enrollment.enrolled_date as enrolled_date, tbl_enrollment.user_id as enrolled_by, tbl_student.* FROM tbl_enrollment Left Join tbl_student on tbl_enrollment.student_id = tbl_student.student_id WHERE tbl_enrollment.class_id = '{classId}' and {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable students = this.ExecuteQuery(query, parameters.ToArray());

            return students;
        }

        

        public DataTable StudentsCountMonthly()
        {
            string year = DateTime.Now.Year.ToString();

            string query = $"SELECT DATE_FORMAT(registered_date, '%m') AS month, COUNT(*) AS student_count FROM tbl_student WHERE DATE_FORMAT(registered_date, '%Y') = '{year}' GROUP BY DATE_FORMAT(registered_date, '%Y-%m') ORDER BY DATE_FORMAT(registered_date, '%Y-%m');";

            DataTable count = this.ExecuteQuery(query);

            return count;
        }
        public bool DeleteStudent(string studentID)
        {
            string query = "DELETE FROM tbl_student WHERE `tbl_student`.`student_id` = @student_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@student_id", studentID)
            };
            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Delete"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", studentID)

            };
            int rowsAffected = 0;

            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.StudentDeletedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
