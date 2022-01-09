using MySql.Data.MySqlClient;
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
using System.Text.RegularExpressions;

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
                var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Count")?.GetValue(entity));
                var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                int eachCount = 500;//每次处理的数据条数
                List<object> list = new List<object>();
                for (var i = 1; i <= count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i - 1 });
                    list.Add(obj);
                    if (i % eachCount == 0 || i >= count)
                    {
                        string sql = ConvertToSql.GetInsertSql(dbTypeEnum, list, out dbDataParameters);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                        }
                        dbDataParameters = new List<IDbDataParameter>();
                        list = new List<object>();
                    }
                }
            }
            else
            {
                //非泛型类
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetInsertSql<T>(dbTypeEnum, entity, out dbDataParameters);
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
                var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Count")?.GetValue(entity));
                var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
                StringBuilder sqlBuilder = new StringBuilder();
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                int eachCount = 500;//每次处理的数据条数
                List<object> list = new List<object>();
                for (var i = 1; i <= count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i - 1 });
                    list.Add(obj);
                    if (i % eachCount == 0 || i >= count)
                    {
                        string sql = ConvertToSql.GetDeleteSql(dbTypeEnum, list, out dbDataParameters);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                        }
                        dbDataParameters = new List<IDbDataParameter>();
                        list = new List<object>();
                    }
                }
            }
            else
            {
                //非泛型类
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetDeleteSql<T>(dbTypeEnum, entity, out dbDataParameters);
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
                var count = Convert.ToInt32(ReflectionHelper.GetPropertyInfo(type, "Count")?.GetValue(entity));
                var itemProperty = ReflectionHelper.GetPropertyInfo(type, "Item");//索引器属性
                StringBuilder sqlBuilder = new StringBuilder();
                List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                int eachCount = 500;//每次处理的数据条数
                List<object> list = new List<object>();
                for (var i = 0; i < count; i++)
                {
                    var obj = itemProperty.GetValue(entity, new object[] { i });
                    list.Add(obj);
                    if (i % eachCount == 0 || i >= count)
                    {
                        IList<IDbDataParameter> tempDbDataParameters = new List<IDbDataParameter>();
                        sqlBuilder.Append(ConvertToSql.GetUpdateSql(dbTypeEnum, obj, out tempDbDataParameters));
                        dbDataParameters.AddRange(tempDbDataParameters);
                        if (sqlBuilder.Length > 0)
                        {
                            sqlHelper.ExecuteNoQuery(sqlBuilder.ToString(), dbDataParameters);
                        }
                        dbDataParameters = new List<IDbDataParameter>();
                        list = new List<object>();
                    }
                }
            }
            else
            {
                //非泛型类
                IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                string sql = ConvertToSql.GetUpdateSql<T>(dbTypeEnum, entity, out dbDataParameters);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqlHelper.ExecuteNoQuery(sql, dbDataParameters);
                }
            }
        }

        public IList<T> Get<T>(T entity)
        {
            return Get(entity, null);
        }

        public IList<T> Get<T>(T entity, PageModel pageModel)
        {
            IList<T> lists = new List<T>();
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("参数entity不能是泛型集合");
            }
            //非泛型类
            IList<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
            string sql = ConvertToSql.GetSelectSql<T>(dbTypeEnum, entity, out dbDataParameters, pageModel);
            if (!string.IsNullOrEmpty(sql))
            {
                DataTable dt = sqlHelper.GetDataTable(sql, dbDataParameters);
                lists = ConvertToEntity.ConvertToList<T>(dt);
            }
            return lists;
        }

        //public IList<T3> Get<T1, T2, T3>(T1 entity1, T2 entity2)
        //{
        //    return Get<T3>(entity1, entity2, null);
        //}

        //public IList<T3> Get<T1, T2, T3>(T1 entity1, T2 entity2, PageModel pageModel)
        //{
        //    throw new NotImplementedException();
        //}

        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">eg:select * from user where id={0} and name like {1}</param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params object[] sqlParams)
        {
            IList<IDbDataParameter> parameters = new List<IDbDataParameter>();
            string execSql = BuildSql(sql, out parameters, sqlParams);
            return sqlHelper.GetScalar(execSql, parameters);
        }

        public IList<T> Get<T>(string sql)
        {
            return Get<T>(sql, null);
        }

        public IList<T> Get<T>(string sql, params object[] sqlParams)
        {
            IList<IDbDataParameter> parameters = new List<IDbDataParameter>();
            string execSql = BuildSql(sql, out parameters, sqlParams);
            return ConvertToEntity.ConvertToList<T>(sqlHelper.GetDataReader(execSql, parameters));
        }

        public int ExecuteNoQuery(string sql)
        {
            return ExecuteNoQuery(sql, null);
        }

        public int ExecuteNoQuery(string sql, params object[] sqlParams)
        {
            IList<IDbDataParameter> parameters = new List<IDbDataParameter>();
            string execSql = BuildSql(sql, out parameters, sqlParams);
            return sqlHelper.ExecuteNoQuery(execSql, parameters);
        }

        private string BuildSql(string sql, out IList<IDbDataParameter> parameters, params object[] sqlParams)
        {
            parameters = new List<IDbDataParameter>();
            if (sqlParams != null)
            {
                for (var i = 0; i < sqlParams.Length; i++)
                {
                    string paramName = sqlParams[i].ToString();
                    sql = Regex.Replace(sql, @"{\s*" + i + @"\s*}", "@" + paramName);
                    parameters.Add(SqlUtils.GetDbDataParameter(dbTypeEnum, paramName, sqlParams));
                }
            }
            return sql;
        }

    }
}
