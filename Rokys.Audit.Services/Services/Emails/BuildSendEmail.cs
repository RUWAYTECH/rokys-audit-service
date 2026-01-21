using Rokys.Audit.Model.Tables;
using Rokys.Audit.External.Services;
using static Rokys.Audit.Common.Constant.Constants;
using Scriban;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Services.Services.Pdf;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Rokys.Audit.Services.Services.Emails
{
    public class BuildSendEmail
    {
        public static async Task NotifySupervisorOfNewAudit(IEmailService emailService, PeriodAudit audit, string urlApp)
        {
            try
            {
                var administrator = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code);
                var auditor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code);
                var supervisor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code);
                var headOperations = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeOperaciones.Code);

                var inputTexts = new Dictionary<string, object>
                {
                    ["AdministratorFullName"] = administrator?.UserReference?.FullName ?? string.Empty,
                    ["AuditorFullName"] = auditor?.UserReference?.FullName ?? string.Empty,
                    ["SupervisorFullName"] = supervisor?.UserReference?.FullName ?? string.Empty,
                    ["CorrelativeNumber"] = audit.CorrelativeNumber,
                    ["ExpirationDate"] = audit.EndDate,
                    ["StoreName"] = audit.Store?.Name ?? "Sin tienda",
                    ["UrlAplication"] = urlApp + "/store-audit/secure/manageaudits/" + audit.PeriodAuditId
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
            IAuditRoleConfigurationRepository auditRoleConfigurationRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository,
            Rokys.Audit.DTOs.Common.FileSettings fileSettings,
            string? storeEmail,
            string urlApp)
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
                    ["StoreName"] = audit.Store?.Name ?? "Sin tienda",
                    ["UrlAplication"] = urlApp + "/store-audit/secure/manageaudits/" + audit.PeriodAuditId
                };

                var htmlBody = template.Render(inputTexts);

                var emailsTo = participants
                    .Where(x => x.RoleCode != RoleCodes.StoreAdmin.Code)
                    .Select(x => x.Email)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                if (storeEmail != null)
                {
                    emailsTo.Add(storeEmail);
                }

                // Generar PDF con los datos de auditoría
                var auditData = await GetAuditDataForPdf(audit, periodAuditGroupResultRepository);
                var pdfBytes = await AuditPdfGenerator.GenerateAuditReportPdf(audit, auditData);

                var fileName = $"{audit.CorrelativeNumber}.pdf";

                // Guardar PDF en carpeta configurada (FileSettings.Path)
                var pdfFolderPath = Path.Combine(fileSettings.Path, "Reports", "Audits", audit.EndDate.Year.ToString());
                Directory.CreateDirectory(pdfFolderPath);
                var pdfFilePath = Path.Combine(pdfFolderPath, fileName);
                await File.WriteAllBytesAsync(pdfFilePath, pdfBytes);

                var attachments = new List<(string fileName, byte[] content)>
                {
                    (fileName, pdfBytes)
                };

                await emailService.SendEmailWithAttachmentsAsync(
                    emailsTo,
                    "Auditoría Finalizada para la tienda",
                    htmlBody,
                    attachments,
                    true
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en NotifyAllUserAudit: {ex.Message}", ex);
            }
        }

        private static async Task<List<(int nro, string proceso, string observations, string impact, string recommendation, string valorized)>> GetAuditDataForPdf(
            PeriodAudit audit,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository)
        {
            var auditData = new List<(int nro, string proceso, string observations, string impact, string recommendation, string valorized)>();

            // Obtener los resultados de grupo con sus resultados de escala INCLUYENDO las relaciones
            var groupResults = await periodAuditGroupResultRepository.GetByPeriodAuditIdWithScaleResultsAsync(audit.PeriodAuditId);

            int nro = 1;
            foreach (var groupResult in groupResults)
            {
                if (groupResult.PeriodAuditScaleResults != null)
                {
                    // Ordenar por SortOrder para mantener el orden correcto
                    foreach (var scaleResult in groupResult.PeriodAuditScaleResults
                        .Where(sr => sr.IsActive)
                        .OrderBy(sr => sr.SortOrder))
                    {

                        bool haveRecommendation = !string.IsNullOrWhiteSpace(scaleResult.Recommendation);
                        bool haveObservation = !string.IsNullOrWhiteSpace(scaleResult.Observations);
                        bool haveValor = decimal.TryParse(scaleResult.Valorized, out var valor) && valor > 0;
                        bool haveImpact = !string.IsNullOrWhiteSpace(scaleResult.Impact);
                        if (!haveRecommendation && !haveValor && !haveImpact && !haveObservation)
                            continue;

                        auditData.Add((
                            nro++,
                            scaleResult.ScaleGroup?.Name ?? "N/A",
                            scaleResult.Observations ?? "",
                            scaleResult.Impact ?? "",
                            scaleResult.Recommendation ?? "",
                            scaleResult.Valorized ?? ""
                        ));
                    }
                }
            }

            return auditData;
        }

        public static async Task NotifyActionPlanCompleted(IEmailService emailService, PeriodAudit audit, string urlApp)
        {
            try
            {
                var jefeDeArea = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code);
                var auditor = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code);
                var gerenteOperaciones = audit.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeOperaciones.Code);

                var inputTexts = new Dictionary<string, object>
                {
                    ["CorrelativeNumber"] = audit.CorrelativeNumber,
                    ["StoreName"] = audit.Store?.Name ?? "Sin tienda",
                    ["UrlAplication"] = urlApp + "/store-audit/secure/manageaudits/" + audit.PeriodAuditId + "/action-plan"
                };

                var templateText = File.ReadAllText(MailTemplate.NotificationActionPlanCompleted);
                var template = Template.Parse(templateText);
                var htmlBody = template.Render(inputTexts);

                var emailsTo = new List<string>();

                if (!string.IsNullOrEmpty(jefeDeArea?.UserReference?.Email))
                    emailsTo.Add(jefeDeArea.UserReference.Email);

                if (!string.IsNullOrEmpty(auditor?.UserReference?.Email))
                    emailsTo.Add(auditor.UserReference.Email);

                if (!string.IsNullOrEmpty(gerenteOperaciones?.UserReference?.Email))
                    emailsTo.Add(gerenteOperaciones.UserReference.Email);

                if (emailsTo.Any())
                {
                    await emailService.SendEmailAsync(emailsTo, "Planes de Acción Completados", htmlBody, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en NotifyActionPlanCompleted: {ex.Message}", ex);
            }
        }

    }
}
