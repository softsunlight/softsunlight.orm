using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 数据库连接工厂类
    /// </summary>
    public class DbConnectionFactory
    {
        /// <summary>
        /// 根据不同的数据库类型创建数据库连接类
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
                    return new SqliteConnection();
                default:
                    return new MySqlConnection();
            }
        }
    }
}
