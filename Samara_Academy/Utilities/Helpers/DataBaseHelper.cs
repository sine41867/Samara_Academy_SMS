using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Samara_Academy.Models;
using Samara_Academy.Views.DialogBoxes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Samara_Academy.Utilities.Helpers
{
    public class DataBaseHelper
    {
        protected readonly string _connectionString = ConfigurationManager.ConnectionStrings["local"].ConnectionString;

        public DataBaseHelper()
        {

        }

        public DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    try
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {

                            if (parameters != null)
                            {

                                command.Parameters.AddRange(parameters);
                            }

                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var dialogBox = new InfoDialog(ex.ToString());
                            dialogBox.ShowDialog();


                        });
                        MessageBox.Show(ex.ToString());
                        return null;

                    }

                }
                catch
                {
                    return null;

                }
            }

            return dataTable;
        }

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null , MySqlParameter[] logParameters = null)
        {
            int rowsAffected = 0;

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
                               
                                string logQuery = "INSERT INTO `tbl_log` (`time`, `date`, `type`, `user_id`, `description`) VALUES (@time, @date, @type, @user_id, @description)";

                                using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                                {

                                    if (parameters != null)
                                    {
                                        command.Parameters.AddRange(parameters);
                                    }
                                    rowsAffected = command.ExecuteNonQuery();
                                }

                                using (MySqlCommand command = new MySqlCommand(logQuery, connection, transaction))
                                {
                                    if(rowsAffected > 0)
                                    {
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

                                }

                                transaction.Commit();


                                return rowsAffected;

                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    var dialogBox = new InfoDialog($"An Error Occured...{ex}");
                                    dialogBox.ShowDialog();

                                    MessageBox.Show(ex.ToString());
                                });


                                return 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return 0;

                    }

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.ToString());
                        /*
                        var dialogBox = new InfoDialog($"An Error Occured...");
                        dialogBox.ShowDialog();
                        */
                    });

                    return 0;


                }
            }

        }

        public object ExecuteScalar(string query, MySqlParameter[] parameters = null)
        {
            object result = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    try
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {

                            if (parameters != null)
                            {
                                command.Parameters.AddRange(parameters);
                            }

                            result = command.ExecuteScalar();
                        }
                    }
                    catch (Exception ex)
                    {
                        /*
                        var dialogBox = new InfoDialog(ex.ToString());
                        dialogBox.ShowDialog();
                        */
                        return -1;

                    }

                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        /*
                        var dialogBox = new InfoDialog(DisplayMessages.ConnectionFailure);
                        dialogBox.ShowDialog();
                        */
                        return -1;

                    });

                }
            }

            return result;
        }

        public Dictionary<string, int> ExecuteDictQuery(string query)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    try
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dictionary.Add(reader.GetString(1), reader.GetInt32(0));
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());

                    }

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.ToString());
                        /*
                        var dialogBox = new InfoDialog($"An error occured...");
                        dialogBox.ShowDialog();
                        */
                    });

                }
            }

            return dictionary;
        }



    }
}
