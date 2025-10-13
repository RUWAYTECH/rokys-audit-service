using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditGroupResultValidator : AbstractValidator<PeriodAuditGroupResultRequestDto>
    {
        public PeriodAuditGroupResultValidator()
        {
            RuleFor(x => x.PeriodAuditId).NotEmpty();
            RuleFor(x => x.GroupId).NotEmpty();
            RuleFor(x => x.ScoreValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalWeighting).GreaterThanOrEqualTo(0);
        }
    }
}
