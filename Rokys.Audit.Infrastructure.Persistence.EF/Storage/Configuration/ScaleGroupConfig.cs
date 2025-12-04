using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ScaleGroupConfig : IEntityTypeConfiguration<ScaleGroup>
    {
        public void Configure(EntityTypeBuilder<ScaleGroup> builder)
        {
            builder.ToTable("ScaleGroup");
            builder.HasKey(r => r.ScaleGroupId);
            builder.Property(a => a.ScaleGroupId).HasDefaultValueSql("NEWID()");
            builder.Property(a => a.GroupId).IsRequired();
            builder.Property(a => a.Code).IsRequired().HasMaxLength(10);
            builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Weighting).IsRequired().HasColumnType("decimal(5,2)");
            builder.Property(g => g.HasSourceData)
                .IsRequired(false)
                .HasDefaultValue(false);
            builder.Property(g => g.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(a => a.IsActive).HasDefaultValue(true);
            builder.Property(a => a.CreatedBy).HasMaxLength(120);
            builder.Property(a => a.CreationDate).HasDefaultValueSql("GETDATE()");
            builder.Property(a => a.UpdatedBy).HasMaxLength(120);
            builder.Property(a => a.Recommendation).HasMaxLength(1000);
            builder.Property(a => a.Impact).HasMaxLength(1000);

            // Foreign key relationships
            builder.HasOne(a => a.Group)
                .WithMany(g => g.ScaleGroups)
                .HasForeignKey(a => a.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}