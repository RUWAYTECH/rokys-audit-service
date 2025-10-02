using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditTableScaleTemplateResultConfig : IEntityTypeConfiguration<PeriodAuditTableScaleTemplateResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditTableScaleTemplateResult> builder)
        {
            builder.ToTable("PeriodAuditTableScaleTemplateResult");

            builder.HasKey(x => x.PeriodAuditTableScaleTemplateResultId);

            builder.Property(x => x.PeriodAuditTableScaleTemplateResultId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.PeriodAuditScaleResultId)
                .IsRequired();

            builder.Property(x => x.TableScaleTemplateId)
                .IsRequired();

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

            // Navigation properties
            builder.HasOne(x => x.PeriodAuditScaleResult)
                .WithMany(pasr => pasr.PeriodAuditTableScaleTemplateResults)
                .HasForeignKey(x => x.PeriodAuditScaleResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TableScaleTemplate)
                .WithMany(tst => tst.PeriodAuditTableScaleTemplateResults)
                .HasForeignKey(x => x.TableScaleTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PeriodAuditFieldValues)
                .WithOne(pafv => pafv.PeriodAuditTableScaleTemplateResult)
                .HasForeignKey(pafv => pafv.PeriodAuditTableScaleTemplateResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraint for JSON validation (will be added via SQL)
            // CONSTRAINT CK_PeriodAuditTableScaleTemplate_TemplateData_IsJson CHECK (ISJSON(TemplateData) = 1)
        }
    }
}