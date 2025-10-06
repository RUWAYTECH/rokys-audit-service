using FluentValidation;
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;

namespace Rokys.Audit.Services.Validations
{
    public class CriteriaSubResultValidator : AbstractValidator<CriteriaSubResultRequestDto>
    {
        public CriteriaSubResultValidator()
        {
            RuleFor(x => x.CriteriaName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ColorCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        }
    }
}
