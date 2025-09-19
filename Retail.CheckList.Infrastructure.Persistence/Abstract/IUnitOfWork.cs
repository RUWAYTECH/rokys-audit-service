using System;
using System.Threading;
using System.Threading.Tasks;

namespace Retail.CheckList.Infrastructure.Persistence.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
