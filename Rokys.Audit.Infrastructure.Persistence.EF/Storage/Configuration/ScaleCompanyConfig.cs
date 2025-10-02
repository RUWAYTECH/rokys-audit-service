using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ScaleCompanyConfig : IEntityTypeConfiguration<ScaleCompany>
    {
        public void Configure(EntityTypeBuilder<ScaleCompany> builder)
        {
            builder.ToTable("ScaleCompany");
            builder.HasKey(x => x.ScaleCompanyId);
            builder.Property(x => x.EnterpriseId).IsRequired();
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            
            // Objetivo general para la empresa
            builder.Property(x => x.ObjectiveValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            // Umbrales para la empresa
            builder.Property(x => x.RiskLow)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            builder.Property(x => x.RiskModerate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            builder.Property(x => x.RiskHigh)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            // Riesgo crítico = mayor a RiesgoElevado
            builder.Property(x => x.RiskCritical)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            
            // Ponderación de la empresa
            builder.Property(x => x.Weighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdateDate);

            // Navigation properties
            builder.HasOne(sc => sc.Enterprise)
                .WithMany(e => e.ScaleCompanies)
                .HasForeignKey(sc => sc.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sc => sc.EnterpriseId);
        }
    }
}
