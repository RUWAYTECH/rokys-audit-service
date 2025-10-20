using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IScaleGroupRepository : IRepository<ScaleGroup>
    {
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId);
        Task<List<ScaleGroup>> GetByGroupIdAsync(Guid groupId);
        Task<bool> GetValidatorByGroupIdAsync(string code, Guid groupId, Guid? excludeId);
    }
}