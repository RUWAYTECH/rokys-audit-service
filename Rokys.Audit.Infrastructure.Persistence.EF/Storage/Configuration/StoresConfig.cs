using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class StoresConfig : IEntityTypeConfiguration<Stores>
    {
        public void Configure(EntityTypeBuilder<Stores> builder)
        {
            builder.ToTable("Stores");

            builder.HasKey(s => s.StoreId);

            builder.Property(s => s.StoreId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Address)
                .HasMaxLength(500);

            builder.Property(s => s.Email)
                .HasMaxLength(255);

            builder.Property(s => s.EnterpriseId)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Navigation properties
            builder.HasOne(s => s.Enterprise)
                .WithMany(e => e.Stores)
                .HasForeignKey(s => s.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.PeriodAudits)
                .WithOne(pa => pa.Store)
                .HasForeignKey(pa => pa.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => s.Code)
                .IsUnique();

            builder.HasIndex(s => s.EnterpriseId);
        }
    }
}