using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 将DataSet、DataTable、DataReader等转换为强类型实体
    /// </summary>
    public class ConvertToEntity
    {
        public static T Convert<T>(DataTable dt)
        {
            if (dt.Rows == null || dt.Rows.Count <= 0)
            {
                return default(T);
            }
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("泛型参数T不能是泛型集合");
            }
            var obj = (T)Activator.CreateInstance(typeof(T));
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var value = dt.Rows[0][i];
                PropertyInfo pi = ReflectionHelper.GetPropertyInfo(type, dt.Columns[i].ColumnName);
                if (pi != null)
                {
                    pi.SetValue(obj, value);
                }
            }
            return obj;
        }

        public static T Convert<T>(DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("泛型参数T不能是泛型集合");
            }
            var obj = (T)Activator.CreateInstance(typeof(T));
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                var value = row[i];
                PropertyInfo pi = ReflectionHelper.GetPropertyInfo(type, row.Table.Columns[i].ColumnName);
                if (pi != null)
                {
                    pi.SetValue(obj, value);
                }
            }
            return obj;
        }

        public static IList<T> ConvertToList<T>(DataTable dt)
        {
            if (dt.Rows == null || dt.Rows.Count <= 0)
            {
                return default(IList<T>);
            }
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("泛型参数T不能是泛型集合");
            }
            IList<T> lists = new List<T>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    lists.Add(Convert<T>(row));
                }
            }
            return lists;
        }

        public static IList<T> ConvertToList<T>(DbDataReader dbDataReader)
        {
            if (dbDataReader.HasRows)
            {
                return default(IList<T>);
            }
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                throw new Exception("泛型参数T不能是泛型集合");
            }
            IList<T> lists = new List<T>();
            while (dbDataReader.Read())
            {
                var obj = (T)Activator.CreateInstance(typeof(T));
                for (int i = 0; i < dbDataReader.FieldCount; i++)
                {
                    var value = dbDataReader.GetValue(i);
                    var columnType = dbDataReader[i].GetType();
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(columnType.Name);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(obj, value);
                    }
                }
                lists.Add(obj);
            }
            return lists;
        }

    }
}
