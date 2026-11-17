using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class GroupingUserConfig : IEntityTypeConfiguration<GroupingUser>
    {
        public void Configure(EntityTypeBuilder<GroupingUser> builder)
        {
            builder.ToTable("GroupingUser");

            builder.HasKey(x => x.GroupingUserId);

            builder.Property(x => x.GroupingUserId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.EnterpriseGroupingId)
                .IsRequired();

            builder.Property(x => x.UserReferenceId)
                .IsRequired();

            builder.Property(x => x.RolesCodes)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.CreationDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UpdateDate);

            // Índice único para evitar duplicados
            builder.HasIndex(x => new { x.EnterpriseGroupingId, x.UserReferenceId })
                .IsUnique();

            // Relaciones
            builder.HasOne(x => x.EnterpriseGrouping)
                .WithMany(eg => eg.GroupingUsers)
                .HasForeignKey(x => x.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserReference)
                .WithMany(ur => ur.GroupingUsers)
                .HasForeignKey(x => x.UserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
