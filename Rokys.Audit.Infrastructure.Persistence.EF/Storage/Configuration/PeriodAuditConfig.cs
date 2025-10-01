using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditConfig : IEntityTypeConfiguration<PeriodAudit>
    {
        public void Configure(EntityTypeBuilder<PeriodAudit> builder)
        {
            builder.ToTable("PeriodAudit");

            builder.HasKey(x => x.PeriodAuditId);

            builder.Property(x => x.PeriodAuditId)
                .HasColumnName("PeriodAuditId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.StoreId)
                .HasColumnName("StoreId")
                .IsRequired(false);

            builder.Property(x => x.StoreName)
                .HasColumnName("StoreName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.AdministratorId)
                .HasColumnName("AdministratorId")
                .IsRequired(false);

            builder.Property(x => x.AssistantId)
                .HasColumnName("AssistantId")
                .IsRequired(false);

            builder.Property(x => x.OperationManagersId)
                .HasColumnName("OperationManagersId")
                .IsRequired(false);

            builder.Property(x => x.FloatingAdministratorId)
                .HasColumnName("FloatingAdministratorId")
                .IsRequired(false);

            builder.Property(x => x.ResponsibleAuditorId)
                .HasColumnName("ResponsibleAuditorId")
                .IsRequired(false);

            builder.Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.ReportDate)
                .HasColumnName("ReportDate")
                .HasColumnType("datetime2(7)")
                .IsRequired(false);

            builder.Property(x => x.AuditedDays)
                .HasColumnName("AuditedDays")
                .IsRequired(false);

            builder.Property(x => x.GlobalObservations)
                .HasColumnName("GlobalObservations")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            builder.Property(x => x.TotalWeighting)
                .HasColumnName("TotalWeighting")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true)
                .IsRequired();

            // Audit properties
            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CreationDate)
                .HasColumnName("CreationDate")
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.UpdateDate)
                .HasColumnName("UpdateDate")
                .HasColumnType("datetime2(7)")
                .IsRequired(false);

            // Navigation properties
            builder.HasMany(x => x.PeriodAuditResults)
                .WithOne(x => x.PeriodAudit)
                .HasForeignKey(x => x.PeriodAuditId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.StoreId)
                .HasDatabaseName("IX_PeriodAudit_Store");

            builder.HasIndex(x => new { x.StartDate, x.EndDate })
                .HasDatabaseName("IX_PeriodAudit_DateRange");

            builder.HasIndex(x => x.ResponsibleAuditorId)
                .HasDatabaseName("IX_PeriodAudit_ResponsibleAuditor");
        }
    }
}