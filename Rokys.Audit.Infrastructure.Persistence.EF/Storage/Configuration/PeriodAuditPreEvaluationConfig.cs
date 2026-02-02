using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditPreEvaluationConfig : IEntityTypeConfiguration<PeriodAuditPreEvaluation>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditPreEvaluation> builder)
        {
            builder.ToTable("PeriodAuditPreEvaluation");
            builder.HasKey(x => x.PeriodAuditPreEvaluationId);
            builder.Property(x => x.TotalWeighted).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(x => x.ScaleValueJSON).HasColumnType("nvarchar(max)");
            builder.Property(x => x.TotalAcumulation).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.HasOne(x => x.PeriodAuditGroupResult)
                .WithMany()
                .HasForeignKey(x => x.PeriodAuditGroupResultId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
