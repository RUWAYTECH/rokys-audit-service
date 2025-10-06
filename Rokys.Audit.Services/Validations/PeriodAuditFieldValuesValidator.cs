using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditFieldValuesValidator : AbstractValidator<PeriodAuditFieldValuesRequestDto>
    {
        public PeriodAuditFieldValuesValidator()
        {
            RuleFor(x => x.FieldCode).NotEmpty();
            RuleFor(x => x.FieldName).NotEmpty();
            RuleFor(x => x.FieldType).NotEmpty();
        }
    }
}