using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EnterpriseThemeConfig : IEntityTypeConfiguration<EnterpriseTheme>
    {
        public void Configure(EntityTypeBuilder<EnterpriseTheme> builder)
        {
            builder.ToTable("EnterpriseTheme");
            builder.HasKey(x => x.EnterpriseThemeId);

            // Logo
            builder.Property(x => x.LogoData);
            builder.Property(x => x.LogoContentType).HasMaxLength(100);
            builder.Property(x => x.LogoFileName).HasMaxLength(255);

            // Colores
            builder.Property(x => x.PrimaryColor).IsRequired().HasMaxLength(7).HasDefaultValue("#0066CC");
            builder.Property(x => x.SecondaryColor).IsRequired().HasMaxLength(7).HasDefaultValue("#333333");
            builder.Property(x => x.AccentColor).HasMaxLength(7);
            builder.Property(x => x.BackgroundColor).HasMaxLength(7);
            builder.Property(x => x.TextColor).HasMaxLength(7);

            builder.Property(x => x.IsActive).HasDefaultValue(true);

            // Auditoría
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(120);
            builder.Property(x => x.UpdatedBy).HasMaxLength(120);
            builder.Property(x => x.UpdateDate);

            // Relación uno a uno con Enterprise
            builder.HasOne(t => t.Enterprise)
                   .WithOne(e => e.Theme)
                   .HasForeignKey<EnterpriseTheme>(t => t.EnterpriseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índice único para garantizar un solo theme por enterprise
            builder.HasIndex(x => x.EnterpriseId).IsUnique();
        }
    }
}
