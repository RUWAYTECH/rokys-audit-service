using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditParticipantConfig : IEntityTypeConfiguration<PeriodAuditParticipant>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditParticipant> builder)
        {
            builder.ToTable("PeriodAuditParticipant");

            builder.HasKey(p => p.PeriodAuditParticipantId);

            builder.Property(p => p.PeriodAuditParticipantId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.PeriodAuditId)
                .IsRequired();

            builder.Property(p => p.UserReferenceId)
                .IsRequired();

            builder.Property(p => p.RoleCodeSnapshot)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.RoleNameSnapshot)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Comments)
                .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Foreign key relationships
            builder.HasOne(p => p.PeriodAudit)
                .WithMany()
                .HasForeignKey(p => p.PeriodAuditId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.UserReference)
                .WithMany()
                .HasForeignKey(p => p.UserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Performance indexes
            builder.HasIndex(p => p.PeriodAuditId)
                .HasDatabaseName("IX_PeriodAuditParticipant_PeriodAuditId");

            builder.HasIndex(p => p.UserReferenceId)
                .HasDatabaseName("IX_PeriodAuditParticipant_UserReferenceId");
        }
    }
}