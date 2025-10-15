using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class EmployeeStoreConfig : IEntityTypeConfiguration<EmployeeStore>
    {
        public void Configure(EntityTypeBuilder<EmployeeStore> builder)
        {
            builder.ToTable("EmployeeStores");

            builder.HasKey(es => es.EmployeeStoreId);

            builder.Property(es => es.EmployeeStoreId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(es => es.UserReferenceId)
                .IsRequired();

            builder.Property(es => es.StoreId)
                .IsRequired();

            builder.Property(es => es.AssignmentDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(es => es.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(es => es.CreatedBy)
                .HasMaxLength(100);

            builder.Property(es => es.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(es => es.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(es => es.UpdateDate);

            // Relación con UserReference
            builder.HasOne(es => es.UserReference)
                .WithMany(ur => ur.EmployeeStores)
                .HasForeignKey(es => es.UserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con Store
            builder.HasOne(es => es.Store)
                .WithMany()
                .HasForeignKey(es => es.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(es => es.UserReferenceId)
                .HasDatabaseName("IX_EmployeeStore_UserReferenceId");

            builder.HasIndex(es => es.StoreId)
                .HasDatabaseName("IX_EmployeeStore_StoreId");

            builder.HasIndex(es => new { es.UserReferenceId, es.StoreId })
                .IsUnique()
                .HasDatabaseName("UQ_EmployeeStore");

            builder.HasIndex(es => new { es.IsActive, es.StoreId })
                .HasDatabaseName("IX_EmployeeStore_Active_Store");
        }
    }
}