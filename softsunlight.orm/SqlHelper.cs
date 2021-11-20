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
        /// 数据库连接类
        /// </summary>
        private IDbConnection dbConnection;

        /// <summary>
        /// 数据库类型
        /// </summary>
        private DbTypeEnum dbTypeEnum;

        /// <summary>
        /// 
        /// </summary>
        private Stopwatch stopwatch;

        public SqlHelper(DbTypeEnum dbTypeEnum)
        {
            this.dbTypeEnum = dbTypeEnum;
            this.dbConnection = SqlUtils.GetDbConnection(dbTypeEnum);
            this.stopwatch = new Stopwatch();
        }

        public void SetConnectionStr(string connectionStr)
        {
            this.dbConnection.ConnectionString = connectionStr;
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
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.Connection = (DbConnection)this.dbConnection;
                if (dbDataParameters != null && dbDataParameters.Count > 0)
                {
                    dbCommand.Parameters.AddRange(dbDataParameters.ToArray());
                }
                stopwatch.Restart();
                dataReader = dbCommand.ExecuteReader();
                stopwatch.Stop();
                Log.Write("执行Sql：" + sql + "耗时:" + stopwatch.Elapsed);
                dbConnection.Close();
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
                dbDataAdapter.SelectCommand.Connection = (DbConnection)this.dbConnection;
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
                dbDataAdapter.SelectCommand.Connection = (DbConnection)this.dbConnection;
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
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.CommandText = sql;
                dbCommand.Connection = (DbConnection)this.dbConnection;
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
                dbConnection.Open();
                DbCommand dbCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
                dbCommand.CommandText = sql;
                dbCommand.Connection = (DbConnection)this.dbConnection;
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
