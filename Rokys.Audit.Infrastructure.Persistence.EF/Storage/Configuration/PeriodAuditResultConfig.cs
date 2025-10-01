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
                .HasColumnName("ObtainedValue")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.RiskLevel)
                .HasColumnName("RiskLevel")
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(x => x.AppliedWeighting)
                .HasColumnName("AppliedWeighting")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(x => x.AppliedLowThreshold)
                .HasColumnName("AppliedLowThreshold")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(x => x.AppliedModerateThreshold)
                .HasColumnName("AppliedModerateThreshold")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(x => x.AppliedHighThreshold)
                .HasColumnName("AppliedHighThreshold")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(x => x.Observations)
                .HasColumnName("Observations")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

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
            builder.HasMany(x => x.PeriodAuditScaleResults)
                .WithOne(x => x.PeriodAuditResult)
                .HasForeignKey(x => x.PeriodAuditResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new { x.PeriodAuditId, x.GroupId })
                .IsUnique()
                .HasDatabaseName("IX_PeriodAuditResult_Audit_Group");

            builder.HasIndex(x => x.RiskLevel)
                .HasDatabaseName("IX_PeriodAuditResult_RiskLevel");
        }
    }
}