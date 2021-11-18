using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// sql工具类，用于获取通用的Connection、SqlCommand、SqlParameter等
    /// </summary>
    public class SqlUtils
    {
        /// <summary>
        /// 获取Command对象
        /// </summary>
        /// <param name="dbTypeEnum"></param>
        /// <returns></returns>
        public static DbCommand GetDbCommand(DbTypeEnum dbTypeEnum)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return new MySqlCommand();
                case DbTypeEnum.SqlServer:
                    return new SqlCommand();
                case DbTypeEnum.Oracle:
                    return new OracleCommand();
                case DbTypeEnum.SQLite:
                    return new SQLiteCommand();
                default:
                    return new MySqlCommand();
            }
        }

        /// <summary>
        /// 获取DataAdapter对象
        /// </summary>
        /// <param name="dbTypeEnum"></param>
        /// <returns></returns>
        public static DbDataAdapter GetDbDataAdapter(DbTypeEnum dbTypeEnum)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return new MySqlDataAdapter();
                case DbTypeEnum.SqlServer:
                    return new SqlDataAdapter();
                case DbTypeEnum.Oracle:
                    return new OracleDataAdapter();
                case DbTypeEnum.SQLite:
                    return new SQLiteDataAdapter();
                default:
                    return new MySqlDataAdapter();
            }
        }

        /// <summary>
        /// 获取Connection对象
        /// </summary>
        /// <param name="dbTypeEnum"></param>
        /// <returns></returns>
        public static IDbConnection GetDbConnection(DbTypeEnum dbTypeEnum)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return new MySqlConnection();
                case DbTypeEnum.SqlServer:
                    return new SqlConnection();
                case DbTypeEnum.Oracle:
                    return new OracleConnection();
                case DbTypeEnum.SQLite:
                    return new SQLiteConnection();
                default:
                    return new MySqlConnection();
            }
        }

        /// <summary>
        /// 获取DataParameter对象
        /// </summary>
        /// <param name="dbTypeEnum"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter GetDbDataParameter(DbTypeEnum dbTypeEnum, string paramName, object value)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return new MySqlParameter(paramName, value);
                case DbTypeEnum.SqlServer:
                    return new SqlParameter(paramName, value);
                case DbTypeEnum.Oracle:
                    return new OracleParameter(paramName, value);
                case DbTypeEnum.SQLite:
                    return new SQLiteParameter(paramName, value);
                default:
                    return new MySqlParameter(paramName, value);
            }
        }

    }
}
