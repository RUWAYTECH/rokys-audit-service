using Rokys.Audit.Model.Tables;
using Rokys.Audit.External.Services;
using static Rokys.Audit.Common.Constant.Constants;
using Scriban;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Services.Emails
{
    public class BuildSendEmail
    {
        public static async Task NotifySupervisorOfNewAudit(IEmailService emailService, PeriodAudit audit)
        {
            try
            {                
                var administrator =audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code);
                var auditor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code);
                var supervisor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code);
                var assistantAudit = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.AsistenteAudit.Code);
                var headOperations = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeOperaciones.Code);

                var inputTexts = new Dictionary<string, object>
                {
                    ["AdministratorFullName"] = administrator?.UserReference?.FullName ?? string.Empty,
                    ["AuditorFullName"] = auditor?.UserReference?.FullName ?? string.Empty,
                    ["SupervisorFullName"] = supervisor?.UserReference?.FullName ?? string.Empty,
                    ["CorrelativeNumber"] = audit.CorrelativeNumber,
                    ["ExpirationDate"] = audit.EndDate,
                    ["StoreName"] = audit.Store?.Name ?? "Sin tienda"
                };

                var templateTextSupervisor = File.ReadAllText(MailTemplate.NotificationSupervisorNewAudit);
                var templateSupervisor = Template.Parse(templateTextSupervisor);
                var htmlBodySupervisor = templateSupervisor.Render(inputTexts);
                var emailsTo = new List<string>
                {
                    administrator.UserReference?.Email ?? string.Empty,
                    auditor.UserReference?.Email ?? string.Empty,
                    supervisor.UserReference?.Email ?? string.Empty
                };
                
                await emailService.SendEmailAsync(emailsTo, "Nueva Auditoria para la tienda", htmlBodySupervisor, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task NotifyAllUserAudit(
            IEmailService emailService,
            PeriodAudit audit,
            IAuditRoleConfigurationRepository auditRoleConfigurationRepository)
        {
            try
            {
                var roles = await auditRoleConfigurationRepository.GetAllAsync();
                var participants = audit.PeriodAuditParticipants
                    .Select(p => new
                    {
                        RoleCode = p.RoleCodeSnapshot,
                        RoleName = roles
                            .FirstOrDefault(r => r.RoleCode == p.RoleCodeSnapshot)?
                            .RoleName ?? "Sin Rol",
                        Sequence = roles
                            .FirstOrDefault(r => r.RoleCode == p.RoleCodeSnapshot)?
                            .SequenceOrder ?? 999, // fallback al final
                        FullName = p.UserReference?.FullName ?? "Sin Nombre",
                        Email = p.UserReference?.Email ?? string.Empty
                    })
                    .OrderBy(p => p.Sequence) // Orden por la configuración del rol
                    .ToList();

                var participantsHtml = string.Join("", participants.Select(p =>
                    $"<li><strong>{p.RoleName}:</strong> {p.FullName}</li>"
                ));

                var templateText = File.ReadAllText(MailTemplate.NotificationAllUserEndAudit);
                var template = Template.Parse(templateText);

                var inputTexts = new Dictionary<string, object>
                {
                    ["ParticipantsListHtml"] = participantsHtml,
                    ["CorrelativeNumber"] = audit.CorrelativeNumber,
                    ["ExpirationDate"] = audit.EndDate,
                    ["StoreName"] = audit.Store?.Name ?? "Sin tienda"
                };

                var htmlBody = template.Render(inputTexts);

                var emailsTo = participants
                    .Select(x => x.Email)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                await emailService.SendEmailAsync(
                    emailsTo,
                    "Auditoría Finalizada para la tienda",
                    htmlBody,
                    true
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en NotifyAllUserAudit: {ex.Message}", ex);
            }
        }

    }
}
