using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage
{
    public class ApplicationDbContext : DbContext
    {
        // Core business entities
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Stores> Stores { get; set; }
        
        // Existing entities
        public DbSet<ScaleCompany> ScaleCompanies { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ScaleGroup> ScaleGroups { get; set; }
        //public DbSet<Proveedor> Proveedores { get; set; }
        
        // New audit entities
        public DbSet<AuditTemplateFields> AuditTemplateFields { get; set; }
        public DbSet<ScoringCriteria> ScoringCriteria { get; set; }
        public DbSet<CriteriaSubResult> CriteriaSubResults { get; set; }
        public DbSet<PeriodAudit> PeriodAudits { get; set; }
        public DbSet<PeriodAuditResult> PeriodAuditResults { get; set; }
        public DbSet<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; }
        public DbSet<PeriodAuditScaleSubResult> PeriodAuditScaleSubResults { get; set; }
        public DbSet<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Core business configurations
            modelBuilder.ApplyConfiguration(new EnterpriseConfig());
            modelBuilder.ApplyConfiguration(new StoresConfig());
            
            // Existing configurations
            modelBuilder.ApplyConfiguration(new ScaleCompanyConfig());
            modelBuilder.ApplyConfiguration(new GroupConfig());
            modelBuilder.ApplyConfiguration(new ScaleGroupConfig());
            //modelBuilder.ApplyConfiguration(new ProveedorConfig());
            
            // New audit configurations
            modelBuilder.ApplyConfiguration(new ScoringCriteriaConfig());
            modelBuilder.ApplyConfiguration(new CriteriaSubResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditScaleResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditScaleSubResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditFieldValuesConfig());
        }
    }
}
