using FluentValidation;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Validations
{
    public class AuditTemplateFieldValidator : AbstractValidator<AuditTemplateFieldRequestDto>
    {
        private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;
        public AuditTemplateFieldValidator(IAuditTemplateFieldRepository auditTemplateFieldRepository, Guid? id = null) 
        {
            _auditTemplateFieldRepository = auditTemplateFieldRepository;

            RuleFor(x => x.TableScaleTemplateId)
                .NotEmpty().WithMessage("La plantilla de tabla es requerida.")
                .NotNull().WithMessage("La plantilla de tabla es requerida.");
            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");
            RuleFor(x => x.FieldCode)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(100).WithMessage("El código no puede exceder los 100 caracteres.")
                .MustAsync(async (dto, code, cancellation) =>
                {
                    return !await _auditTemplateFieldRepository.ExistsByCodeAsync(code, id);
                })
            .WithMessage("El código ya existe.");
            RuleFor(x => x.FieldType)
                .MaximumLength(50).WithMessage("El tipo no puede exceder los 50 caracteres.")
                .Must(value => string.IsNullOrEmpty(value) || new[] { "numeric", "text", "date", "select" }.Contains(value))
                .WithMessage("El tipo debe ser uno de los siguientes valores: numeric, text, date, select.");
            RuleFor(x => x.IsCalculated);
            RuleFor(x => x.CalculationFormula)
                .MaximumLength(500).WithMessage("La formula no puede exceder los 500 caracteres.");
            RuleFor(x => x.AcumulationType)
                .MaximumLength(50).WithMessage("El tipo de acumulación no puede exceder los 50 caracteres.")
                .Must(value => string.IsNullOrEmpty(value) || new[] { "SUM", "COUNT" }.Contains(value))
                .WithMessage("El tipo de acumulación debe ser 'SUM' o 'COUNT'.");
            RuleFor(x => x.FieldOptions);
            RuleFor(x => x.DefaultValue);
        }
    }
}
