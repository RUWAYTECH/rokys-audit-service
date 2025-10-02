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
            builder.HasKey(r => r.GroupId);
            
            builder.Property(a => a.GroupId)
                .HasDefaultValueSql("NEWID()");
                
            builder.Property(a => a.EnterpriseId)
                .IsRequired();
                
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            // Objetivo general para el grupo
            builder.Property(a => a.ObjectiveValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            // Umbrales para el grupo
            builder.Property(a => a.RiskLow)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.RiskModerate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.RiskHigh)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            // Riesgo crítico = mayor a RiesgoElevado
            builder.Property(a => a.RiskCritical)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            // Ponderación del grupo
            builder.Property(a => a.Weighting)
                .IsRequired()
                .HasColumnType("decimal(5,2)");
                
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);
                
            builder.Property(a => a.CreatedBy)
                .HasMaxLength(120);
                
            builder.Property(a => a.CreationDate)
                .HasDefaultValueSql("GETDATE()");
                
            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(120);

            // Navigation properties
            builder.HasOne(g => g.Enterprise)
                .WithMany(e => e.Groups)
                .HasForeignKey(g => g.EnterpriseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.ScaleGroups)
                .WithOne(sg => sg.Group)
                .HasForeignKey(sg => sg.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.PeriodAuditResults)
                .WithOne(par => par.Group)
                .HasForeignKey(par => par.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(g => g.EnterpriseId);
        }
    }
}