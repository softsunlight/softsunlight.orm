using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using softsunlight.orm.Enum;

namespace softsunlight.orm.Linq
{
    public class MySqlQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DbQuery<TElement>(DbTypeEnum.MySql, expression);
        }

        public object Execute(Expression expression)
        {

            return null;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }
    }
}
