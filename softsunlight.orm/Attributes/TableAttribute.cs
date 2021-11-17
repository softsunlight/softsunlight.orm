using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm.Attributes
{
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {

        }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }

    }
}
