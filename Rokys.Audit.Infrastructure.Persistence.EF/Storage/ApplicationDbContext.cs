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
        
        // User and employee management
        public DbSet<UserReference> UserReferences { get; set; }
        public DbSet<EmployeeStore> EmployeeStores { get; set; }
        
        // New audit entities
        public DbSet<TableScaleTemplate> TableScaleTemplates { get; set; }
        public DbSet<AuditTemplateFields> AuditTemplateFields { get; set; }
        public DbSet<ScoringCriteria> ScoringCriteria { get; set; }
        public DbSet<CriteriaSubResult> CriteriaSubResults { get; set; }
        public DbSet<PeriodAudit> PeriodAudits { get; set; }
        //public DbSet<PeriodAuditResult> PeriodAuditResults { get; set; }
        public DbSet<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; }
        public DbSet<PeriodAuditScaleSubResult> PeriodAuditScaleSubResults { get; set; }
        public DbSet<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; }
        public DbSet<PeriodAuditTableScaleTemplateResult> PeriodAuditTableScaleTemplateResults { get; set; }
        public DbSet<PeriodAuditScoringCriteriaResult> PeriodAuditScoringCriteriaResults { get; set; }
        public DbSet<StorageFiles> EvidenceFiles { get; set; }
        public DbSet<InboxItems> InboxItems { get; set; }
        public DbSet<AuditStatus> AuditStatuses { get; set; }
        public DbSet<MaintenanceTable> MaintenanceTables { get; set; }
        public DbSet<MaintenanceDetailTable> MaintenanceDetailTables { get; set; }

        public DbSet<PeriodAuditGroupResult> PeriodAuditGroupResults { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Core business configurations
            modelBuilder.ApplyConfiguration(new EnterpriseConfig());
            modelBuilder.ApplyConfiguration(new StoresConfig());
            
            // User and employee management configurations
            modelBuilder.ApplyConfiguration(new UserReferenceConfig());
            modelBuilder.ApplyConfiguration(new EmployeeStoreConfig());
            
            // Existing configurations
            modelBuilder.ApplyConfiguration(new ScaleCompanyConfig());
            modelBuilder.ApplyConfiguration(new GroupConfig());
            modelBuilder.ApplyConfiguration(new ScaleGroupConfig());
            //modelBuilder.ApplyConfiguration(new ProveedorConfig());
            
            // New audit configurations
            modelBuilder.ApplyConfiguration(new TableScaleTemplateConfig());
            modelBuilder.ApplyConfiguration(new ScoringCriteriaConfig());
            modelBuilder.ApplyConfiguration(new CriteriaSubResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditConfig());
            //modelBuilder.ApplyConfiguration(new PeriodAuditResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditScaleResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditScaleSubResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditFieldValuesConfig());

            modelBuilder.ApplyConfiguration(new AuditTemplateFieldsConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditTableScaleTemplateResultConfig());
            modelBuilder.ApplyConfiguration(new PeriodAuditScoringCriteriaResultConfig());
            modelBuilder.ApplyConfiguration(new StorageFilesConfig());
            modelBuilder.ApplyConfiguration(new InboxItemsConfig());
            modelBuilder.ApplyConfiguration(new AuditStatusConfig());

            modelBuilder.ApplyConfiguration(new MaintenanceTableConfig());
            modelBuilder.ApplyConfiguration(new MaintenanceDetailTableConfig());

            modelBuilder.ApplyConfiguration(new PeriodAuditGroupResultConfig());
        }
    }
}
