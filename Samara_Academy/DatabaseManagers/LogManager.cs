using MySql.Data.MySqlClient;
using Samara_Academy.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.DatabaseManagers
{
    public class LogManager:DataBaseHelper
    {
        public LogManager()
        {
        }
        public DataTable Logs(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_log LIMIT {pageSize} OFFSET {offset}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_log WHERE {searchBy} LIKE @searchText";


                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));
            }
            DataTable logs = this.ExecuteQuery(query, parameters.ToArray());

            return logs;
        }

        public DataTable OwnLogs(int pageSize, int offset, string searchText = "", string searchBy = "")
        {
            string query = $"SELECT * FROM tbl_log WHERE user_id = @user_id LIMIT {pageSize} OFFSET {offset} ";

            List<MySqlParameter> parameters = [new MySqlParameter("@user_id", SharedResources.UserID)];


            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
            {
                query = $"SELECT * FROM tbl_log WHERE user_id = @user_id AND {searchBy} LIKE @searchText ";

                parameters.Add(new MySqlParameter("@searchText", "%" + searchText + "%"));

            }
            DataTable logs = this.ExecuteQuery(query, parameters.ToArray());

            return logs;
        }
        public int LogsCount()
        {
            string query = "Select Count(*) from tbl_log";
            object result = this.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int OwnLogsCount()
        {
            string query = "Select Count(*) from tbl_log WHERE user_id = @user_id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@user_id", SharedResources.UserID)
            };

            object result = this.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }


    }
}
