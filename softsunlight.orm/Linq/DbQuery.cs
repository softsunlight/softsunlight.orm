using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using softsunlight.orm.Enum;

namespace softsunlight.orm.Linq
{
    public class DbQuery<T> : IQueryable<T>, IQueryable
    {

        private SqlHelper sqlHelper;

        private Expression expression;

        public DbQuery(SqlHelper sqlHelper)
        {
            this.sqlHelper = sqlHelper;
            this.expression = Expression.Constant(this);
        }

        public DbQuery(SqlHelper sqlHelper, Expression expression)
        {
            this.sqlHelper = sqlHelper;
            this.expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => this.expression;

        public IQueryProvider Provider => SqlUtils.GetQueryProvider(this.sqlHelper);

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Provider.Execute(this.expression)).GetEnumerator();
        }
    }
}
