using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class AuditStatusConfig : IEntityTypeConfiguration<AuditStatus>
    {
        public void Configure(EntityTypeBuilder<AuditStatus> builder)
        {
            builder.ToTable("AuditStatus");

            builder.HasKey(x => x.AuditStatusId);

            builder.Property(x => x.AuditStatusId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.ColorCode)
                .HasMaxLength(10);

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
                .HasDatabaseName("UX_AuditStatus_Code");

            // Navigation properties
            builder.HasMany(x => x.PeriodAudits)
                .WithOne(pa => pa.AuditStatus)
                .HasForeignKey(pa => pa.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}