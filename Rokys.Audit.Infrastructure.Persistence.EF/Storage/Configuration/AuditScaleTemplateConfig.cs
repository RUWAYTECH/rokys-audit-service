using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class AuditScaleTemplateConfig : IEntityTypeConfiguration<AuditScaleTemplate>
    {
        public void Configure(EntityTypeBuilder<AuditScaleTemplate> builder)
        {
            builder.ToTable("AuditScaleTemplate");

            builder.HasKey(x => x.AuditScaleTemplateId);

            builder.Property(x => x.AuditScaleTemplateId)
                .HasColumnName("AuditScaleTemplateId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Code)
                .HasColumnName("Code")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.TemplateData)
                .HasColumnName("TemplateData")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreationDate)
                .HasColumnName("CreationDate")
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.UpdateDate)
                .HasColumnName("UpdateDate")
                .HasColumnType("datetime2(7)")
                .IsRequired(false);

            // Unique constraint for Code
            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasDatabaseName("IX_AuditScaleTemplate_Code");

            // Check constraint for valid JSON
            builder.HasCheckConstraint("CK_AuditScaleTemplate_TemplateData_JSON", "ISJSON(TemplateData) = 1");

            // Navigation properties
            builder.HasMany(x => x.AuditTemplateFields)
                .WithOne(x => x.AuditScaleTemplate)
                .HasForeignKey(x => x.AuditScaleTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}