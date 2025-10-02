using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditScaleSubResultConfig : IEntityTypeConfiguration<PeriodAuditScaleSubResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditScaleSubResult> builder)
        {
            builder.ToTable("PeriodAuditScaleSubResult");

            builder.HasKey(x => x.PeriodAuditScaleSubResultId);

            builder.Property(x => x.PeriodAuditScaleSubResultId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PeriodAuditScaleResultId)
                .IsRequired();

            builder.Property(x => x.CriteriaSubResultId)
                .IsRequired();

            // Identificación del Criterio (desnormalizado)
            builder.Property(x => x.CriteriaCode)
                .HasMaxLength(10);

            builder.Property(x => x.CriteriaName)
                .IsRequired()
                .HasMaxLength(255);

            // Valores evaluados
            builder.Property(x => x.EvaluatedValue)
                .HasMaxLength(255);

            builder.Property(x => x.CalculatedResult)
                .HasMaxLength(255);

            builder.Property(x => x.AppliedFormula)
                .HasMaxLength(500);

            // Resultado de la evaluación
            builder.Property(x => x.ScoreObtained)
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.ColorCode)
                .HasMaxLength(20);

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
                .WithMany(pasr => pasr.PeriodAuditScaleSubResults)
                .HasForeignKey(x => x.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CriteriaSubResult)
                .WithMany(csr => csr.PeriodAuditScaleSubResults)
                .HasForeignKey(x => x.CriteriaSubResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.PeriodAuditScaleResultId)
                .HasDatabaseName("IX_PeriodAuditScaleSubResult_ScaleResultId");

            builder.HasIndex(x => x.CriteriaSubResultId)
                .HasDatabaseName("IX_PeriodAuditScaleSubResult_CriteriaId");

            // Unique constraint: Un sub-resultado por criterio por resultado de escala
            builder.HasIndex(x => new { x.PeriodAuditScaleResultId, x.CriteriaSubResultId })
                .IsUnique()
                .HasDatabaseName("UK_PeriodAuditScaleSubResult_ScaleResult_Criteria");
        }
    }
}