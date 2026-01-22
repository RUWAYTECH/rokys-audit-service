using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EnterpriseGroupConfig : IEntityTypeConfiguration<EnterpriseGroup>
    {
        public void Configure(EntityTypeBuilder<EnterpriseGroup> builder)
        {
            builder.ToTable("EnterpriseGroup");

            builder.HasKey(e => e.EnterpriseGroupId);

            builder.Property(e => e.EnterpriseGroupId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.EnterpriseId)
                .IsRequired();

            builder.Property(e => e.EnterpriseGroupingId)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(120);

            builder.Property(e => e.CreationDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(e => e.UpdateDate);

            // Constraint único
            builder.HasIndex(e => new { e.EnterpriseId, e.EnterpriseGroupingId })
                .IsUnique()
                .HasDatabaseName("UQ_EnterpriseGroup");

            // Relaciones
            builder.HasOne(eg => eg.Enterprise)
                .WithMany()
                .HasForeignKey(eg => eg.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(eg => eg.EnterpriseGrouping)
                .WithMany(eg => eg.EnterpriseGroups)
                .HasForeignKey(eg => eg.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
