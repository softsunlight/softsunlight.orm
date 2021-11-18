using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 将.net 数据类型转换为数据库中的数据类型
    /// </summary>
    public class NetTypeConvertDbType
    {
        public static string GetDbType(DbTypeEnum dbTypeEnum, Type type)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return GetMySqlDbType(type);
                case DbTypeEnum.SqlServer:
                    return GetSqlServerDbType(type);
                case DbTypeEnum.Oracle:
                    return GetOracleDbType(type);
                case DbTypeEnum.SQLite:
                    return GetSQLiteDbType(type);
                default:
                    return GetMySqlDbType(type);
            }
        }

        private static string GetMySqlDbType(Type type)
        {
            if (type == typeof(int))
            {
                return "INT";
            }
            else if (type == typeof(int))
            {
                return "BIGINT";
            }
            else if (type == typeof(float))
            {
                return "FLOAT";
            }
            else if (type == typeof(double))
            {
                return "DOUBLE";
            }
            else if (type == typeof(decimal))
            {
                return "DECIMAL";
            }
            else if (type == typeof(DateTime))
            {
                return "DATETIME";
            }
            else
            {
                return "VARCHAR(64)";
            }
        }

        private static string GetSqlServerDbType(Type type)
        {
            return "";
        }

        private static string GetOracleDbType(Type type)
        {
            return "";
        }

        private static string GetSQLiteDbType(Type type)
        {
            return "";
        }

    }
}
