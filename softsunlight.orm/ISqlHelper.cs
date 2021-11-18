using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlHelper
    {

        DataTable GetDataTable(string sql);

        DataTable GetDataTable(string sql,IList<IDbDataParameter> dbDataParameters);

        DataSet GetDataSet(string sql);

        DataSet GetDataSet(string sql,IList<IDbDataParameter> dbDataParameters);

        //IDbDataReader GetDataReader(string sql);

        //IDbDataReader GetDataReader(string sql,IList<IDbDataParameter> dbDataParameters);

        object? GetScalar(string sql);

        object? GetScalar(string sql,IList<IDbDataParameter> dbDataParameters);

    }
}
