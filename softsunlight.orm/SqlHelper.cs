using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using softsunlight.orm.Enum;
using softsunlight.orm.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace softsunlight.orm
{
    public class SqlHelper
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        private DbTypeEnum dbTypeEnum;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connectionStr;

        /// <summary>
        /// 计时信息
        /// </summary>
        private Stopwatch stopwatch;

        public SqlHelper(DbTypeEnum dbTypeEnum)
        {
            this.dbTypeEnum = dbTypeEnum;
            this.stopwatch = new Stopwatch();
        }

        public void SetConnectionStr(string connectionStr)
        {
            this.connectionStr = connectionStr;
        }

        public IDataReader GetDataReader(string sql)
        {
            return GetDataReader(sql, null);
        }

        public DbDataReader GetDataReader(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            DbDataReader dataReader = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    return dataReader;
                }
                IDbConnection dbConnection = SqlUtils.GetDbConnection(this.dbTypeEnum);
                dbConnection.ConnectionString = this.connectionStr;
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.CommandText = sql;
                dbCommand.Connection = (DbConnection)dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                stopwatch.Restart();
                dataReader = dbCommand.ExecuteReader();
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ex);
                throw ex;
            }
            return dataReader;
        }

        public DataSet GetDataSet(string sql)
        {
            return GetDataSet(sql, null);
        }

        public DataSet GetDataSet(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            DataSet dataSet = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    return dataSet;
                }
                DbDataAdapter dbDataAdapter = SqlUtils.GetDbDataAdapter(this.dbTypeEnum);
                dbDataAdapter.SelectCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbDataAdapter.SelectCommand.CommandText = sql;
                IDbConnection dbConnection = SqlUtils.GetDbConnection(this.dbTypeEnum);
                dbConnection.ConnectionString = this.connectionStr;
                dbDataAdapter.SelectCommand.Connection = (DbConnection)dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbDataAdapter.SelectCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                stopwatch.Restart();
                dbDataAdapter.Fill(dataSet);
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ex);
                throw ex;
            }
            return dataSet;
        }

        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, null);
        }

        public DataTable GetDataTable(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            DataTable dt = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    return dt;
                }
                DbDataAdapter dbDataAdapter = SqlUtils.GetDbDataAdapter(this.dbTypeEnum);
                dbDataAdapter.SelectCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbDataAdapter.SelectCommand.CommandText = sql;
                IDbConnection dbConnection = SqlUtils.GetDbConnection(this.dbTypeEnum);
                dbConnection.ConnectionString = this.connectionStr;
                dbDataAdapter.SelectCommand.Connection = (DbConnection)dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbDataAdapter.SelectCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                DataSet dataSet = new DataSet();
                stopwatch.Restart();
                dbDataAdapter.Fill(dataSet);
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
                dt = dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ex);
                throw ex;
            }
            return dt;
        }

        public object GetScalar(string sql)
        {
            return GetScalar(sql, null);
        }

        public object GetScalar(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            object objResult = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    return objResult;
                }
                IDbConnection dbConnection = SqlUtils.GetDbConnection(this.dbTypeEnum);
                dbConnection.ConnectionString = this.connectionStr;
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.CommandText = sql;
                dbCommand.Connection = (DbConnection)dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                stopwatch.Restart();
                objResult = dbCommand.ExecuteScalar();
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ex);
                throw ex;
            }
            return objResult;
        }

        public int ExecuteNoQuery(string sql)
        {
            return ExecuteNoQuery(sql, null);
        }

        public int ExecuteNoQuery(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            int result = 0;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    return result;
                }
                IDbConnection dbConnection = SqlUtils.GetDbConnection(this.dbTypeEnum);
                dbConnection.ConnectionString = this.connectionStr;
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.CommandText = sql;
                dbCommand.Connection = (DbConnection)dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                stopwatch.Restart();
                result = dbCommand.ExecuteNonQuery();
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ex);
                throw ex;
            }
            return result;
        }

    }
}
