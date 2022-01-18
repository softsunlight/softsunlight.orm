using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using softsunlight.orm.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace softsunlight.orm.tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //自定义orm框架测试

            //API
            var db = new SoftSunlightSqlClient(DbTypeEnum.MySql, "Server=127.0.0.1;Database=test;uid=root;pwd=123456;Charset=utf8;SslMode=none;");
            //var db = new SoftSunlightSqlClient(DbTypeEnum.SqlServer, "Data Source=.;Initial Catalog=testDB;Integrated Security=SSPI;");
            //for (int i = 0; i < 10; i++)
            //{
            //    Task.Run(() =>
            //    {
            //        Log.Write(ReflectionHelper.GetPropertyInfo(typeof(Person), "Age").ToString());
            //        //db.Get(new Person() { Id = 1 });
            //    });
            //}
            //Console.Read();
            //var db = new SoftSunlightSqlClient(DbTypeEnum.SqlServer, "Data Source=.;Initial Catalog=testDB;Integrated Security=SSPI;");
            //string sql = ConvertToSql.GetCreateTableSql<Person>(DbTypeEnum.MySql);
            //db.ExecuteNoQuery(sql);
            db.Add(new Person() { Name = "wyb", Age = 26 });
            //db.Add(new List<Person>() { new Person() { Name = "wyb", Age = 26 } });
            //List<Person> personList = new List<Person>();
            //for (int i = 0; i < 500000; i++)
            //{
            //    personList.Add(new Person() { Name = "wyb_" + i, Age = 16 + i });
            //}

            //db.Add(personList);
            //db.Delete(new Person());
            //db.Delete(personList);
            //db.Update(new Person());
            //db.Update(new List<Person>());
            ////单表查询
            //db.Get(new Person() { Id = 1 });
            //PageModel pageModel = new PageModel();
            //db.Get(new Person(), pageModel);
            ////连接查询
            //db.Get(new Person(), new Course());
            //db.Get(new Person(), new Course(), pageModel);
            ////执行自定义sql
            //object objResult = db.Get("");
            //IEnumerable<Person> lists = db.Get<Person>("");
            int result = db.ExecuteNoQuery("select count(0) from person where id={0}", 1);//执行增加、删除、更新语句
        }
    }

    class Person
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? CreateTime { get; set; }
    }

    class Course
    {

    }

}
