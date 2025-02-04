using MySql.Data.MySqlClient;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.StudentVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Samara_Academy.DatabaseManagers
{
    public class TeacherManager : DataBaseHelper
    {
        public TeacherManager()
        {

        }
        public bool AddTeacher(Teacher teacher)
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
                                string teacherId = "";

                                string query = "INSERT INTO `tbl_teacher` ( `name`, `user_id`, `registered_date`, `mobile`, `whatsapp`, `last_modified_date`) VALUES (@name, @user_id, @registered_date, @mobile, @whatsapp, @last_modified_date)";
                                using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@name", teacher.Name);
                                    cmd.Parameters.AddWithValue("@user_id", teacher.InsertedBy);
                                    cmd.Parameters.AddWithValue("@registered_date", teacher.RegisteredDate);
                                    cmd.Parameters.AddWithValue("@mobile", teacher.Mobile);
                                    cmd.Parameters.AddWithValue("@whatsapp", teacher.WhatsApp);
                                    cmd.Parameters.AddWithValue("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"));


                                    cmd.ExecuteNonQuery();

                                }
                                string query3 = "SELECT teacher_id from tbl_teacher ORDER BY teacher_id DESC";
                                using (MySqlCommand cmd = new MySqlCommand(query3, connection, transaction))
                                {

                                    teacherId = cmd.ExecuteScalar().ToString();

                                }
                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";

                                using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                {
                                    MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Insert"),
                                        new MySqlParameter("@user_id", SharedResources.UserID),
                                        new MySqlParameter("@description", teacherId)

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

                                transaction.Commit();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dialogBox = new InfoDialog(DisplayMessages.TeacherAddedSuccess);
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
        public bool IsValidID(string teacherId)
        {

            string query = "Select Count(*) from tbl_teacher WHERE teacher_id = @teacher_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@teacher_id", teacherId)
            };


            object result = this.ExecuteScalar(query, parameters);
            try
            {
                if (Convert.ToInt32(result) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }




        }

        public int TeachersCount()
        {

            string query = "Select Count(*) from tbl_teacher";

            object result = this.ExecuteScalar(query);

            return Convert.ToInt32(result);


        }

        public DataTable Teachers(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_teacher LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_teacher WHERE {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable teachers = this.ExecuteQuery(query, parameters.ToArray());

            return teachers;
        }

        public DataTable Teachers(string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_teacher";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_teacher WHERE {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable teachers = this.ExecuteQuery(query, parameters.ToArray());

            return teachers;
        }
        public Teacher GetTeacherByID(string teacherID)
        {
            DataTable result = new DataTable();
            string query = "SELECT * FROM tbl_teacher WHERE teacher_id = @teacherID";

            MySqlParameter[] parameters = { new MySqlParameter("@teacherID", teacherID) };

            result = this.ExecuteQuery(query, parameters);

            if (result == null)
            {
                return null;
            }

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];

                return new Teacher()
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
        public bool DeleteTeacher(string teacherID)
        {
            string query = "DELETE FROM tbl_teacher WHERE `tbl_teacher`.`teacher_id` = @teacher_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@teacher_id", teacherID)
            };
            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Delete"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", teacherID)

            };
            int rowsAffected = 0;

            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.TeacherDeletedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UpdateTeacher(Teacher teacher)
        {
            string query = "UPDATE `tbl_teacher` SET `name` = @name, `mobile` = @mobile, `whatsapp` = @whatsapp, `last_modified_date` = @last_modified_date WHERE `tbl_teacher`.`teacher_id` = @teacher_id";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", teacher.Name),
                new MySqlParameter("@mobile", teacher.Mobile),
                new MySqlParameter("@whatsapp", teacher.WhatsApp),
                new MySqlParameter("@teacher_id", teacher.TeacherID),
                new MySqlParameter("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"))
            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Update"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", teacher.TeacherID)

            };
            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.TeacherUpdatedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable TeachersCountMonthly()
        {
            string year = DateTime.Now.Year.ToString();

            string query = $"SELECT DATE_FORMAT(registered_date, '%m') AS month, COUNT(*) AS teacher_count FROM tbl_teacher WHERE DATE_FORMAT(registered_date, '%Y') = '{year}' GROUP BY DATE_FORMAT(registered_date, '%Y-%m') ORDER BY DATE_FORMAT(registered_date, '%Y-%m');";

            DataTable count = this.ExecuteQuery(query);

            return count;
        }

    }
}
