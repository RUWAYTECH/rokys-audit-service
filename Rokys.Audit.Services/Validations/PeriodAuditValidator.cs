using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAudit;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditValidator : AbstractValidator<PeriodAuditRequestDto>
    {
        public PeriodAuditValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.StatusId).NotEmpty();
            RuleFor(x => x.ScaleName).NotEmpty();
        }
    }
}