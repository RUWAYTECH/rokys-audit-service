using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class AuditTemplateFieldsConfig : IEntityTypeConfiguration<AuditTemplateFields>
    {
        public void Configure(EntityTypeBuilder<AuditTemplateFields> builder)
        {
            builder.ToTable("AuditTemplateFields");

            builder.HasKey(x => x.AuditTemplateFieldId);

            builder.Property(x => x.AuditTemplateFieldId)
                .HasColumnName("AuditTemplateFieldId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ScaleGroupId)
                .HasColumnName("ScaleGroupId")
                .IsRequired();

            builder.Property(x => x.AuditScaleTemplateId)
                .HasColumnName("AuditScaleTemplateId")
                .IsRequired();

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
                .HasMaxLength(2)
                .IsRequired();

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

            builder.Property(x => x.DefaultValue)
                .HasColumnName("DefaultValue")
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
                .WithMany(x => x.AuditTemplateFields)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AuditScaleTemplate)
                .WithMany(x => x.AuditTemplateFields)
                .HasForeignKey(x => x.AuditScaleTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navigation properties
            builder.HasMany(x => x.ScoringCriteria)
                .WithOne(x => x.AuditTemplateField)
                .HasForeignKey(x => x.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PeriodAuditFieldValues)
                .WithOne(x => x.AuditTemplateField)
                .HasForeignKey(x => x.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new { x.ScaleGroupId, x.AuditScaleTemplateId })
                .HasDatabaseName("IX_AuditTemplateFields_ScaleGroup_Template");

            builder.HasIndex(x => x.FieldCode)
                .HasDatabaseName("IX_AuditTemplateFields_FieldCode");
        }
    }
}