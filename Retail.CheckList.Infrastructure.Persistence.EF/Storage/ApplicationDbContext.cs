using Microsoft.EntityFrameworkCore;
using Retail.CheckList.Infrastructure.Persistence.EF.Storage.Configuration;

namespace Retail.CheckList.Infrastructure.Persistence.EF.Storage
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProveedorConfig());
        }
    }
}
