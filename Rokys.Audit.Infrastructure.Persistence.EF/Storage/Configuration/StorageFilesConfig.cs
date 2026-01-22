using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class StorageFilesConfig : IEntityTypeConfiguration<StorageFiles>
    {
        public void Configure(EntityTypeBuilder<StorageFiles> builder)
        {
            builder.ToTable("StorageFiles");

            builder.HasKey(x => x.StorageFileId);

            builder.Property(x => x.StorageFileId)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.OriginalName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ClassificationType)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.EntityId)
                .IsRequired();

            builder.Property(x => x.EntityName)
                .HasMaxLength(200);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.FileType)
                .HasMaxLength(50);

            builder.Property(x => x.UploadedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UploadDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

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