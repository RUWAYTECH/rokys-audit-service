using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ScaleCompany> ScaleCompanies { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ScaleCompanyConfig());
            modelBuilder.ApplyConfiguration(new GroupConfig());
        }
    }
}
