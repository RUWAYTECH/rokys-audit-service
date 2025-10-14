using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditResultConfig : IEntityTypeConfiguration<PeriodAuditResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditResult> builder)
        {
            builder.ToTable("PeriodAuditResult");

            builder.HasKey(x => x.PeriodAuditResultId);

            builder.Property(x => x.PeriodAuditResultId)
                .HasColumnName("PeriodAuditResultId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PeriodAuditId)
                .HasColumnName("PeriodAuditId")
                .IsRequired();

            builder.Property(x => x.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            builder.Property(x => x.ObtainedValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            // Historical weighting and thresholds
            builder.Property(x => x.AppliedRiskLow)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.AppliedRiskModerate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.AppliedRiskHigh)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.AppliedRiskCritical)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.AppliedWeighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.Observations)
                .HasMaxLength(150);

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

            // Foreign key relationships
            builder.HasOne(x => x.PeriodAudit)
                .WithMany(x => x.PeriodAuditResults)
                .HasForeignKey(x => x.PeriodAuditId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Group)
                .WithMany(x => x.PeriodAuditResults)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navigation properties
            //builder.HasMany(x => x.PeriodAuditScaleResults)
            //    .WithOne(pasr => pasr.PeriodAuditResult)
            //    .HasForeignKey(pasr => pasr.PeriodAuditResultId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.EvidenceFiles)
                .WithOne(ef => ef.PeriodAuditResult)
                .HasForeignKey(ef => ef.PeriodAuditResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => new { x.PeriodAuditId, x.GroupId })
                .IsUnique()
                .HasDatabaseName("IX_PeriodAuditResult_Audit_Group");
        }
    }
}