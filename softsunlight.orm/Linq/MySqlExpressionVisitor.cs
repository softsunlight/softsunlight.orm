using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace softsunlight.orm.Linq
{
    public class MySqlExpressionVisitor : ExpressionVisitor
    {
        private StringBuilder sqlBuilder;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

    }
}
