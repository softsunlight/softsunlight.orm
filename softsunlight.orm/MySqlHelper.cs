using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// MySql数据库操作类
    /// </summary>
    public class MySqlHelper : ISqlHelper
    {
        public MySqlHelper(string connectionStr)
        {

        }

        public MySqlHelper(MySqlConnection sqlConnection)
        {

        }

        public DataTable GetDataTable(string sql)
        {
            DataTable dt=null;

            return dt;
        }
    }
}
