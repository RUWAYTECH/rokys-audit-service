using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class PeriodAuditActionPlanConfig : IEntityTypeConfiguration<PeriodAuditActionPlan>
    {
        public void Configure(EntityTypeBuilder<PeriodAuditActionPlan> builder)
        {
            builder.ToTable("PeriodAuditActionPlan");
            builder.HasKey(x => x.PeriodAuditActionPlanId);

            builder.Property(x => x.PeriodAuditActionPlanId)
                .HasColumnName("PeriodAuditActionPlanId")
                .IsRequired();

            builder.Property(x => x.DisiplinaryMeasureTypeId)
                .HasColumnName("DisiplinaryMeasureTypeId")
                .IsRequired();

            builder.Property(x => x.ResponsibleUserId)
                .HasColumnName("ResponsibleUserId")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired();

            builder.Property(x => x.DueDate)
                .HasColumnName("DueDate")
                .IsRequired();

            builder.Property(x => x.ApplyMeasure)
                .HasColumnName("ApplyMeasure")
                .IsRequired();

             // Audit properties
            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.CreationDate)
                .HasColumnName("CreationDate")
                .IsRequired();
            builder.Property(x => x.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .HasMaxLength(100)
                .IsRequired(false);
            builder.Property(x => x.UpdateDate)
                .HasColumnName("UpdateDate")
                .IsRequired(false);

            //Relaciones corregidas
            /* builder.HasOne(x => x.PeriodAuditScaleResult)
                .WithOne(pa => pa.PeriodAuditActionPlan)
                .HasForeignKey(x => x.PeriodAuditActionPlanId)
                .OnDelete(DeleteBehavior.Restrict); */

            builder.HasOne(x => x.DisiplinaryMeasureType)
                .WithMany(g => g.PeriodAuditActionPlans)
                .HasForeignKey(x => x.DisiplinaryMeasureTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ResponsibleUser)
                .WithMany(g => g.PeriodAuditActionPlans)
                .HasForeignKey(x => x.ResponsibleUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
