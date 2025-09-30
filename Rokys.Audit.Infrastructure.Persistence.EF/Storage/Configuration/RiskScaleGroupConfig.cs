using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class RiskScaleGroupConfig : IEntityTypeConfiguration<RiskScaleGroup>
    {
        public void Configure(EntityTypeBuilder<RiskScaleGroup> builder)
        {
            builder.ToTable("RiskScaleGroup");
            builder.HasKey(r => r.RiskScaleGroupId);
            
            builder.Property(a => a.RiskScaleGroupId)
                .HasDefaultValueSql("NEWID()");
                
            builder.Property(a => a.ScaleGroupId)
                .IsRequired();
                
            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(200);
                
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
                
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);
                
            builder.Property(a => a.CreatedBy)
                .HasMaxLength(120);
                
            builder.Property(a => a.CreationDate)
                .HasDefaultValueSql("GETDATE()");
                
            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(120);

            // Foreign key relationships
            builder.HasOne(a => a.ScaleGroup)
                .WithMany(sg => sg.RiskScaleGroups)
                .HasForeignKey(a => a.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}