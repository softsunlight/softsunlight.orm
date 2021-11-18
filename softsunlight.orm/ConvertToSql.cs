using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 将实体类转换成sql语句
    /// </summary>
    public class ConvertToSql
    {
        /// <summary>
        /// 将实体转换为插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        public static string GetInsertSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = Array.Empty<IDbDataParameter>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return sqlBuilder.ToString();
            }
            string tableName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                TableAttribute tableAttribute = (TableAttribute)attributes[0];
                if (!string.IsNullOrEmpty(tableAttribute.TableName))
                {
                    tableName = tableAttribute.TableName;
                }
            }
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);

            List<string> columnList = new List<string>();
            sqlBuilder.Append("INSERT INTO ").Append(GetSafeName(dbTypeEnum, tableName));
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (propertyInfo.Name.Equals("Id") && (value is int))
                {
                    continue;
                }

                if (value != null)
                {
                    columnList.Add(GetSafeName(dbTypeEnum, propertyInfo.Name));
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
            }
            sqlBuilder.Append("(" + string.Join(",", columnList) + ")").Append(" VALUES(" + string.Join(",", dbDataParameters.Select(p => p.ParameterName)) + ")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 将实体转换为更新语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        public static string GetUpdateSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = Array.Empty<IDbDataParameter>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return sqlBuilder.ToString();
            }
            string tableName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                TableAttribute tableAttribute = (TableAttribute)attributes[0];
                if (!string.IsNullOrEmpty(tableAttribute.TableName))
                {
                    tableName = tableAttribute.TableName;
                }
            }
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);

            sqlBuilder.Append("UPDATE " + GetSafeName(dbTypeEnum, tableName) + " SET");
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (propertyInfo.Name.Equals("Id") && (value is int))
                {
                    sqlBuilder.Append(" WHERE Id=@Id;");
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
                else
                {
                    if (value != null)
                    {
                        sqlBuilder.Append(" " + propertyInfo.Name + "=@" + propertyInfo.Name);
                        dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                    }
                }
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 将实体转换为删除语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        public static string GetDeleteSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = Array.Empty<IDbDataParameter>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return sqlBuilder.ToString();
            }
            string tableName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                TableAttribute tableAttribute = (TableAttribute)attributes[0];
                if (!string.IsNullOrEmpty(tableAttribute.TableName))
                {
                    tableName = tableAttribute.TableName;
                }
            }
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("DELETE FROM ").Append(GetSafeName(dbTypeEnum, tableName));
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (propertyInfo.Name.Equals("Id") && (value is int))
                {
                    sqlBuilder.Append(" WHERE Id=@Id;");
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 将实体转换为查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        public static string GetSelectSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            return GetSelectSql<T>(dbTypeEnum, entity, out dbDataParameters, null);
        }

        /// <summary>
        /// 将实体转换为查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <param name="pageModel"></param>
        /// <returns></returns>
        public static string GetSelectSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters, PageModel pageModel)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = Array.Empty<IDbDataParameter>();
            if (pageModel != null)
            {
                if (pageModel.PageNo <= 0)
                {
                    pageModel.PageNo = 0;
                }
                if (pageModel.PageSize <= 0 || pageModel.PageSize > 200)
                {
                    pageModel.PageSize = 20;
                }
            }
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return sqlBuilder.ToString();
            }
            string tableName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                TableAttribute tableAttribute = (TableAttribute)attributes[0];
                if (!string.IsNullOrEmpty(tableAttribute.TableName))
                {
                    tableName = tableAttribute.TableName;
                }
            }
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("SELECT " + string.Join(",", propertyInfos.Select(p => "`" + p.Name + "`")) + " FROM " + GetSafeName(dbTypeEnum, tableName) + " WHERE 1=1");
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    sqlBuilder.Append(" and " + propertyInfo.Name + "=@" + propertyInfo.Name);
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
            }
            if (pageModel != null)
            {
                sqlBuilder.Append(" limit " + (pageModel.PageNo - 1) * pageModel.PageSize + ",").Append(pageModel.PageSize);
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 根据类型获取创建表的SQL语句
        /// to do:现在是根据Mysql来创建表，不同的数据库创建表有点不同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <returns></returns>
        public static string GetCreateTableSql<T>(DbTypeEnum dbTypeEnum)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("参数T不能为泛型集合");
            }
            string tableName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                TableAttribute tableAttribute = (TableAttribute)attributes[0];
                if (!string.IsNullOrEmpty(tableAttribute.TableName))
                {
                    tableName = tableAttribute.TableName;
                }
            }
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("CREATE TABLE ").Append(GetSafeName(dbTypeEnum, tableName)).Append("(").Append(Environment.NewLine);
            string primaryKey = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(dbTypeEnum, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(dbTypeEnum, propertyInfo.PropertyType));
                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long))
                    {
                        sqlBuilder.Append(" AUTO_INCREMENT,");
                    }
                    sqlBuilder.Append(Environment.NewLine);
                    primaryKey = "\tPRIMARY KEY (" + GetSafeName(dbTypeEnum, propertyInfo.Name) + ")";
                }
                else
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(dbTypeEnum, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(dbTypeEnum, propertyInfo.PropertyType)).Append(",").Append(Environment.NewLine);
                }
            }
            sqlBuilder.Append(primaryKey).Append(Environment.NewLine);
            sqlBuilder.Append(")ENGINE=InnoDB DEFAULT CHARSET=utf8;");
            return sqlBuilder.ToString();
        }

        private static string GetSafeName(DbTypeEnum dbTypeEnum, string name)
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return stringBuilder.Append("`").Append(name).Append("`").ToString();
                case DbTypeEnum.SqlServer:
                    return stringBuilder.Append("[").Append(name).Append("]").ToString();
                case DbTypeEnum.Oracle:
                    return stringBuilder.Append("").Append(name).Append("").ToString();
                case DbTypeEnum.SQLite:
                    return stringBuilder.Append("").Append(name).Append("").ToString();
                default:
                    return stringBuilder.Append("`").Append(name).Append("`").ToString();
            }
        }

    }
}
