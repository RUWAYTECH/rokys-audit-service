using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ScoringCriteriaConfig : IEntityTypeConfiguration<ScoringCriteria>
    {
        public void Configure(EntityTypeBuilder<ScoringCriteria> builder)
        {
            builder.ToTable("ScoringCriteria");

            builder.HasKey(x => x.ScoringCriteriaId);

            builder.Property(x => x.ScoringCriteriaId)
                .HasColumnName("ScoringCriteriaId")
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
            builder.HasOne(x => x.ScaleGroup)
                .WithMany(sg => sg.ScoringCriteria)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ScaleGroupId)
                .HasDatabaseName("IX_ScoringCriteria_ScaleGroupId");
        }
    }
}