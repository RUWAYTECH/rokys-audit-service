using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditGroupResultConfig : IEntityTypeConfiguration<PeriodAuditGroupResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditGroupResult> builder) 
        {
            builder.ToTable("PeriodAuditGroupResult");
            builder.HasKey(x => x.PeriodAuditGroupResultId);

            builder.Property(x => x.PeriodAuditGroupResultId)
                .HasColumnName("PeriodAuditGroupResultId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PeriodAuditId)
                .HasColumnName("PeriodAuditId")
                .IsRequired();

            builder.Property(x => x.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            builder.Property(x => x.ScoreValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            builder.Property(x => x.Observations)
                .HasColumnName("Observations")
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(x => x.ScaleDescription)
                .HasColumnName("ScaleDescription")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.TotalWeighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.GroupColor)
                .HasColumnName("GroupColor")
                .HasMaxLength(20)
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
                .IsRequired();
            builder.Property(x => x.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .HasMaxLength(100)
                .IsRequired(false);
            builder.Property(x => x.UpdateDate)
                .HasColumnName("UpdateDate")
                .IsRequired(false);

            // Relaciones corregidas
            builder.HasOne(x => x.PeriodAudit)
                .WithMany(pa => pa.PeriodAuditGroupResults)
                .HasForeignKey(x => x.PeriodAuditId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Group)
                .WithMany(g => g.PeriodAuditGroupResults)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
