using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditTableScaleTemplateResultValidator : AbstractValidator<PeriodAuditTableScaleTemplateResultRequestDto>
    {
        public PeriodAuditTableScaleTemplateResultValidator()
        {
            RuleFor(x => x.PeriodAuditScaleResultId).NotEmpty();
            RuleFor(x => x.TableScaleTemplateId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Orientation).NotEmpty().Must(o => o == "H" || o == "V");
        }
    }
}
