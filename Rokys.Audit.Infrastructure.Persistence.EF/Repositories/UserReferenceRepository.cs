using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class UserReferenceRepository : EFRepository<UserReference>, IUserReferenceRepository
    {
        public UserReferenceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserReference?> GetByUserIdAsync(Guid userId)
        {
            return await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(ur => ur.UserId == userId);
        }

        public async Task<UserReference?> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(ur => ur.EmployeeId == employeeId);
        }

        public async Task<UserReference?> GetByDocumentNumberAsync(string documentNumber)
        {
            return await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(ur => ur.DocumentNumber == documentNumber);
        }

        public async Task<UserReference?> GetByEmailAsync(string email)
        {
            return await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(ur => ur.Email == email);
        }

        public async Task<List<UserReference>> GetByRoleCodeAsync(string roleCode)
        {
            return await DbSet.AsNoTracking()
                .Where(ur => ur.RoleCode == roleCode && ur.IsActive)
                .OrderBy(ur => ur.FirstName)
                .ThenBy(ur => ur.LastName)
                .ToListAsync();
        }

        public async Task<List<UserReference>> GetActiveUsersAsync()
        {
            return await DbSet.AsNoTracking()
                .Where(ur => ur.IsActive)
                .OrderBy(ur => ur.FirstName)
                .ThenBy(ur => ur.LastName)
                .ToListAsync();
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId, Guid? excludeId = null)
        {
            var query = DbSet.Where(ur => ur.UserId == userId && ur.IsActive);
            
            if (excludeId.HasValue)
            {
                query = query.Where(ur => ur.UserReferenceId != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByEmployeeIdAsync(Guid employeeId, Guid? excludeId = null)
        {
            var query = DbSet.Where(ur => ur.EmployeeId == employeeId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(ur => ur.UserReferenceId != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<List<UserReference>> GetByRoleCodesAsync(List<string> roleCodes)
        {
            if (roleCodes == null || !roleCodes.Any())
            {
                return new List<UserReference>();
            }

            // Traer todos los usuarios activos
            var activeUsers = await DbSet
                .Where(u => u.IsActive)
                .ToListAsync();

            // Filtrar en memoria los que tengan alguno de los roles
            return activeUsers
                .Where(u => !string.IsNullOrEmpty(u.RoleCode) &&
                           roleCodes.Any(role => u.RoleCode.Contains(role)))
                .ToList();
        }
    }
}