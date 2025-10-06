using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class MaintenanceDetailTableConfig : IEntityTypeConfiguration<MaintenanceDetailTable>
    {
        public void Configure(EntityTypeBuilder<MaintenanceDetailTable> builder)
        {
            builder.ToTable("MaintenanceDetailTable");
            builder.HasKey(e => e.MaintenanceDetailTableId);
            builder.Property(e => e.MaintenanceDetailTableId)
                   .HasDefaultValueSql("NEWID()");
            builder.Property(e => e.MaintenanceTableId)
                   .IsRequired();
            builder.Property(e => e.Code)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(e => e.Description)
                   .HasMaxLength(255);
            builder.Property(e => e.JsonData);
            builder.Property(e => e.OrderRow);
            builder.Property(e => e.IsDefault)
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
            // Relación muchos a uno
            builder.HasOne(d => d.MaintenanceTable)
                   .WithMany(m => m.Details)
                   .HasForeignKey(d => d.MaintenanceTableId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
