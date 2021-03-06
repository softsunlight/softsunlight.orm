using softsunlight.orm.Attributes;
using softsunlight.orm.Enum;
using softsunlight.orm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace softsunlight.orm.tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //自定义orm框架测试

            //API
            //var db = new SoftSunlightSqlClient(DbTypeEnum.MySql, "Server=127.0.0.1;Database=test;uid=root;pwd=123456;Charset=utf8;SslMode=none;");

            var db = new SoftSunlightSqlClient(DbTypeEnum.SqlServer, "Data Source=.;Initial Catalog=testDB;Integrated Security=SSPI;");
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
            //db.Add(new Person() { Name = "wyb", Age = 26 });
            //db.Add(new List<Person>() { new Person() { Name = "wyb", Age = 26 } });
            //List<Person> personList = new List<Person>();
            //for (int i = 0; i < 500000; i++)
            //{
            //    personList.Add(new Person() { Name = "wyb_" + i, Age = 16 + i });
            //}

            //db.Add(personList);

            //var queryResult = db.Query<Person>().Where(p => p.Id == 1);
            //foreach (var item in queryResult)
            //{
            //    Console.WriteLine(item.Name);
            //}
            //var queryResult = db.Query<Person>().Count(p => p.Age == 5);
            //var queryResult = db.Query<Person>().Count();
            //Console.WriteLine(queryResult);
            //string str = "aa";
            //var queryResult = db.Query<Person>().All(p => p.Name.Contains("aa"));
            var str = "连衣裙";
            //var queryResult = db.Query<MoguItem>().Where(p => p.Title.Contains("连衣裙"));
            var queryResult = db.Query<Person>().Where(p => p.Name.Contains("连衣裙")).Where(p => p.Age == 1);
            //Console.WriteLine(queryResult);
            foreach (var item in queryResult)
            {
                //Console.WriteLine(item.Title);
            }

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
            //int result = db.ExecuteNoQuery("select count(0) from person where id={0}", 1);//执行增加、删除、更新语句

            Console.ReadLine();
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

    /// <summary>
    /// 蘑菇街商品
    /// </summary>
    class MoguItem
    {
        public int? Id { get; set; }
        public string ShopId { get; set; }
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? LowNowPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? HighNowPrice { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }

}
