using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class InboxItemsConfig : IEntityTypeConfiguration<InboxItems>
    {
        public void Configure(EntityTypeBuilder<InboxItems> builder)
        {
            builder.ToTable("InboxItems");

            builder.HasKey(x => x.InboxItemId);

            builder.Property(x => x.InboxItemId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.Comments)
                .HasColumnType("nvarchar(max)");

            // DueDate and Priority removed from InboxItems

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
        }
    }
}
