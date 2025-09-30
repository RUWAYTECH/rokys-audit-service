using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class RiskScaleConfig : IEntityTypeConfiguration<RiskScale>
    {
        public void Configure(EntityTypeBuilder<RiskScale> builder)
        {
            builder.ToTable("RiskScale");
            builder.HasKey(r => r.RiskScaleId);
            
            builder.Property(a => a.RiskScaleId)
                .UseIdentityColumn(1, 1);
                
            builder.Property(a => a.RiskScaleGroupId)
                .IsRequired();
                
            builder.Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(a => a.ShortDescription)
                .HasMaxLength(100);
                
            builder.Property(a => a.ObjectiveValue)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.LowRisk)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.ModerateRisk)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.HighRisk)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.Weighting)
                .HasColumnType("decimal(5,2)");
                
            builder.Property(a => a.NonToleratedRisk)
                .HasDefaultValue(false);
                
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);
                
            builder.Property(a => a.CreatedBy)
                .HasMaxLength(120);
                
            builder.Property(a => a.CreationDate)
                .HasDefaultValueSql("GETDATE()");
                
            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(120);

            // Foreign key relationships
            builder.HasOne(a => a.RiskScaleGroup)
                .WithMany(rsg => rsg.RiskScales)
                .HasForeignKey(a => a.RiskScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}