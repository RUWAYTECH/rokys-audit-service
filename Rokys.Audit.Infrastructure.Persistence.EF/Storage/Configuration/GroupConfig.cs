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
                
            builder.Property(a => a.ObjectiveValue)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.RiskLow)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.RiskModerate)
                .HasColumnType("decimal(10,2)");
                
            builder.Property(a => a.RiskHigh)
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
        }
    }
}