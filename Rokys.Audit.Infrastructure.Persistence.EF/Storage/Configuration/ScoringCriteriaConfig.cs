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
                .HasColumnName("ScaleGroupId")
                .IsRequired();

            builder.Property(x => x.AuditTemplateFieldId)
                .HasColumnName("AuditTemplateFieldId")
                .IsRequired(false);

            builder.Property(x => x.CriteriaName)
                .HasColumnName("CriteriaName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CriteriaCode)
                .HasColumnName("CriteriaCode")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.EvaluatedFieldCode)
                .HasColumnName("EvaluatedFieldCode")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.EvaluatedFieldName)
                .HasColumnName("EvaluatedFieldName")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.EvaluatedFieldType)
                .HasColumnName("EvaluatedFieldType")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.ResultFormula)
                .HasColumnName("ResultFormula")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.ComparisonOperator)
                .HasColumnName("ComparisonOperator")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.ExpectedValue)
                .HasColumnName("ExpectedValue")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            builder.Property(x => x.Score)
                .HasColumnName("Score")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.ScoreWeight)
                .HasColumnName("ScoreWeight")
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(1.00m)
                .IsRequired();

            builder.Property(x => x.IsRequired)
                .HasColumnName("IsRequired")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .HasColumnName("SortOrder")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasColumnName("ErrorMessage")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.SuccessMessage)
                .HasColumnName("SuccessMessage")
                .HasMaxLength(500)
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
            builder.HasOne(x => x.ScaleGroup)
                .WithMany(x => x.ScoringCriteria)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AuditTemplateField)
                .WithMany(x => x.ScoringCriteria)
                .HasForeignKey(x => x.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ScaleGroupId)
                .HasDatabaseName("IX_ScoringCriteria_ScaleGroup");

            builder.HasIndex(x => x.EvaluatedFieldCode)
                .HasDatabaseName("IX_ScoringCriteria_EvaluatedFieldCode");

            builder.HasIndex(x => x.CriteriaCode)
                .HasDatabaseName("IX_ScoringCriteria_CriteriaCode");
        }
    }
}