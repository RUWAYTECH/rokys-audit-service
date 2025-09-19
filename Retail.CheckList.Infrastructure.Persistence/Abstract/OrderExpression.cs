using System;
using System.Linq.Expressions;

namespace Retail.CheckList.Infrastructure.Persistence.Abstract
{
    public class OrderExpression<T>
    {
        public OrderExpression() { }
        public OrderExpression(OrderType type, Expression<Func<T, object>> expression)
        {
            Type = type;
            Expression = expression;
        }

        public OrderType Type { get; set; }
        public Expression<Func<T, object>> Expression { get; set; }

        public static OrderExpression<T> Asc(Expression<Func<T, object>> expression)
        {
            return new OrderExpression<T>(OrderType.Asc, expression);
        }
        public static OrderExpression<T> Desc(Expression<Func<T, object>> expression)
        {
            return new OrderExpression<T>(OrderType.Desc, expression);
        }
    }

    public enum OrderType : byte
    {
        Asc = 1,
        Desc = 2
    }
}
