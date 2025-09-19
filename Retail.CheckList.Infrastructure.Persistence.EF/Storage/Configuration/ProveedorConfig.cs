using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ProveedorConfig : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedor");
            builder.HasKey(r => r.IdProveedor);
            builder.Property(a => a.IdProveedor);
            builder.Property(a => a.RUC);
            builder.Property(a => a.RazonSocial);
        }
    }
}
