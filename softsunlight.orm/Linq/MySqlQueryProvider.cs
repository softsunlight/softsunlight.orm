using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Data;

namespace softsunlight.orm.Linq
{
    public class MySqlQueryProvider : IQueryProvider
    {
        private SqlHelper sqlHelper;

        public MySqlQueryProvider(SqlHelper sqlHelper)
        {
            this.sqlHelper = sqlHelper;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DbQuery<TElement>(this.sqlHelper, expression);
        }

        public object Execute(Expression expression)
        {
            //返回可枚举的集合
            MySqlExpressionVisitor mySqlExpressionVisitor = new MySqlExpressionVisitor();
            mySqlExpressionVisitor.Visit(expression);
            string sql = mySqlExpressionVisitor.SqlBuilder.ToString();
            if (expression.Type == typeof(int))
            {
                return Convert.ToInt32(this.sqlHelper.GetScalar(sql));
            }
            else if (expression.Type == typeof(bool))
            {
                return Convert.ToInt32(this.sqlHelper.GetScalar(sql)) > 0;
            }
            else
            {
                return Activator.CreateInstance(typeof(ObjectReader<>).MakeGenericType(expression.Type.GenericTypeArguments[0]), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new object[] { this.sqlHelper.GetDataTable(sql) }, null);
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }

    }

    internal class ObjectReader<T> : IEnumerable<T>, IEnumerable where T : class, new()
    {
        Enumerator enumerator;

        internal ObjectReader(DataTable dt)
        {
            this.enumerator = new Enumerator(dt);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Enumerator e = this.enumerator;
            if (e == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }
            this.enumerator = null;
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        class Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private DataTable dt;

            private T current;

            private int index = -1;

            internal Enumerator(DataTable dt)
            {
                this.dt = dt;
            }

            public T Current
            {
                get { return this.current; }
            }

            object IEnumerator.Current
            {
                get { return this.current; }
            }

            public bool MoveNext()
            {
                index++;
                if (index < this.dt.Rows.Count)
                {
                    T instance = ConvertToEntity.Convert<T>(dt.Rows[index]);
                    this.current = instance;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                this.index = -1;
            }

            public void Dispose()
            {
                this.dt.Dispose();
            }
        }
    }

}
