using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class CriteriaSubResultConfig : IEntityTypeConfiguration<CriteriaSubResult>
    {
        public void Configure(EntityTypeBuilder<CriteriaSubResult> builder)
        {
            builder.ToTable("CriteriaSubResult");

            builder.HasKey(x => x.CriteriaSubResultId);

            builder.Property(x => x.CriteriaSubResultId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ScaleGroupId)
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

            builder.Property(x => x.ColorCode)
                .IsRequired()
                .HasMaxLength(20);

            // Puntuación
            builder.Property(x => x.Score)
                .HasColumnType("decimal(10,2)");

            // Configuración
            builder.Property(x => x.ForSummary)
                .IsRequired()
                .HasDefaultValue(false);

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
            builder.HasOne(x => x.ScaleGroup)
                .WithMany(sg => sg.CriteriaSubResults)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ScaleGroupId)
                .HasDatabaseName("IX_ScoringCriteria_ScaleGroupId");
        }
    }
}