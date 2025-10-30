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

            builder.Property(x => x.UserId)
                .HasColumnName("UserId");

            builder.Property(x => x.Action)
                .HasMaxLength(100)
                .HasColumnName("Action");

            builder.Property(x => x.SequenceNumber)
                .HasColumnName("SequenceNumber")
                .IsRequired()
                .HasDefaultValue(0);

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

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
