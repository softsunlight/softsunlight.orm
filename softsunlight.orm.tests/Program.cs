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
            var db = new SoftSunlightSqlClient("Data Source=127.0.0.1,");
            ConvertToSql.GetCreateTableSql<Person>(DbTypeEnum.MySql);
            //db.Add(new Person() { Name = "wyb", Age = 26 });
            //db.Add(new List<Person>() { new Person() { Name = "wyb", Age = 26 } });
            List<Person> personList = new List<Person>();
            for (int i = 0; i < 10; i++)
            {
                personList.Add(new Person() { Name = "wyb_" + i, Age = 16 + i });
            }
            db.Add(personList);
            db.Delete(new Person());
            db.Delete(new List<Person>());
            db.Update(new Person());
            db.Update(new List<Person>());
            //单表查询
            db.Get(new Person());
            PageModel pageModel = new PageModel();
            db.Get(new Person(), out pageModel);
            //连接查询
            db.Get(new Person(), new Course());
            db.Get(new Person(), new Course(), out pageModel);
            //执行自定义sql
            object objResult = db.Get("");
            IEnumerable<Person> lists = db.Get<Person>("");
            int result = db.ExecuteNoQuery("");//执行增加、删除、更新语句
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
