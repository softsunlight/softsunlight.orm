﻿using MySql.Data.MySqlClient;
using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using softsunlight.orm.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        private SqlHelper sqlHelper;

        /// <summary>
        /// 数据库类型
        /// </summary>
        private DbTypeEnum dbTypeEnum;

        /// <summary>
        /// 默认使用MySql数据库
        /// </summary>
        public SoftSunlightSqlClient(string connectionStr) : this(DbTypeEnum.MySql, connectionStr)
        {

        }

        public SoftSunlightSqlClient(DbTypeEnum dbTypeEnum, string connectionStr)
        {
            this.dbTypeEnum = dbTypeEnum;
            sqlHelper = new SqlHelper(dbTypeEnum);
            sqlHelper.SetConnectionStr(connectionStr);
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
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetInsertSql<T>(dbTypeEnum, entity, out dbDataParameters);
                Log.Write(sql);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                }
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
                var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Item")?.GetValue(entity));
                var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
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
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetDeleteSql<T>(dbTypeEnum, entity, out dbDataParameters);
                Log.Write(sql);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                }
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
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetUpdateSql<T>(dbTypeEnum, entity, out dbDataParameters);
                Log.Write(sql);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                }
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
            IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
            string sql = ConvertToSql.GetSelectSql<T>(dbTypeEnum, entity, out dbDataParameters);
            Log.Write(sql);
            if (!string.IsNullOrEmpty(sql))
            {
                DbDataReader dataReader = sqlHelper.GetDataReader(sql, dbDataParameters);
                lists = ConvertToEntity.ConvertToList<T>(dataReader).ToList();
            }
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

        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        public object ExecuteScalar(string sql, params object[] sqlParams)
        {
            return 1;
        }

        public IEnumerable<T> Get<T>(string sql)
        {
            return Get<T>(sql, null);
        }

        public IEnumerable<T> Get<T>(string sql, params object[] sqlParams)
        {
            return (IEnumerable<T>)Activator.CreateInstance(typeof(T));
        }

        public int ExecuteNoQuery(string sql)
        {
            return ExecuteNoQuery(sql, null);
        }

        public int ExecuteNoQuery(string sql, params object[] sqlParams)
        {
            return 0;
        }

    }
}
