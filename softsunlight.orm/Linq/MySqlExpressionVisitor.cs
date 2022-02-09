using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace softsunlight.orm.Linq
{
    public class MySqlExpressionVisitor : ExpressionVisitor
    {
        public StringBuilder SqlBuilder { private set; get; }

        public MySqlExpressionVisitor()
        {
            SqlBuilder = new StringBuilder();
            SqlBuilder.Append("select * from");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            this.Visit(node.Left);
            if (node.NodeType == ExpressionType.AndAlso)
            {
                SqlBuilder.Append(" and");
            }
            else if (node.NodeType == ExpressionType.Equal)
            {
                SqlBuilder.Append(" =");
            }
            this.Visit(node.Right);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                SqlBuilder.Append(" `" + node.Type.GenericTypeArguments[0].Name + "`");
                SqlBuilder.Append(" where ");
                this.Visit(node.Arguments[1]);
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            SqlBuilder.Append(node.Value);
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.Visit(node.Body);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            SqlBuilder.Append(node.Member.Name);
            return node;
        }

    }
}
