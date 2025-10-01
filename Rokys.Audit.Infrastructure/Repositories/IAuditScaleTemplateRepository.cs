using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IAuditScaleTemplateRepository : IRepository<AuditScaleTemplate>
    {
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    }
}