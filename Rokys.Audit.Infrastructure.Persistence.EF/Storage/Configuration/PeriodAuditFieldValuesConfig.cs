using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditFieldValuesConfig : IEntityTypeConfiguration<PeriodAuditFieldValues>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditFieldValues> builder)
        {
            builder.ToTable("PeriodAuditFieldValues");

            builder.HasKey(x => x.PeriodAuditFieldValueId);

            builder.Property(x => x.PeriodAuditFieldValueId)
                .HasColumnName("PeriodAuditFieldValueId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.AuditTemplateFieldId)
                .IsRequired(false);

            builder.Property(x => x.PeriodAuditTableScaleTemplateResultId);

            // Información del Campo (desnormalizado)
            builder.Property(x => x.FieldCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.FieldName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FieldType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsCalculated)
                .HasMaxLength(50);

            builder.Property(x => x.CalculationFormula)
                .HasMaxLength(500);

            builder.Property(x => x.AcumulationType)
                .HasMaxLength(50);

            builder.Property(x => x.FieldOptions)
                .HasColumnType("nvarchar(max)");

            // VALORES CAPTURADOS
            builder.Property(x => x.TextValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NumericValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.DateValue)
                .HasColumnType("datetime2");

            builder.Property(x => x.BooleanValue);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.FieldOptionsValue)
                .HasMaxLength(255);

            builder.Property(x => x.ValidationStatus)
                .HasMaxLength(50);

            builder.Property(x => x.ValidationMessage)
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
            builder.HasOne(x => x.AuditTemplateField)
                .WithMany(atf => atf.PeriodAuditFieldValues)
                .HasForeignKey(x => x.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PeriodAuditTableScaleTemplateResult)
                .WithMany(patstr => patstr.PeriodAuditFieldValues)
                .HasForeignKey(x => x.PeriodAuditTableScaleTemplateResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.FieldCode)
                .HasDatabaseName("IX_PeriodAuditFieldValues_FieldCode");

            builder.HasIndex(x => x.AuditTemplateFieldId)
                .HasDatabaseName("IX_PeriodAuditFieldValues_TemplateField");

            builder.HasIndex(x => x.NumericValue)
                .HasDatabaseName("IX_PeriodAuditFieldValues_NumericValue")
                .HasFilter("NumericValue IS NOT NULL");
        }
    }
}