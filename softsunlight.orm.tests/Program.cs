using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using System;
using System.Collections.Generic;

namespace softsunlight.orm.tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //自定义orm框架测试

            //API
            var db = new SoftSunlightSqlClient(DbTypeEnum.MySql, "Server=127.0.0.1;Database=test;uid=root;pwd=123456;Charset=utf8;SslMode = none;");
            //var db = new SoftSunlightSqlClient(DbTypeEnum.SqlServer, "Data Source=.;Initial Catalog=testDB;Integrated Security=SSPI;");
            //string sql = ConvertToSql.GetCreateTableSql<Person>(DbTypeEnum.SqlServer);
            //db.ExecuteNoQuery(sql);
        }
    }

    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Course
    {

    }

}
