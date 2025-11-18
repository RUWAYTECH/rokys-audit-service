using Rokys.Audit.Model.Tables;
using Rokys.Audit.External.Services;
using static Rokys.Audit.Common.Constant.Constants;
using Scriban;
using Rokys.Audit.Common.Constant;

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
        public static async Task NotifyAllUserAudit(IEmailService emailService, PeriodAudit audit)
        {
            try
            {
                var templateText = File.ReadAllText(MailTemplate.NotificationSupervisorNewAudit);
                var template = Template.Parse(templateText);
                var auditor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code);
                var supervisor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code);

                // Filtra solo participantes activos y con email válido
                var participants = audit.PeriodAuditParticipants
                    .Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.UserReference?.Email))
                    .ToList();

                foreach (var participant in participants)
                {
                    var inputTexts = new Dictionary<string, object>
                    {
                        ["UserFullName"] = participant.UserReference?.FullName ?? string.Empty,
                        ["AuditorFullName"] = auditor?.UserReference?.FullName ?? string.Empty,
                        ["SupervisorFullName"] = supervisor?.UserReference?.FullName ?? string.Empty,
                        ["CorrelativeNumber"] = audit.CorrelativeNumber,
                        ["ExpirationDate"] = audit.EndDate,
                        ["StoreName"] = audit.Store?.Name ?? "Sin tienda"
                    };

                    var htmlBody = template.Render(inputTexts);
                    var email = participant.UserReference.Email ?? string.Empty;

                    await emailService.SendEmailAsync(new[] { email }, "Nueva Auditoría para la tienda", htmlBody, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
