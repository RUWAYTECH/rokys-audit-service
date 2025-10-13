using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IAuditTemplateFieldRepository : IRepository<AuditTemplateFields>
    {
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId);
        Task<bool> ExistsByCodeAndTemplateIdAsync(string code, Guid tableScaleTemplateId, Guid? excludeId = null);
    }
}
