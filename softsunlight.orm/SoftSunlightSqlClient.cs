using MySql.Data.MySqlClient;
using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using softsunlight.orm.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace softsunlight.orm
{
    public class SoftSunlightSqlClient
    {
        /// <summary>
        /// 数据库操作类
        /// </summary>
        private ISqlHelper sqlHelper;

        /// <summary>
        /// 默认使用MySql数据库
        /// </summary>
        public SoftSunlightSqlClient() : this(DbTypeEnum.MySql)
        {

        }

        public SoftSunlightSqlClient(DbTypeEnum dbTypeEnum)
        {
            //sqlHelper = DbConnectionFactory.GetDbConnection(dbTypeEnum);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Add<T>(T entity)
        {
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                //泛型集合
                if (type.GenericTypeArguments.Length <= 0)
                {
                    return;
                }
                var count = Convert.ToInt32(type.GetProperty("Count")?.GetValue(entity));
                var itemProperty = type.GetProperty("Item");//索引器属性
                string tableName = string.Empty;
                PropertyInfo[] propertyInfos = null;
                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                for (var i = 0; i < count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i });
                    if (i == 0)
                    {
                        Type instanceType = obj.GetType();
                        tableName = instanceType.Name;
                        var attributes = instanceType.GetCustomAttributes(typeof(TableAttribute), false);
                        if (attributes.Length > 0)
                        {
                            TableAttribute tableAttribute = (TableAttribute)attributes[0];
                            if (!string.IsNullOrEmpty(tableAttribute.TableName))
                            {
                                tableName = tableAttribute.TableName;
                            }
                        }
                        propertyInfos = ReflectionHelper.GetPropertyInfos(instanceType);
                        sqlBuilder.Append("INSERT INTO `" + tableName + "`(" + string.Join(",", propertyInfos.Select(p => "`" + p.Name + "`")) + ")").Append(" VALUES");
                    }
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        object? value = propertyInfo.GetValue(obj);
                        if (propertyInfo.Name.Equals("Id") && (value is int))
                        {
                            continue;
                        }
                        dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name + "_" + i, value));
                    }
                    sqlBuilder.Append("(" + string.Join(",", propertyInfos.Select(p => "@" + p.Name + "_" + i)) + ")").Append("\r\n");
                }
                Log.Write(sqlBuilder.ToString());
            }
            else
            {
                //非泛型类
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

                StringBuilder sqlBuilder = new StringBuilder();
                List<string> columnList = new List<string>();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                sqlBuilder.Append("INSERT INTO `" + tableName + "`");
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    object? value = propertyInfo.GetValue(entity);
                    if (propertyInfo.Name.Equals("Id") && (value is int))
                    {
                        continue;
                    }

                    if (value != null)
                    {
                        columnList.Add("`" + propertyInfo.Name + "`");
                        dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name, value));
                    }
                }
                sqlBuilder.Append("(" + string.Join(",", columnList) + ")").Append(" VALUES(" + string.Join(",", dbDataParameters.Select(p => p.ParameterName)) + ")");
                //MySqlCommand cmd = new MySqlCommand(sqlBuilder.ToString(), (MySqlConnection)dbConnection);
                Log.Write(sqlBuilder.ToString());
                //cmd.Parameters.AddRange(dbDataParameters.ToArray());
                //cmd.ExecuteNonQuery();
            }
        }

        public void Delete<T>(T entity)
        {
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                //泛型集合
                if (type.GenericTypeArguments.Length <= 0)
                {
                    return;
                }
                var count = Convert.ToInt32(type.GetProperty("Count")?.GetValue(entity));
                var itemProperty = type.GetProperty("Item");//索引器属性
                string tableName = string.Empty;
                PropertyInfo[] propertyInfos = null;
                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                for (var i = 0; i < count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i });
                    if (i == 0)
                    {
                        Type instanceType = obj.GetType();
                        tableName = instanceType.Name;
                        var attributes = instanceType.GetCustomAttributes(typeof(TableAttribute), false);
                        if (attributes.Length > 0)
                        {
                            TableAttribute tableAttribute = (TableAttribute)attributes[0];
                            if (!string.IsNullOrEmpty(tableAttribute.TableName))
                            {
                                tableName = tableAttribute.TableName;
                            }
                        }
                        propertyInfos = ReflectionHelper.GetPropertyInfos(instanceType);
                    }
                    sqlBuilder.Append("DELETE FROM `" + tableName + "`");
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        object? value = propertyInfo.GetValue(obj);
                        if (propertyInfo.Name.Equals("Id") && (value is int))
                        {
                            sqlBuilder.Append(" WHERE Id=@Id_" + i + ";");
                            dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name + "_" + i, value));
                        }
                    }
                }
                Log.Write(sqlBuilder.ToString());
            }
            else
            {
                //非泛型类
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

                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                sqlBuilder.Append("DELETE FROM `" + tableName + "`");
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    object? value = propertyInfo.GetValue(entity);
                    if (propertyInfo.Name.Equals("Id") && (value is int))
                    {
                        sqlBuilder.Append(" WHERE Id=@Id;");
                        dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name, value));
                    }
                }
                //MySqlCommand cmd = new MySqlCommand(sqlBuilder.ToString(), (MySqlConnection)dbConnection);
                Log.Write(sqlBuilder.ToString());
                //cmd.Parameters.AddRange(dbDataParameters.ToArray());
                //cmd.ExecuteNonQuery();
            }
        }

        public void Update<T>(T entity)
        {
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                //泛型集合
                if (type.GenericTypeArguments.Length <= 0)
                {
                    return;
                }
                var count = Convert.ToInt32(type.GetProperty("Count")?.GetValue(entity));
                var itemProperty = type.GetProperty("Item");//索引器属性
                string tableName = string.Empty;
                PropertyInfo[] propertyInfos = null;
                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                for (var i = 0; i < count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i });
                    if (i == 0)
                    {
                        Type instanceType = obj.GetType();
                        tableName = instanceType.Name;
                        var attributes = instanceType.GetCustomAttributes(typeof(TableAttribute), false);
                        if (attributes.Length > 0)
                        {
                            TableAttribute tableAttribute = (TableAttribute)attributes[0];
                            if (!string.IsNullOrEmpty(tableAttribute.TableName))
                            {
                                tableName = tableAttribute.TableName;
                            }
                        }
                        propertyInfos = ReflectionHelper.GetPropertyInfos(instanceType);
                    }
                    sqlBuilder.Append("UPDATE `" + tableName + "` SET");
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        object? value = propertyInfo.GetValue(obj);
                        if (propertyInfo.Name.Equals("Id") && (value is int))
                        {
                            sqlBuilder.Append(" WHERE Id=@Id_" + i + ";");
                            dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name + "_" + i, value));
                        }
                        else
                        {
                            if (value != null)
                            {
                                sqlBuilder.Append(" " + propertyInfo.Name + "=@" + propertyInfo.Name + "_" + i);
                                dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name + "_" + i, value));
                            }
                        }
                    }
                }
                Log.Write(sqlBuilder.ToString());
            }
            else
            {
                //非泛型类
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

                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                sqlBuilder.Append("UPDATE `" + tableName + "` SET");
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    object? value = propertyInfo.GetValue(entity);
                    if (propertyInfo.Name.Equals("Id") && (value is int))
                    {
                        sqlBuilder.Append(" WHERE Id=@Id;");
                        dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name, value));
                    }
                    else
                    {
                        if (value != null)
                        {
                            sqlBuilder.Append(" " + propertyInfo.Name + "=@" + propertyInfo.Name);
                            dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name, value));
                        }
                    }
                }
                //MySqlCommand cmd = new MySqlCommand(sqlBuilder.ToString(), (MySqlConnection)dbConnection);
                Log.Write(sqlBuilder.ToString());
                //cmd.Parameters.AddRange(dbDataParameters.ToArray());
                //cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<T> Get<T>(T entity)
        {
            IList<T> lists = new List<T>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("参数entity不能是泛型集合");
            }
            //非泛型类
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

            StringBuilder sqlBuilder = new StringBuilder();
            List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
            sqlBuilder.Append("SELECT " + string.Join(",", propertyInfos.Select(p => "`" + p.Name + "`")) + " FROM `" + tableName + "` WHERE 1=1");
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object? value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    sqlBuilder.Append(" and " + propertyInfo.Name + "=@" + propertyInfo.Name);
                    dbDataParameters.Add(new MySqlParameter("@" + propertyInfo.Name, value));
                }
            }
            //MySqlCommand cmd = new MySqlCommand(sqlBuilder.ToString(), (MySqlConnection)dbConnection);
            Log.Write(sqlBuilder.ToString());
            //cmd.Parameters.AddRange(dbDataParameters.ToArray());
            //MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
            //while (mySqlDataReader.Read())
            //{
            //    var obj = (T)Activator.CreateInstance(typeof(T));
            //    for (int i = 0; i < mySqlDataReader.FieldCount; i++)
            //    {
            //        var value = mySqlDataReader.GetValue(i);
            //        var columnType = mySqlDataReader[i].GetType();
            //        PropertyInfo propertyInfo = obj.GetType().GetProperty(columnType.Name);
            //        if (propertyInfo != null)
            //        {
            //            propertyInfo.SetValue(obj, value);
            //        }
            //    }
            //    lists.Add(obj);
            //}
            //mySqlDataReader.Close();
            return lists;
        }

        public void Get<T>(T entity, out PageModel pageModel)
        {
            PageModel pageModel1 = new PageModel();
            pageModel = pageModel1;
        }

        public void Get<T1, T2>(T1 entity1, T2 entity2)
        {

        }

        public void Get<T1, T2>(T1 entity1, T2 entity2, out PageModel pageModel)
        {
            pageModel = new PageModel();
        }

        public object Get(string sql)
        {
            return 1;
        }

        public IEnumerable<T> Get<T>(string sql)
        {
            return (IEnumerable<T>)Activator.CreateInstance(typeof(T));
        }

        public int ExecuteSql(string sql)
        {
            return 0;
        }

    }
}
