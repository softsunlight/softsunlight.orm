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
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SqlBuilder.Append(" (");
            this.Visit(node.Left);
            if (node.NodeType == ExpressionType.AndAlso)
            {
                SqlBuilder.Append(" and");
            }
            else if (node.NodeType == ExpressionType.Equal)
            {
                SqlBuilder.Append(" =");
            }
            else if (node.NodeType == ExpressionType.NotEqual)
            {
                SqlBuilder.Append(" <>");
            }
            else if (node.NodeType == ExpressionType.GreaterThan)
            {
                SqlBuilder.Append(" >");
            }
            else if (node.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                SqlBuilder.Append(" >=");
            }
            else if (node.NodeType == ExpressionType.LessThan)
            {
                SqlBuilder.Append(" <");
            }
            else if (node.NodeType == ExpressionType.LessThanOrEqual)
            {
                SqlBuilder.Append(" <=");
            }
            else if (node.NodeType == ExpressionType.OrElse)
            {
                SqlBuilder.Append(" or");
            }
            this.Visit(node.Right);
            SqlBuilder.Append(" )");
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                SqlBuilder.Append("select * from ");
                SqlBuilder.Append(node.Arguments[0].Type.GenericTypeArguments[0].Name);
                SqlBuilder.Append(" where ");
                this.Visit(node.Arguments[1]);
            }
            else if (node.Method.Name == "Count")
            {
                SqlBuilder.Append("select count(0) from ");
                SqlBuilder.Append(node.Arguments[0].Type.GenericTypeArguments[0].Name);
                if (node.Arguments.Count > 1)
                {
                    SqlBuilder.Append(" where ");
                    this.Visit(node.Arguments[1]);
                }
            }
            else if (node.Method.Name == "Contains")
            {
                if (!SqlBuilder.ToString().Contains("where"))
                {
                    SqlBuilder.Append(" where ");
                }
                this.Visit(node.Object);
                if (node.Arguments[0].NodeType == ExpressionType.Constant)
                {
                    SqlBuilder.Append(" like '%");
                    this.Visit(node.Arguments[0]);
                    SqlBuilder.Append("%'");
                }
                else if (node.Arguments[0].NodeType == ExpressionType.MemberAccess)
                {
                    SqlBuilder.Append(" like '%");
                    this.Visit(node.Arguments[0]);
                    SqlBuilder.Append("%'");
                }
            }
            else if (node.Method.Name == "All")
            {
                SqlBuilder.Append("select ( (select count(0) from ");
                SqlBuilder.Append(node.Arguments[0].Type.GenericTypeArguments[0].Name);
                if (!SqlBuilder.ToString().Contains("where"))
                {
                    SqlBuilder.Append(" where ");
                }
                if (node.Arguments.Count > 1)
                {
                    this.Visit(node.Arguments[1]);
                }
                SqlBuilder.Append(" ) = (select count(0) from " + node.Arguments[0].Type.GenericTypeArguments[0].Name + ") )");
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var fields = node.GetType().GetFields();
            object val = null;
            if (fields.Length > 0)
            {
                val = fields[0].GetValue(node.Value);
            }
            else
            {
                val = node.Value;
            }
            if (val is string)
            {
                if (SqlBuilder[SqlBuilder.Length - 1] == '%')
                {
                    SqlBuilder.Append(val);
                }
                else
                {
                    SqlBuilder.Append("'").Append(val).Append("'");
                }
            }
            else
            {
                SqlBuilder.Append(val);
            }
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.Visit(node.Body);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.NodeType == ExpressionType.Constant)
            {
                this.Visit(node.Expression);
            }
            else
            {
                SqlBuilder.Append(node.Member.Name);
            }
            return node;
        }

        /// <summary>
        /// 一元运算符
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            this.Visit(node.Operand);
            return node;
        }

    }
}
