using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class TableScaleTemplateConfig : IEntityTypeConfiguration<TableScaleTemplate>
    {
        public void Configure(EntityTypeBuilder<TableScaleTemplate> builder)
        {
            builder.ToTable("TableScaleTemplate");

            builder.HasKey(x => x.TableScaleTemplateId);

            builder.Property(x => x.TableScaleTemplateId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.ScaleGroupId)
                .IsRequired();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Title)
                .HasMaxLength(255);

            builder.Property(x => x.TemplateData)
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

            // Unique constraint
            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasDatabaseName("UX_TableScaleTemplate_Code");

            // Navigation properties
            builder.HasOne(x => x.ScaleGroup)
                .WithMany(sg => sg.TableScaleTemplates)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AuditTemplateFields)
                .WithOne(atf => atf.TableScaleTemplate)
                .HasForeignKey(atf => atf.TableScaleTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PeriodAuditTableScaleTemplateResults)
                .WithOne(patstr => patstr.TableScaleTemplate)
                .HasForeignKey(patstr => patstr.TableScaleTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraint for JSON validation (will be added via SQL)
            // CONSTRAINT CK_TemplateData_IsJson CHECK (ISJSON(TemplateData) = 1)
        }
    }
}