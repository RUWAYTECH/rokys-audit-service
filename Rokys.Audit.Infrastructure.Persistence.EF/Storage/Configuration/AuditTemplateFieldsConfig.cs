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
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.TableScaleTemplateId)
                .IsRequired();

            // Información del Campo
            builder.Property(x => x.FieldCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.FieldName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FieldType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsCalculated);

            builder.Property(x => x.CalculationFormula)
                .HasMaxLength(500);

            builder.Property(x => x.AcumulationType)
                .HasMaxLength(50);

            builder.Property(x => x.FieldOptions)
                .HasColumnType("nvarchar(max)");

            // Metadatos
            builder.Property(x => x.DefaultValue)
                .HasColumnType("nvarchar(max)");

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
            builder.HasOne(x => x.TableScaleTemplate)
                .WithMany(tst => tst.AuditTemplateFields)
                .HasForeignKey(x => x.TableScaleTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PeriodAuditFieldValues)
                .WithOne(pafv => pafv.AuditTemplateField)
                .HasForeignKey(pafv => pafv.AuditTemplateFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.FieldCode)
                .HasDatabaseName("IX_AuditTemplateFields_FieldCode");

            builder.HasIndex(x => x.TableScaleTemplateId)
                .HasDatabaseName("IX_AuditTemplateFields_TableScaleTemplateId");

            // Check constraint for AcumulationType (will be added via SQL)
            // CONSTRAINT CK_AuditTemplateFields_AcumulationType CHECK (AcumulationType IN ('SUM', 'COUNT') OR AcumulationType IS NULL)
        }
    }
}