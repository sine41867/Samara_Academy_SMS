using MySql.Data.MySqlClient;
using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.Views.DialogBoxes;
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
    public class UserManager: DataBaseHelper
    {
        public UserManager()
        {
            
        }

        public int UsersCount()
        {
            string query = "Select Count(*) from tbl_user";
            object result = this.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public DataTable Users(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_user LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_user WHERE {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable users = this.ExecuteQuery(query, parameters.ToArray());

            return users;
        }
        public bool UpdateUser(User user)
        {
            string query = "UPDATE `tbl_user` SET `mobile` = @mobile WHERE `tbl_user`.`user_id` = @user_id";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@mobile", user.Mobile),
                new MySqlParameter("@user_id", user.UserID)

            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Update"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", user.UserID)

            };

            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.UserUpdatedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdatePassword(string userId, string password)
        {
            string query = "UPDATE `tbl_user` SET `password` = @password WHERE `tbl_user`.`user_id` = @user_id";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@password", password),
                new MySqlParameter("@user_id", userId)

            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Change Password"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", userId)

            };

            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.ChangePasswordSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public User GetUserByID(string userId)
        {
            DataTable result = new DataTable();
            string query = "SELECT name, mobile, user_role, registered_date, inserted_by  FROM tbl_user WHERE user_id = @userId";

            MySqlParameter[] parameters = { new MySqlParameter("@userId", userId) };

            result = this.ExecuteQuery(query, parameters);

            if (result == null)
            {
                return null;
            }

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];

                return new User()
                {
                    Name = row["name"].ToString(),
                    Mobile = row["mobile"].ToString(),
                    UserRole = row["user_role"].ToString(),
                    DateOfRegistration = row["registered_date"].ToString(),
                    UserID = userId,
                    InsertedBy = row["inserted_by"].ToString()
                };
            }
            else
            {
                return null;
            }

        }
        public bool DeleteUser(string userID)
        {
            string query = "DELETE FROM tbl_user WHERE `tbl_user`.`user_id` = @user_id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@user_id", userID)
            };
            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Delete"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", userID)

            };
            int rowsAffected = 0;

            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.UserDeletedSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AddUser(User user)
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
                                string query1 = "INSERT INTO `tbl_user` ( `user_id`, `password`,  `name`, `mobile`, `inserted_by`, `registered_date`, `user_role`, `is_disable`) VALUES (@user_id, @password, @name, @mobile, @inserted_by, @registered_date, @user_role, @is_disable)";

                                using (MySqlCommand cmd = new MySqlCommand(query1, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@user_id", user.UserID);
                                    cmd.Parameters.AddWithValue("@password", user.Password);
                                    cmd.Parameters.AddWithValue("@name", user.Name);
                                    cmd.Parameters.AddWithValue("@mobile", user.Mobile);
                                    cmd.Parameters.AddWithValue("@inserted_by", user.InsertedBy);
                                    cmd.Parameters.AddWithValue("@registered_date", user.DateOfRegistration);
                                    cmd.Parameters.AddWithValue("@user_role", user.UserRole);
                                    cmd.Parameters.AddWithValue("@is_disable", false);

                                    cmd.ExecuteNonQuery();

                                }


                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";

                                using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                {
                                    MySqlParameter[] logParameters = {
                                         new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                                        new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                                        new MySqlParameter("@type", "Insert"),
                                        new MySqlParameter("@user_id", SharedResources.UserID),
                                        new MySqlParameter("@description", user.UserID)

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
                                    var dialogBox = new InfoDialog(DisplayMessages.UserAddedSuccess);
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

        public bool EnableUser(string userId)
        {
            string query = "UPDATE `tbl_user` SET `is_disable` = @is_disable WHERE `tbl_user`.`user_id` = @user_id";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@is_disable", false),
                new MySqlParameter("@user_id", userId),

            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Activate"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", userId)

            };

            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.UserEnabledSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DisableUser(string userId)
        {
            string query = "UPDATE `tbl_user` SET `is_disable` = @is_disable WHERE `tbl_user`.`user_id` = @user_id";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@is_disable", true),
                new MySqlParameter("@user_id", userId),
            };

            MySqlParameter[] logParameters = {
                 new MySqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new MySqlParameter("@type", "Deactivate"),
                new MySqlParameter("@user_id", SharedResources.UserID),
                new MySqlParameter("@description", userId)

            };

            int rowsAffected = 0;
            rowsAffected = this.ExecuteNonQuery(query, parameters, logParameters);

            if (rowsAffected > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var dialogBox = new InfoDialog(DisplayMessages.UserDisabledSuccess);
                    dialogBox.ShowDialog();

                });
                return true;
            }
            else
            {
                return false;
            }
        }
        public User GetFullUserByID(string userId)
        {
            DataTable result = new DataTable();
            string query = "SELECT *  FROM tbl_user WHERE user_id = @userId";

            MySqlParameter[] parameters = { new MySqlParameter("@userId", userId) };

            result = this.ExecuteQuery(query, parameters);

            if (result == null)
            {
                return null;
            }

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];

                return new User()
                {
                    Name = row["name"].ToString(),
                    Mobile = row["mobile"].ToString(),
                    UserRole = row["user_role"].ToString(),
                    DateOfRegistration = row["registered_date"].ToString(),
                    UserID = userId,
                    InsertedBy = row["inserted_by"].ToString(),
                    Password = row["password"].ToString(),
                    IsDisable = (bool)row["is_disable"]
                };
            }
            else
            {
                return null;
            }

        }



    }
}
