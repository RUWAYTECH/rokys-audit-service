using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EnterpriseConfig : IEntityTypeConfiguration<Enterprise>
    {
        public void Configure(EntityTypeBuilder<Enterprise> builder)
        {
            builder.ToTable("Enterprise");

            builder.HasKey(e => e.EnterpriseId);

            builder.Property(e => e.EnterpriseId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Address)
                .HasMaxLength(500);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Navigation properties
            builder.HasMany(e => e.Stores)
                .WithOne(s => s.Enterprise)
                .HasForeignKey(s => s.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.ScaleCompanies)
                .WithOne(sc => sc.Enterprise)
                .HasForeignKey(sc => sc.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Groups)
                .WithOne(g => g.Enterprise)
                .HasForeignKey(g => g.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique();
        }
    }
}