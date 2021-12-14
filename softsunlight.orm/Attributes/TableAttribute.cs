using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm.Attributes
{
    /// <summary>
    /// 为实体指定表名
    /// </summary>
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {

        }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }

    }
}
