using MySql.Data.MySqlClient;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
using Samara_Academy.VMs.TeacherVMs;
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
    public class ClassManager : DataBaseHelper
    {
        public ClassManager()
        {

        }

        public DataTable ClassesCountMonthly()
        {
            string year = DateTime.Now.Year.ToString();

            string query = $"SELECT DATE_FORMAT(registered_date, '%m') AS month, COUNT(*) AS class_count FROM tbl_class WHERE DATE_FORMAT(registered_date, '%Y') = '{year}' GROUP BY DATE_FORMAT(registered_date, '%Y-%m') ORDER BY DATE_FORMAT(registered_date, '%Y-%m');";

            DataTable count = this.ExecuteQuery(query);

            return count;
        }

        public bool AddClass(Class cls)
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
                                string classID = "";

                                string query1 = "INSERT INTO `tbl_class` ( `name`, `user_id`, `teacher_id`, `registered_date`, `fee`, `margin`, `time`, `day`, `last_modified_date`) VALUES ( @name, @user_id, @teacher_id, @registered_date, @fee, @margin, @time, @day, @last_modified_date)";

                                using (MySqlCommand cmd = new MySqlCommand(query1, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@name", cls.Name);
                                    cmd.Parameters.AddWithValue("@user_id", cls.InsertedBy);
                                    cmd.Parameters.AddWithValue("@teacher_id", cls.TeacherID);
                                    cmd.Parameters.AddWithValue("@registered_date", cls.RegisteredDate);
                                    cmd.Parameters.AddWithValue("@fee", cls.Fee);
                                    cmd.Parameters.AddWithValue("@margin", cls.Margin);
                                    cmd.Parameters.AddWithValue("@time", cls.Time);
                                    cmd.Parameters.AddWithValue("@day", cls.Day);
                                    cmd.Parameters.AddWithValue("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"));

                                    cmd.ExecuteNonQuery();

                                }

                                string query3 = "SELECT class_id from tbl_class ORDER BY class_id DESC";
                                using (MySqlCommand cmd = new MySqlCommand(query3, connection, transaction))
                                {

                                    classID = cmd.ExecuteScalar().ToString();

                                }
                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";

                                using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                {
                                    MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Insert"),
                                        new MySqlParameter("@user_id", SharedResources.UserID),
                                        new MySqlParameter("@description", classID)

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
                                    var dialogBox = new InfoDialog(DisplayMessages.ClassAddedSuccess);
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

                                    MessageBox.Show(ex.ToString());

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

        public DataTable Classes(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT tbl_class.*, tbl_teacher.name as teacher_name FROM tbl_class LEFT JOIN tbl_teacher on tbl_class.teacher_id = tbl_teacher.teacher_id LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT tbl_class.*, tbl_teacher.name as teacher_name FROM tbl_class LEFT JOIN tbl_teacher on tbl_class.teacher_id = tbl_teacher.teacher_id WHERE tbl_class.{searchBy} LIKE @searchText";

                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable classes = this.ExecuteQuery(query, parameters.ToArray());

            return classes;
        }

        public DataTable Classes(string searchText = "", string searchBy = "")
        {
            string query = $"SELECT tbl_class.*, tbl_teacher.name as teacher_name FROM tbl_class LEFT JOIN tbl_teacher on tbl_class.teacher_id = tbl_teacher.teacher_id";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT tbl_class.*, tbl_teacher.name as teacher_name FROM tbl_class LEFT JOIN tbl_teacher on tbl_class.teacher_id = tbl_teacher.teacher_id WHERE tbl_class.{searchBy} LIKE @searchText";

                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable classes = this.ExecuteQuery(query, parameters.ToArray());

            return classes;
        }
        public int ClassesCount()
        {

            string query = "Select Count(*) from tbl_class";

            object result = this.ExecuteScalar(query);

            return Convert.ToInt32(result);


        }
        public Class GetClassByID(string classId)
        {
            string query = "Select tbl_class.*, tbl_teacher.name as teacher_name from tbl_class left join tbl_teacher on tbl_class.teacher_id = tbl_teacher.teacher_id where tbl_class.class_id = @class_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@class_id", classId)

            };

            DataTable result = this.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {

                DataRow row = result.Rows[0];
                return new Class()
                {
                    ClassID = classId,
                    Name = row["name"].ToString(),
                    Fee = float.Parse(row["fee"].ToString()),
                    Margin = float.Parse(row["margin"].ToString()),
                    Time = row["time"].ToString(),
                    Day = row["day"].ToString(),
                    RegisteredDate = row["registered_date"].ToString(),
                    TeacherID = row["teacher_id"].ToString(),
                    LastModifiedDate = row["last_modified_date"].ToString(),
                    InsertedBy = row["user_id"].ToString(),
                    TeacherName = row["teacher_name"].ToString()


                };
            }

            return new Class();

        }

        public ObservableCollection<Class> GetClassesByTeacherID(string teacherId)
        {
            DataTable results = new DataTable();
            string query = "Select tbl_class.class_id as id, tbl_class.name as name from tbl_class where tbl_class.teacher_id = @teacherId";

            MySqlParameter[] parameters = { new MySqlParameter("@teacherId", teacherId) };

            results = this.ExecuteQuery(query, parameters);

            if (results != null)
            {
                ObservableCollection<Class> classes = new ObservableCollection<Class>();

                int i = 0;
                while (i < results.Rows.Count)
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

        public bool DeleteClass(string classID)
        {
            string query = "DELETE FROM tbl_class WHERE `tbl_class`.`class_id` = @class_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@class_id", classID)
            };
            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Delete"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", classID)

            };
            int rowsAffected = 0;

            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.ClassDeletedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateClass(Class cls)
        {
            string query = "UPDATE `tbl_class` SET `name` = @name, `teacher_id` = @teacher_id, `fee` = @fee, `margin` = @margin, `day` = @day, `time` = @time, , `last_modified_date` = @last_modified_date WHERE `tbl_class`.`class_id` = @class_id";

            MySqlParameter[] parameters = 
            {
                new MySqlParameter("@name", cls.Name),
                new MySqlParameter("@teacher_id", cls.TeacherID),
                new MySqlParameter("@fee", cls.Fee),
                new MySqlParameter("@margin", cls.Margin),
                new MySqlParameter("@day", cls.Day),
                new MySqlParameter("@time", cls.Time),
                new MySqlParameter("@class_id", cls.ClassID),
                new MySqlParameter("@last_modified_date", DateTime.Now.ToString("yyy-MM-dd"))

            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Update"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", cls.ClassID)

            };
            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.ClassUpdatedSuccess);
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
