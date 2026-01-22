using DocumentFormat.OpenXml.Office2010.Excel;
using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditScaleResultValidator : AbstractValidator<PeriodAuditScaleResultRequestDto>
    {
        private readonly IPeriodAuditScaleResultRepository _periodAuditScaleResultRepository;
        public PeriodAuditScaleResultValidator(IPeriodAuditScaleResultRepository periodAuditScaleResultRepository, Guid? id = null)
        {
            _periodAuditScaleResultRepository = periodAuditScaleResultRepository;
            RuleFor(x => x.PeriodAuditGroupResultId).NotEmpty()
                .MustAsync(async (dto, periodAuditId, _) =>
                {
                    var exists = await _periodAuditScaleResultRepository.GetValidatorByScaleGroupIdAsync(periodAuditId, dto.ScaleGroupId, id);
                    return !exists;
                }).WithMessage("No se puede insertar un grupo duplicado"); ;
            RuleFor(x => x.ScaleGroupId).NotEmpty();
            RuleFor(x => x.ScoreValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AppliedWeighting).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Observations).MaximumLength(500);
            
        }
    }
}
