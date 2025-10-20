using Rokys.Audit.Model.Tables;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class StorageFilesRepository : EFRepository<StorageFiles>, IStorageFilesRepository
    {
        public StorageFilesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
