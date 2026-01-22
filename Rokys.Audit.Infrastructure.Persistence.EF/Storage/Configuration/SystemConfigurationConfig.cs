using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class SystemConfigurationConfig : IEntityTypeConfiguration<SystemConfiguration>
    {
        public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
        {
            builder.ToTable("SystemConfiguration");

            builder.HasKey(x => x.SystemConfigurationId);

            builder.Property(x => x.SystemConfigurationId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.ConfigKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ConfigValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.DataType)
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.ReferenceType)
                .HasMaxLength(50);

            builder.Property(x => x.ReferenceCode)
                .HasMaxLength(50);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(x => x.CreatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UpdateDate)
                .HasColumnType("datetime2");

            // Unique constraint
            builder.HasIndex(x => x.ConfigKey)
                .IsUnique()
                .HasDatabaseName("UQ_SystemConfiguration_Key");
        }
    }
}
