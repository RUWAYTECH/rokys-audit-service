using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Storage.Configuration
{
    public class UserReferenceConfig : IEntityTypeConfiguration<UserReference>
    {
        public void Configure(EntityTypeBuilder<UserReference> builder)
        {
            builder.ToTable("UserReference");

            builder.HasKey(ur => ur.UserReferenceId);

            builder.Property(ur => ur.UserReferenceId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(ur => ur.UserId);

            builder.Property(ur => ur.EmployeeId);

            builder.Property(ur => ur.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ur => ur.LastName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ur => ur.Email)
                .HasMaxLength(150);

            builder.Property(ur => ur.PersonalEmail)
                .HasMaxLength(150);

            builder.Property(ur => ur.DocumentNumber)
                .HasMaxLength(20);

            builder.Property(ur => ur.RoleCode)
                .HasMaxLength(50);

            builder.Property(ur => ur.RoleName)
                .HasMaxLength(100);

            builder.Property(ur => ur.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ur => ur.CreatedBy)
                .HasMaxLength(100);

            builder.Property(ur => ur.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(ur => ur.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(ur => ur.UpdateDate);

            // Navegación - PeriodAudit como Administrador
            builder.HasMany(ur => ur.AdministratorAudits)
                .WithOne(pa => pa.Administrator)
                .HasForeignKey(pa => pa.AdministratorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - PeriodAudit como Asistente
            builder.HasMany(ur => ur.AssistantAudits)
                .WithOne(pa => pa.Assistant)
                .HasForeignKey(pa => pa.AssistantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - PeriodAudit como Gerente de Operación
            builder.HasMany(ur => ur.OperationManagerAudits)
                .WithOne(pa => pa.OperationManager)
                .HasForeignKey(pa => pa.OperationManagersId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - PeriodAudit como Administrador Suplente
            builder.HasMany(ur => ur.FloatingAdministratorAudits)
                .WithOne(pa => pa.FloatingAdministrator)
                .HasForeignKey(pa => pa.FloatingAdministratorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - PeriodAudit como Auditor Responsable
            builder.HasMany(ur => ur.ResponsibleAuditorAudits)
                .WithOne(pa => pa.ResponsibleAuditor)
                .HasForeignKey(pa => pa.ResponsibleAuditorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - Supervisores
            builder.HasMany(ur => ur.SupervisorAudits)
                .WithOne(pa => pa.Supervisor)
                .HasForeignKey(pa => pa.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Navegación - EmployeeStores
            builder.HasMany(ur => ur.EmployeeStores)
                .WithOne(es => es.UserReference)
                .HasForeignKey(es => es.UserReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(es => es.InboxItems)
                .WithOne(ii => ii.User)
                .HasForeignKey(ii => ii.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(ur => ur.UserId)
                .HasDatabaseName("IX_UserReference_UserId");

            builder.HasIndex(ur => ur.EmployeeId)
                .HasDatabaseName("IX_UserReference_EmployeeId");

            builder.HasIndex(ur => ur.DocumentNumber)
                .HasDatabaseName("IX_UserReference_DocumentNumber");

            builder.HasIndex(ur => ur.Email)
                .HasDatabaseName("IX_UserReference_Email");

            builder.HasIndex(ur => ur.PersonalEmail)
                .HasDatabaseName("IX_UserReference_PersonalEmail");

            builder.HasIndex(ur => new { ur.IsActive, ur.RoleCode })
                .HasDatabaseName("IX_UserReference_Active_Role");
        }
    }
}