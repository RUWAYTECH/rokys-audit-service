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
                .HasColumnName("AuditTemplateFieldId")
                .IsRequired(false);

            builder.Property(x => x.ScaleGroupId)
                .HasColumnName("ScaleGroupId")
                .IsRequired();

            builder.Property(x => x.PeriodAuditScaleResultId)
                .HasColumnName("PeriodAuditScaleResultId")
                .IsRequired(false);

            builder.Property(x => x.GroupCode)
                .HasColumnName("GroupCode")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.GroupName)
                .HasColumnName("GroupName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Orientation)
                .HasColumnName("Orientation")
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(x => x.FieldCode)
                .HasColumnName("FieldCode")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.FieldName)
                .HasColumnName("FieldName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.FieldType)
                .HasColumnName("FieldType")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.TextValue)
                .HasColumnName("TextValue")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.NumericValue)
                .HasColumnName("NumericValue")
                .HasColumnType("decimal(18,6)")
                .IsRequired(false);

            builder.Property(x => x.DateValue)
                .HasColumnName("DateValue")
                .HasColumnType("datetime2(7)")
                .IsRequired(false);

            builder.Property(x => x.BooleanValue)
                .HasColumnName("BooleanValue")
                .IsRequired(false);

            builder.Property(x => x.ImageUrl)
                .HasColumnName("ImageUrl")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.IsRequired)
                .HasColumnName("IsRequired")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.ValidationStatus)
                .HasColumnName("ValidationStatus")
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(x => x.ValidationMessage)
                .HasColumnName("ValidationMessage")
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
            builder.HasOne(x => x.AuditTemplateField)
                .WithMany(x => x.PeriodAuditFieldValues)
                .HasForeignKey(x => x.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ScaleGroup)
                .WithMany(x => x.PeriodAuditFieldValues)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PeriodAuditScaleResult)
                .WithMany(x => x.PeriodAuditFieldValues)
                .HasForeignKey(x => x.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new { x.ScaleGroupId, x.FieldCode })
                .HasDatabaseName("IX_PeriodAuditFieldValues_Group_Field");

            builder.HasIndex(x => x.AuditTemplateFieldId)
                .HasDatabaseName("IX_PeriodAuditFieldValues_TemplateField");

            builder.HasIndex(x => x.PeriodAuditScaleResultId)
                .HasDatabaseName("IX_PeriodAuditFieldValues_ScaleResult");
        }
    }
}