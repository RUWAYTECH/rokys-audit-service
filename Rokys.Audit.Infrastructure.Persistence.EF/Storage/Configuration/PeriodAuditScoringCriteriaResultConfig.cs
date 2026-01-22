using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditScoringCriteriaResultConfig : IEntityTypeConfiguration<PeriodAuditScoringCriteriaResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditScoringCriteriaResult> builder)
        {
            builder.ToTable("PeriodAuditScoringCriteriaResult");

            builder.HasKey(x => x.PeriodAuditScoringCriteriaResultId);

            builder.Property(x => x.PeriodAuditScoringCriteriaResultId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.PeriodAuditScaleResultId)
                .IsRequired();

            // Identificación del Criterio
            builder.Property(x => x.CriteriaName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.CriteriaCode)
                .HasMaxLength(10);

            // Fórmula y Evaluación
            builder.Property(x => x.ResultFormula)
                .HasMaxLength(500);

            builder.Property(x => x.ComparisonOperator)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.ExpectedValue)
                .IsRequired()
                .HasMaxLength(255);

            // Puntuación
            builder.Property(x => x.Score)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            builder.Property(x => x.SuccessMessage)
                .HasMaxLength(500);

            builder.Property(x => x.ResultObtained)
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(x => x.CreatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UpdateDate)
                .HasColumnType("datetime2");

            // Navigation properties
            builder.HasOne(x => x.PeriodAuditScaleResult)
                .WithMany(pasr => pasr.PeriodAuditScoringCriteriaResults)
                .HasForeignKey(x => x.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}