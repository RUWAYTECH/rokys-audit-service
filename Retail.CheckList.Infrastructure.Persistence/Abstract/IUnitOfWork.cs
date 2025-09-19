using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rokys.Audit.Infrastructure.Persistence.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
