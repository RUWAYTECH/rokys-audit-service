using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class GroupConfig : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");

            builder.HasKey(g => g.GroupId);

            builder.Property(g => g.GroupId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(g => g.EnterpriseId)
                .IsRequired(false);

            builder.Property(g => g.EnterpriseGroupingId)
                .IsRequired(false);

            builder.Property(g => g.Code)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Weighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(g => g.IsActive)
                .HasDefaultValue(true);

            builder.Property(g => g.CreatedBy)
                .HasMaxLength(120);

            builder.Property(g => g.CreationDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(g => g.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(g => g.UpdateDate);

            // Relaciones
            builder.HasOne(g => g.Enterprise)
                .WithMany(e => e.Groups)
                .HasForeignKey(g => g.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.EnterpriseGrouping)
                .WithMany()
                .HasForeignKey(g => g.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

