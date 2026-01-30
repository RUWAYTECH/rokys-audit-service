using FluentValidation;
using Rokys.Audit.DTOs.Requests.SubScale;

namespace Rokys.Audit.Services.Validations
{
    public class SubScaleValidator : AbstractValidator<SubScaleRequestDto>
    {
        public SubScaleValidator()
        {
            RuleFor(x => x.EnterpriseGroupingId)
                .NotEmpty().WithMessage("La agrupación empresarial es requerida.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(10).WithMessage("El código acepta como máximo 10 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre acepta como máximo 100 caracteres.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("El valor es requerido.");

            RuleFor(x => x.ColorCode)
                .NotEmpty().WithMessage("El código de color es requerido.")
                .MaximumLength(10).WithMessage("El código de color acepta como máximo 10 caracteres.");
        }
    }
}
