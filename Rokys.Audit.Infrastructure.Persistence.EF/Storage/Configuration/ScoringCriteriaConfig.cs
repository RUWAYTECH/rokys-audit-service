using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class ScoringCriteriaConfig : IEntityTypeConfiguration<ScoringCriteria>
    {
        public void Configure(EntityTypeBuilder<ScoringCriteria> builder)
        {
            builder.ToTable("ScoringCriteria");

            builder.HasKey(x => x.ScoringCriteriaId);

            builder.Property(x => x.ScoringCriteriaId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ScaleGroupId)
                .IsRequired();

            builder.Property(x => x.CriteriaCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.CriteriaName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ResultFormula)
                .HasMaxLength(500);

            builder.Property(x => x.ComparisonOperator)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.ExpectedValue)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Score)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            builder.Property(x => x.SuccessMessage)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.CreationDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(120);

            builder.Property(x => x.UpdateDate)
                .HasColumnType("datetime2");

            builder.HasOne(x => x.ScaleGroup)
                .WithMany(sg => sg.ScoringCriteria)
                .HasForeignKey(x => x.ScaleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ScaleGroupId)
                .HasDatabaseName("IX_ScoringCriteria_ScaleGroupId");
        }
    }
}
