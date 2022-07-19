using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;

namespace ef.demo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDb myDb = new MyDb();
            var reuslt = myDb.Persons.Where(p => 1 == 1).Skip(1).Take(2).Skip(1);
            foreach (var person in reuslt)
            {
                Console.WriteLine(person.Id);
            }
        }
    }

    class MyDb : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=.;Initial Catalog=testDB;Integrated Security=SSPI;");
            base.OnConfiguring(optionsBuilder);
        }
    }

    [Table("Person")]
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
