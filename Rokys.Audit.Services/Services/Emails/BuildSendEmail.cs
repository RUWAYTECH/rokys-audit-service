using Rokys.Audit.Model.Tables;
using Rokys.Audit.External.Services;
using static Rokys.Audit.Common.Constant.Constants;
using Scriban;

namespace Rokys.Audit.Services.Services.Emails
{
    public class BuildSendEmail
    {
        public static async Task NotifySupervisorOfNewAudit(IEmailService emailService, PeriodAudit audit)
        {
            try
            {
                var auditor = audit.ResponsibleAuditor;
                var supervisor = audit.Supervisor;
                var administrator = audit.Administrator;
                var inputTexts = new Dictionary<string, object>
                {
                    ["AdministratorFullName"] = $"{administrator.FirstName} {administrator.LastName}",
                    ["AuditorFullName"] = $"{audit.ResponsibleAuditor.FullName}",
                    ["SupervisorFullName"] = $"{supervisor.FullName}",
                    ["CorrelativeNumber"] = audit.CorrelativeNumber,
                    ["ExpirationDate"] = audit.EndDate,
                    ["StoreName"] = audit.Store.Name
                };


                var templateTextSupervisor = File.ReadAllText(MailTemplate.NotificationSupervisorNewAudit);
                var templateSupervisor = Template.Parse(templateTextSupervisor);
                var htmlBodySupervisor = templateSupervisor.Render(inputTexts);
                var emailsTo = new List<string>();
                if (!string.IsNullOrEmpty(supervisor.Email))
                {
                    emailsTo.Add(supervisor.Email);
                }
                if (!string.IsNullOrEmpty(administrator.Email))
                {
                    emailsTo.Add(administrator.Email);
                }
                await emailService.SendEmailAsync(emailsTo, "Nueva Auditoria para la tienda", htmlBodySupervisor, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
