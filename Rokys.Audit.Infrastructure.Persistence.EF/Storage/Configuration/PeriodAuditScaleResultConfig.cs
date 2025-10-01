using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditScaleResultConfig : IEntityTypeConfiguration<PeriodAuditScaleResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditScaleResult> builder)
        {
            builder.ToTable("PeriodAuditScaleResult");

            builder.HasKey(x => x.PeriodAuditScaleResultId);

            builder.Property(x => x.PeriodAuditScaleResultId)
                .HasColumnName("PeriodAuditScaleResultId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PeriodAuditResultId)
                .HasColumnName("PeriodAuditResultId")
                .IsRequired();

            builder.Property(x => x.ScaleGroupId)
                .HasColumnName("ScaleGroupId")
                .IsRequired();

            builder.Property(x => x.TotalValue)
                .HasColumnName("TotalValue")
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
            builder.HasOne(x => x.PeriodAuditResult)
                .WithMany(x => x.PeriodAuditScaleResults)
                .HasForeignKey(x => x.PeriodAuditResultId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ScaleGroup)
                .WithMany(x => x.PeriodAuditScaleResults)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navigation properties
            builder.HasMany(x => x.PeriodAuditFieldValues)
                .WithOne(x => x.PeriodAuditScaleResult)
                .HasForeignKey(x => x.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new { x.PeriodAuditResultId, x.ScaleGroupId })
                .IsUnique()
                .HasDatabaseName("IX_PeriodAuditScaleResult_Result_ScaleGroup");

            builder.HasIndex(x => x.RiskLevel)
                .HasDatabaseName("IX_PeriodAuditScaleResult_RiskLevel");
        }
    }
}