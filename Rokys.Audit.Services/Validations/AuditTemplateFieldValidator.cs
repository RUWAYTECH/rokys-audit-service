using FluentValidation;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;

namespace Rokys.Audit.Services.Validations
{
    public class AuditTemplateFieldValidator : AbstractValidator<AuditTemplateFieldRequestDto>
    {
        public AuditTemplateFieldValidator() 
        {
            RuleFor(x => x.TableScaleTemplateId)
                .NotEmpty().WithMessage("La plantilla de tabla es requerida.")
                .NotNull().WithMessage("La plantilla de tabla es requerida.");
            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");
            RuleFor(x => x.FieldCode)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(100).WithMessage("El código no puede exceder los 100 caracteres.");
            RuleFor(x => x.FieldType)
                .NotEmpty().WithMessage("El tipo es requerido.")
                .MaximumLength(50).WithMessage("El tipo no puede exceder los 50 caracteres.");
            RuleFor(x => x.IsCalculated);
            RuleFor(x => x.CalculationFormula)
                .MaximumLength(500).WithMessage("La formula no puede exceder los 500 caracteres.");
            RuleFor(x => x.AcumulationType)
                .MaximumLength(50).WithMessage("El tipo de acumulación no puede exceder los 50 caracteres.");
            RuleFor(x => x.FieldOptions);
            RuleFor(x => x.DefaultValue);
        }
    }
}
