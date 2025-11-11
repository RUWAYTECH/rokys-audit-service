using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class AuditRoleConfigurationConfig : IEntityTypeConfiguration<AuditRoleConfiguration>
    {
        public void Configure(EntityTypeBuilder<AuditRoleConfiguration> builder)
        {
            builder.ToTable("AuditRoleConfiguration");

            builder.HasKey(a => a.AuditRoleConfigurationId);

            builder.Property(a => a.AuditRoleConfigurationId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(a => a.RoleCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(a => a.RoleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.IsRequired)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(a => a.AllowMultiple)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.SequenceOrder)
                .IsRequired(false);

            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Unique constraint for RoleCode
            builder.HasIndex(a => a.RoleCode)
                .IsUnique()
                .HasDatabaseName("UQ_AuditRoleConfiguration_RoleCode");

            // Index for performance
            builder.HasIndex(a => new { a.IsActive, a.SequenceOrder })
                .HasDatabaseName("IX_AuditRoleConfiguration_Active_Sequence");
        }
    }
}