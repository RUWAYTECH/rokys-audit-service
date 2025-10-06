using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class MaintenanceTableConfig : IEntityTypeConfiguration<MaintenanceTable>
    {
        public void Configure(EntityTypeBuilder<MaintenanceTable> builder)
        {
            builder.ToTable("MaintenanceTable");

            builder.HasKey(e => e.MaintenanceTableId);

            builder.Property(e => e.MaintenanceTableId)
                   .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(e => e.Description)
                   .HasMaxLength(255);

            builder.Property(e => e.IsSystem)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(e => e.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.Property(e => e.CreationDate)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(e => e.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.UpdateDate);

            builder.Property(e => e.UpdatedBy)
                   .HasMaxLength(100);

            // Relación uno a muchos
            builder.HasMany(e => e.Details)
                   .WithOne(d => d.MaintenanceTable)
                   .HasForeignKey(d => d.MaintenanceTableId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
