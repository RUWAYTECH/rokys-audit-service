using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class SubScaleConfig : IEntityTypeConfiguration<SubScale>
    {
        public void Configure(EntityTypeBuilder<SubScale> builder)
        {
            builder.ToTable("SubScale");

            builder.HasKey(x => x.SubScaleId);

            builder.Property(x => x.SubScaleId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.EnterpriseGroupingId)
                .IsRequired();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.ColorCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UpdateDate);

            // Relaciones
            builder.HasOne(x => x.EnterpriseGrouping)
                .WithMany(x => x.SubScales)
                .HasForeignKey(x => x.EnterpriseGroupingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
