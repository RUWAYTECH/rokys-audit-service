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

            builder.Property(x => x.PeriodAuditGroupResultId)
                .HasColumnName("PeriodAuditGroupResultId")
                .IsRequired();

            builder.Property(x => x.ScaleGroupId)
                .HasColumnName("ScaleGroupId")
                .IsRequired();

            builder.Property(x => x.ScoreValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.AppliedWeighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.Observations)
                .HasColumnName("Observations")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.Impact)
                .HasColumnName("Impact")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.Recommendation)
                .HasColumnName("Recommendation")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.Valorized)
                .HasColumnName("Valorized")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.ScaleDescription)
                .HasColumnName("ScaleDescription")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.ScaleColor)
                .HasColumnName("ScaleColor")
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(x => x.SortOrder);

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
            builder.HasOne(x => x.PeriodAuditGroupResult)
                .WithMany(x => x.PeriodAuditScaleResults)
                .HasForeignKey(x => x.PeriodAuditGroupResultId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ScaleGroup)
                .WithMany(x => x.PeriodAuditScaleResults)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navigation properties
            builder.HasMany(x => x.PeriodAuditTableScaleTemplateResults)
                .WithOne(patstr => patstr.PeriodAuditScaleResult)
                .HasForeignKey(patstr => patstr.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PeriodAuditScoringCriteriaResults)
                .WithOne(pascr => pascr.PeriodAuditScaleResult)
                .HasForeignKey(pascr => pascr.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PeriodAuditScaleSubResults)
                .WithOne(passr => passr.PeriodAuditScaleResult)
                .HasForeignKey(passr => passr.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => new { x.PeriodAuditGroupResultId, x.ScaleGroupId })
                .IsUnique()
                .HasDatabaseName("IX_PeriodAuditScaleResult_Result_ScaleGroup");
        }
    }
}