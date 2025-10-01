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
            builder.Property(x => x.ObjectiveValue);
            builder.Property(x => x.RiskLow);
            builder.Property(x => x.RiskModerate);
            builder.Property(x => x.RiskHigh);
            builder.Property(x => x.Weighting);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdateDate);
        }
    }
}
