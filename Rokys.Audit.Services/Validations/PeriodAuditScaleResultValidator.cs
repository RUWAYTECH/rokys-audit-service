using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditScaleResultValidator : AbstractValidator<PeriodAuditScaleResultRequestDto>
    {
        public PeriodAuditScaleResultValidator()
        {
            RuleFor(x => x.PeriodAuditGroupResultId).NotEmpty();
            RuleFor(x => x.ScaleGroupId).NotEmpty();
            RuleFor(x => x.ScoreValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AppliedWeighting).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Observations).MaximumLength(500);
        }
    }
}
