using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 将.net 数据类型转换为数据库中的数据类型
    /// </summary>
    public class NetTypeConvertDbType
    {
        public static string GetDbType(DbTypeEnum dbTypeEnum, PropertyInfo propertyInfo)
        {
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return GetMySqlDbType(propertyInfo);
                case DbTypeEnum.SqlServer:
                    return GetSqlServerDbType(propertyInfo);
                case DbTypeEnum.Oracle:
                    return GetOracleDbType(propertyInfo);
                case DbTypeEnum.SQLite:
                    return GetSQLiteDbType(propertyInfo);
                default:
                    return GetMySqlDbType(propertyInfo);
            }
        }

        private static string GetMySqlDbType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(int))
            {
                return "INT";
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                return "BIGINT";
            }
            else if (propertyInfo.PropertyType == typeof(float))
            {
                return "FLOAT";
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                return "DOUBLE";
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                return "DECIMAL";
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                return "DATETIME";
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                Type type = propertyInfo.GetType();
                StringLengthAttribute stringLengthAttribute = (StringLengthAttribute)type.GetCustomAttribute(typeof(StringLengthAttribute));
                if (stringLengthAttribute != null)
                {
                    if (stringLengthAttribute.Length > 0)
                    {
                        return "VARCHAR(" + stringLengthAttribute.Length + ")";
                    }
                }
                return "VARCHAR(64)";
            }
            else
            {
                return "VARCHAR(64)";
            }
        }

        private static string GetSqlServerDbType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(int))
            {
                return "INT";
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                return "BIGINT";
            }
            else if (propertyInfo.PropertyType == typeof(float))
            {
                return "FLOAT";
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                return "DOUBLE";
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                return "DECIMAL";
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                return "DATETIME";
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                Type type = propertyInfo.GetType();
                StringLengthAttribute stringLengthAttribute = (StringLengthAttribute)type.GetCustomAttribute(typeof(StringLengthAttribute));
                if (stringLengthAttribute != null)
                {
                    if (stringLengthAttribute.Length > 0)
                    {
                        return "VARCHAR(" + stringLengthAttribute.Length + ")";
                    }
                }
                return "VARCHAR(64)";
            }
            else
            {
                return "VARCHAR(64)";
            }
        }

        private static string GetOracleDbType(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        private static string GetSQLiteDbType(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

    }
}
