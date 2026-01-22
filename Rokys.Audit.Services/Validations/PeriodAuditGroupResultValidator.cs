using FluentValidation;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Validations
{
    public class PeriodAuditGroupResultValidator : AbstractValidator<PeriodAuditGroupResultRequestDto>
    {
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;
        public PeriodAuditGroupResultValidator(IPeriodAuditGroupResultRepository periodAuditGroupResultRepository, Guid? id = null)
        {
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
            RuleFor(x => x.PeriodAuditId).NotEmpty()
                .MustAsync(async (dto, periodAuditId, _) =>
                {
                    var exists = await _periodAuditGroupResultRepository.GetValidatorByGroupIdAsync(periodAuditId, dto.GroupId, id);
                    return !exists;
                }).WithMessage("No se puede insertar un grupo duplicado");
            RuleFor(x => x.GroupId).NotEmpty();
            RuleFor(x => x.ScoreValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalWeighting).GreaterThanOrEqualTo(0);
        }
    }
}
