using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class InboxItemsRepository : EFRepository<InboxItems>, IInboxItemsRepository
    {
        public InboxItemsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
