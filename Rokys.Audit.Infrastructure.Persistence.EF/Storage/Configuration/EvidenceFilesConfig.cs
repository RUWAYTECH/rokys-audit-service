using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EvidenceFilesConfig : IEntityTypeConfiguration<EvidenceFiles>
    {
        public void Configure(EntityTypeBuilder<EvidenceFiles> builder)
        {
            builder.ToTable("EvidenceFiles");

            builder.HasKey(x => x.EvidenceFileId);

            builder.Property(x => x.EvidenceFileId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.PeriodAuditResultId)
                .IsRequired();

            builder.Property(x => x.OriginalName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.FileType)
                .HasMaxLength(50);

            builder.Property(x => x.UploadedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UploadDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

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
            builder.HasOne(x => x.PeriodAuditResult)
                .WithMany(par => par.EvidenceFiles)
                .HasForeignKey(x => x.PeriodAuditResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}