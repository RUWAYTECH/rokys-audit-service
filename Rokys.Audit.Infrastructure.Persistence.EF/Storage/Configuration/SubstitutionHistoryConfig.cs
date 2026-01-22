using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class SubstitutionHistoryConfig : IEntityTypeConfiguration<SubstitutionHistory>
    {
        public void Configure(EntityTypeBuilder<SubstitutionHistory> builder)
        {
            builder.ToTable("SubstitutionHistory");

            builder.HasKey(s => s.SubstitutionHistoryId);

            builder.Property(s => s.SubstitutionHistoryId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(s => s.PeriodAuditId)
                .IsRequired();

            builder.Property(s => s.RoleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.PreviousUserReferenceId)
                .IsRequired();

            builder.Property(s => s.NewUserReferenceId)
                .IsRequired();

            builder.Property(s => s.ChangeReason)
                .HasMaxLength(255);

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Foreign key relationships
            builder.HasOne(s => s.PeriodAudit)
                .WithMany()
                .HasForeignKey(s => s.PeriodAuditId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PreviousUserReference)
                .WithMany()
                .HasForeignKey(s => s.PreviousUserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.NewUserReference)
                .WithMany()
                .HasForeignKey(s => s.NewUserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Performance indexes
            builder.HasIndex(s => s.PeriodAuditId)
                .HasDatabaseName("IX_SubstitutionHistory_PeriodAuditId");

            builder.HasIndex(s => s.PreviousUserReferenceId)
                .HasDatabaseName("IX_SubstitutionHistory_PreviousUserReferenceId");

            builder.HasIndex(s => s.NewUserReferenceId)
                .HasDatabaseName("IX_SubstitutionHistory_NewUserReferenceId");
        }
    }
}
