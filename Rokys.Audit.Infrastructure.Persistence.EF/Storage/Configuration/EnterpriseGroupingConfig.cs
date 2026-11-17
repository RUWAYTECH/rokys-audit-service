using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EnterpriseGroupingConfig : IEntityTypeConfiguration<EnterpriseGrouping>
    {
        public void Configure(EntityTypeBuilder<EnterpriseGrouping> builder)
        {
            builder.ToTable("EnterpriseGrouping");

            builder.HasKey(e => e.EnterpriseGroupingId);

            builder.Property(e => e.EnterpriseGroupingId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.Code)
                .IsUnique();
            builder.Property(x => x.ScaleType).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(120);

            builder.Property(e => e.CreationDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(e => e.UpdateDate);

            // Relaciones
            builder.HasMany(e => e.EnterpriseGroups)
                .WithOne(eg => eg.EnterpriseGrouping)
                .HasForeignKey(eg => eg.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.GroupingUsers)
                .WithOne(gu => gu.EnterpriseGrouping)
                .HasForeignKey(gu => gu.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
