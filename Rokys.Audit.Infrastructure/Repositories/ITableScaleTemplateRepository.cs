using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface ITableScaleTemplateRepository : IRepository<TableScaleTemplate>
    {
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId);
        Task<IEnumerable<TableScaleTemplate>> GetByScaleGroupIdAsync(Guid scaleGroupId);
        Task<bool> ExistsByCodeAndScaleGroupIdAsync(string code, Guid scaleGroupId, Guid? excludeId = null);
    }
}