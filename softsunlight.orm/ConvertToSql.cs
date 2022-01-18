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
            dbDataParameters = new List<IDbDataParameter>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return GetBatchInsertSql(dbTypeEnum, entity, out dbDataParameters);
            }
            string tableName = GetTableName(type);
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

                if (value != null && !IsDefaultValue(propertyInfo.PropertyType, value))
                {
                    columnList.Add(GetSafeName(dbTypeEnum, propertyInfo.Name));
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
            }
            sqlBuilder.Append("(" + string.Join(",", columnList) + ")").Append(" VALUES(" + string.Join(",", dbDataParameters.Select(p => p.ParameterName)) + ")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        private static string GetBatchInsertSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            dbDataParameters = new List<IDbDataParameter>();
            Type type = typeof(T);
            //泛型集合
            if (type.GenericTypeArguments.Length <= 0)
            {
                return string.Empty;
            }
            var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Count")?.GetValue(entity));
            var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
            string tableName = string.Empty;
            PropertyInfo[] propertyInfos = null;
            StringBuilder sqlBuilder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                var obj = itemProperty.GetValue(entity, new object[] { i });
                if (i == 0)
                {
                    Type instanceType = obj.GetType();
                    tableName = GetTableName(instanceType);
                    propertyInfos = ReflectionHelper.GetPropertyInfos(instanceType);
                    sqlBuilder.Append("INSERT INTO " + GetSafeName(dbTypeEnum, tableName) + "(" + string.Join(",", propertyInfos.Where(p => !p.Name.Equals("Id")).Select(p => GetSafeName(dbTypeEnum, p.Name))) + ")").Append(" VALUES");
                }
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    object? value = propertyInfo.GetValue(obj);
                    if (propertyInfo.Name.Equals("Id") && (value is int))
                    {
                        continue;
                    }
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name + "_" + i, value));
                }
                if (i != count - 1)
                {
                    sqlBuilder.Append("(" + string.Join(",", propertyInfos.Where(p => !p.Name.Equals("Id")).Select(p => "@" + p.Name + "_" + i)) + ")").Append(",");
                }
                else
                {
                    sqlBuilder.Append("(" + string.Join(",", propertyInfos.Where(p => !p.Name.Equals("Id")).Select(p => "@" + p.Name + "_" + i)) + ")").Append(";");
                }
            }
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
            dbDataParameters = new List<IDbDataParameter>();
            Type type = entity.GetType();
            if (type.IsGenericType)
            {
                return sqlBuilder.ToString();
            }
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            string whereSql = "";
            sqlBuilder.Append("UPDATE " + GetSafeName(dbTypeEnum, tableName) + " SET");
            List<string> paramList = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                long tick = DateTime.Now.Ticks;
                if (propertyInfo.Name.Equals("Id") && (value is int))
                {
                    whereSql = " WHERE Id=@Id_" + tick + ";";
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name + "_" + tick, value));
                }
                else
                {
                    if (value != null && !IsDefaultValue(propertyInfo.PropertyType, value))
                    {
                        paramList.Add(GetSafeName(dbTypeEnum, propertyInfo.Name) + "=@" + propertyInfo.Name + "_" + tick);
                        dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name + "_" + tick, value));
                    }
                }
            }
            sqlBuilder.Append(" ").Append(string.Join(",", paramList));
            sqlBuilder.Append(whereSql);
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
            dbDataParameters = new List<IDbDataParameter>();
            Type type = entity.GetType();
            if (type.IsGenericType)
            {
                return GetBatchBatchDeleteSql(dbTypeEnum, entity, out dbDataParameters);
            }
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("DELETE FROM ").Append(GetSafeName(dbTypeEnum, tableName));
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (propertyInfo.Name.Equals("Id") && (value is int))
                {
                    Guid guid = Guid.NewGuid();
                    sqlBuilder.Append(" WHERE Id=@Id_" + guid + ";");
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name + "_" + guid, value));
                    break;
                }
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 将实体转换为批量删除语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTypeEnum"></param>
        /// <param name="entity"></param>
        /// <param name="dbDataParameters"></param>
        /// <returns></returns>
        private static string GetBatchBatchDeleteSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters)
        {
            dbDataParameters = new List<IDbDataParameter>();
            Type type = typeof(T);
            //泛型集合
            if (type.GenericTypeArguments.Length <= 0)
            {
                return string.Empty;
            }
            var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Count")?.GetValue(entity));
            var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
            string tableName = string.Empty;
            PropertyInfo[] propertyInfos = null;
            StringBuilder sqlBuilder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                var obj = itemProperty.GetValue(entity, new object[] { i });
                if (i == 0)
                {
                    Type instanceType = obj.GetType();
                    tableName = GetTableName(instanceType);
                    propertyInfos = ReflectionHelper.GetPropertyInfos(instanceType);
                    sqlBuilder.Append("DELETE FROM ").Append(GetSafeName(dbTypeEnum, tableName)).Append(" WHERE Id in (");
                }
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    object? value = propertyInfo.GetValue(obj);
                    if (propertyInfo.Name.Equals("Id") && (value is int))
                    {
                        sqlBuilder.Append(value);
                        if (i != count - 1)
                        {
                            sqlBuilder.Append(",");
                        }
                        dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name + "_" + i, value));
                        continue;
                    }
                }
            }
            sqlBuilder.Append(")");
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
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return GetMySqlSelectTableSql<T>(dbTypeEnum, entity, out dbDataParameters, pageModel);
                case DbTypeEnum.SqlServer:
                    return GetSqlServerSelectTableSql<T>(dbTypeEnum, entity, out dbDataParameters, pageModel);
                default:
                    return GetMySqlSelectTableSql<T>(dbTypeEnum, entity, out dbDataParameters, pageModel);
            }
        }

        private static string GetMySqlSelectTableSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters, PageModel pageModel)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = new List<IDbDataParameter>();
            if (pageModel != null)
            {
                if (pageModel.PageNo <= 0)
                {
                    pageModel.PageNo = 1;
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
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("SELECT " + string.Join(",", propertyInfos.Select(p => GetSafeName(dbTypeEnum, p.Name))) + " FROM " + GetSafeName(dbTypeEnum, tableName) + " WHERE 1=1");
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (value != null && !IsDefaultValue(propertyInfo.PropertyType, value))
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

        private static string GetSqlServerSelectTableSql<T>(DbTypeEnum dbTypeEnum, T entity, out IList<IDbDataParameter> dbDataParameters, PageModel pageModel)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            dbDataParameters = new List<IDbDataParameter>();
            if (pageModel != null)
            {
                if (pageModel.PageNo <= 0)
                {
                    pageModel.PageNo = 1;
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
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("SELECT " + string.Join(",", propertyInfos.Select(p => GetSafeName(dbTypeEnum, p.Name))) + (pageModel != null ? ",ROW_NUMBER() OVER(ORDER BY Id ASC) AS rownum" : "") + " FROM " + GetSafeName(dbTypeEnum, tableName) + " WHERE 1=1");
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (value != null && !IsDefaultValue(propertyInfo.PropertyType, value))
                {
                    sqlBuilder.Append(" and " + propertyInfo.Name + "=@" + propertyInfo.Name);
                    dbDataParameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, "@" + propertyInfo.Name, value));
                }
            }
            if (pageModel != null)
            {
                sqlBuilder.Append(" and (rownum >= " + ((pageModel.PageNo - 1) * pageModel.PageSize + 1) + " and rownum <= " + (pageModel.PageNo * pageModel.PageSize) + " )");
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 获取类型数据库表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
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

            return tableName;
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
            switch (dbTypeEnum)
            {
                case DbTypeEnum.MySql:
                    return GetMySqlCreateTableSql<T>();
                case DbTypeEnum.SqlServer:
                    return GetSqlServerCreateTableSql<T>();
                default:
                    return GetMySqlCreateTableSql<T>();
            }
        }

        private static string GetMySqlCreateTableSql<T>()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("参数T不能为泛型集合");
            }
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("CREATE TABLE ").Append(GetSafeName(DbTypeEnum.MySql, tableName)).Append("(").Append(Environment.NewLine);
            string primaryKey = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(DbTypeEnum.MySql, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(DbTypeEnum.MySql, propertyInfo));
                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long))
                    {
                        sqlBuilder.Append(" AUTO_INCREMENT,");
                    }
                    sqlBuilder.Append(Environment.NewLine);
                    primaryKey = "\tPRIMARY KEY (" + GetSafeName(DbTypeEnum.MySql, propertyInfo.Name) + ")";
                }
                else
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(DbTypeEnum.MySql, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(DbTypeEnum.MySql, propertyInfo)).Append(",").Append(Environment.NewLine);
                }
            }
            sqlBuilder.Append(primaryKey).Append(Environment.NewLine);
            sqlBuilder.Append(")ENGINE=InnoDB DEFAULT CHARSET=utf8;");
            return sqlBuilder.ToString();
        }

        private static string GetSqlServerCreateTableSql<T>()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("参数T不能为泛型集合");
            }
            string tableName = GetTableName(type);
            PropertyInfo[] propertyInfos = ReflectionHelper.GetPropertyInfos(type);
            sqlBuilder.Append("CREATE TABLE ").Append(GetSafeName(DbTypeEnum.SqlServer, tableName)).Append("(").Append(Environment.NewLine);
            string primaryKey = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(DbTypeEnum.SqlServer, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(DbTypeEnum.SqlServer, propertyInfo)).Append(" PRIMARY KEY");
                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long))
                    {
                        sqlBuilder.Append(" IDENTITY (1, 1),");
                    }
                    sqlBuilder.Append(Environment.NewLine);
                }
                else
                {
                    sqlBuilder.Append("\t").Append(GetSafeName(DbTypeEnum.SqlServer, propertyInfo.Name)).Append(" ").Append(NetTypeConvertDbType.GetDbType(DbTypeEnum.SqlServer, propertyInfo)).Append(",").Append(Environment.NewLine);
                }
            }
            sqlBuilder.Append(primaryKey).Append(Environment.NewLine);
            sqlBuilder.Append(")");
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

        /// <summary>
        /// 判断值是否为默认值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsDefaultValue(Type type, object? value)
        {
            if (type == typeof(int))
            {
                if ((int)value == 0)
                {
                    return true;
                }
            }
            else if (type == typeof(long))
            {
                if ((long)value == 0)
                {
                    return true;
                }
            }
            else if (type == typeof(float))
            {
                if ((float)value == 0)
                {
                    return true;
                }
            }
            else if (type == typeof(double))
            {
                if ((double)value == 0)
                {
                    return true;
                }
            }
            else if (type == typeof(decimal))
            {
                if ((decimal)value == 0)
                {
                    return true;
                }
            }
            else if (type == typeof(DateTime))
            {
                if ((DateTime)value == DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
