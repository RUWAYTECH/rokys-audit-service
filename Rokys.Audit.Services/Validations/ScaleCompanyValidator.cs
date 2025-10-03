using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScaleCompany;

namespace Rokys.Audit.Services.Validations
{
    public class ScaleCompanyValidator : AbstractValidator<ScaleCompanyRequestDto>
    {
        public ScaleCompanyValidator() 
        {
            RuleFor(x => x.EnterpriseId)
                .NotEmpty().WithMessage("La empresa es requerida.")
                .NotNull().WithMessage("La empresa es requerida.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(50).WithMessage("El código acepta como máximo 50 caracteres.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre acepta como máximo 100 caracteres.");
            RuleFor(x => x.MinValue)
                .NotNull().WithMessage("El valor mínimo es requerido.");
            RuleFor(x => x.MaxValue)
                .NotNull().WithMessage("El valor máximo es requerido.");
            RuleFor(x => x.ColorCode)
                .MaximumLength(20).WithMessage("El código de color acepta como máximo 20 caracteres.");
            RuleFor(x => x.Icon)
                .MaximumLength(100).WithMessage("El icono acepta como máximo 100 caracteres.");
            RuleFor(x => x.SortOrder)
                .NotNull().WithMessage("El orden es requerido.");
        }
    }
}
