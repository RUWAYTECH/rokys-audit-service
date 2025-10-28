using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ScaleCompanyConfig : IEntityTypeConfiguration<ScaleCompany>
    {
        public void Configure(EntityTypeBuilder<ScaleCompany> builder)
        {
            builder.ToTable("ScaleCompany");
            builder.HasKey(x => x.ScaleCompanyId);
            builder.Property(x => x.EnterpriseId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MinValue).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(x => x.MaxValue).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(x => x.ColorCode).HasMaxLength(20);
            builder.Property(x => x.SortOrder).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdateDate);

            // Navigation properties
            builder.HasOne(sc => sc.Enterprise)
                .WithMany(e => e.ScaleCompanies)
                .HasForeignKey(sc => sc.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sc => sc.EnterpriseId);
        }
    }
}
