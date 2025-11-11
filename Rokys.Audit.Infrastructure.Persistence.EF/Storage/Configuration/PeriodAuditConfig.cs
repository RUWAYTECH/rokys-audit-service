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
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.StatusId);

            builder.Property(x => x.CorrelativeNumber)
                .HasColumnName("CorrelativeNumber")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.ScaleCode)
                .HasColumnName("ScaleCode")
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(x => x.IsActive)
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
            builder.HasOne(x => x.Store)
                .WithMany(s => s.PeriodAudits)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AuditStatus)
                .WithMany(ast => ast.PeriodAudits)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PeriodAuditResults)
                .WithOne(par => par.PeriodAudit)
                .HasForeignKey(par => par.PeriodAuditId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PeriodAuditParticipants)
                .WithOne(p => p.PeriodAudit)
                .HasForeignKey(p => p.PeriodAuditId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.StoreId)
                .HasDatabaseName("IX_PeriodAudit_Store");

            builder.HasIndex(x => new { x.StartDate, x.EndDate })
                .HasDatabaseName("IX_PeriodAudit_DateRange");
        }
    }
}