using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rokys.Audit.Infrastructure.Persistence.Abstract
{
    public interface IQueryDataAccess<T> where T : class
    {
        Task<T> FirstAsync(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, string[] includes = null);

        Task<TOut> FirstAsync<TOut>(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, string[] includes = null, Expression<Func<T, TOut>> projectionExpression = null);

        Task<IEnumerable<T>> ListAllAsync(OrderExpression<T>[] orderExpression = null, string[] includes = null);

        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, string[] includes = null);

        Task<IEnumerable<T>> ListIncludeAsync(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, List<Expression<Func<T, object>>> includes = null);

        Task<IEnumerable<TOut>> ListAsync<TOut>(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, string[] includes = null, Expression<Func<T, TOut>> projectionExpression = null);


        Task<long> CountAsync(Expression<Func<T, bool>> whereExpression = null);

        Task<bool> AnyAsync(Expression<Func<T, bool>> whereExpression);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, string[] includes = null);

        Task<T> FirstOrDefaultIncludesAsync(Expression<Func<T, bool>> whereExpression, OrderExpression<T>[] orderExpression = null, List<Expression<Func<T, object>>> includes = null);

    }
}
