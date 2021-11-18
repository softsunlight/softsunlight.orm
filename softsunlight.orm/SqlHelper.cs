using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using softsunlight.orm.Enum;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
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

        public SqlHelper(DbTypeEnum dbTypeEnum)
        {
            this.dbTypeEnum = dbTypeEnum;
            this.dbConnection = SqlUtils.GetDbConnection(dbTypeEnum);
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
            dbConnection.Open();
            DbCommand mySqlCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
            if (dbDataParameters != null && dbDataParameters.Count > 0)
            {
                mySqlCommand.Parameters.AddRange(dbDataParameters.ToArray());
            }
            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            dbConnection.Close();
            return dataReader;
        }

        public DataSet GetDataSet(string sql)
        {
            return GetDataSet(sql, null);
        }

        public DataSet GetDataSet(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            DataSet dataSet = new DataSet();
            DbDataAdapter mySqlDataAdapter = SqlUtils.GetDbDataAdapter(this.dbTypeEnum);
            mySqlDataAdapter.SelectCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
            mySqlDataAdapter.SelectCommand.CommandText = sql;
            mySqlDataAdapter.SelectCommand.Connection = (DbConnection)this.dbConnection;
            if (dbDataParameters != null && dbDataParameters.Count > 0)
            {
                mySqlDataAdapter.SelectCommand.Parameters.AddRange(dbDataParameters.ToArray());
            }
            mySqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, null);
        }

        public DataTable GetDataTable(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            DataTable dt = null;
            DbDataAdapter mySqlDataAdapter = SqlUtils.GetDbDataAdapter(this.dbTypeEnum);
            mySqlDataAdapter.SelectCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
            mySqlDataAdapter.SelectCommand.CommandText = sql;
            mySqlDataAdapter.SelectCommand.Connection = (DbConnection)this.dbConnection;
            if (dbDataParameters != null && dbDataParameters.Count > 0)
            {
                mySqlDataAdapter.SelectCommand.Parameters.AddRange(dbDataParameters.ToArray());
            }
            DataSet dataSet = new DataSet();
            mySqlDataAdapter.Fill(dataSet);
            dt = dataSet.Tables[0];
            return dt;
        }

        public object GetScalar(string sql)
        {
            return GetScalar(sql, null);
        }

        public object GetScalar(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            dbConnection.Open();
            DbCommand mySqlCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
            mySqlCommand.CommandText = sql;
            if (dbDataParameters != null && dbDataParameters.Count > 0)
            {
                mySqlCommand.Parameters.AddRange(dbDataParameters.ToArray());
            }
            object obj = mySqlCommand.ExecuteScalar();
            dbConnection.Close();
            return obj;
        }

        public int ExecuteNoQuery(string sql)
        {
            return ExecuteNoQuery(sql, null);
        }

        public int ExecuteNoQuery(string sql, IList<IDbDataParameter> dbDataParameters)
        {
            dbConnection.Open();
            DbCommand mySqlCommand = SqlUtils.GetDbCommand(this.dbTypeEnum);
            mySqlCommand.CommandText = sql;
            if (dbDataParameters != null && dbDataParameters.Count > 0)
            {
                mySqlCommand.Parameters.AddRange(dbDataParameters.ToArray());
            }
            int result = mySqlCommand.ExecuteNonQuery();
            dbConnection.Close();
            return result;
        }

    }
}
