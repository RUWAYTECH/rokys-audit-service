using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScaleGroup;

namespace Rokys.Audit.Services.Validations
{
    public class ScaleGroupValidator : AbstractValidator<ScaleGroupRequestDto>
    {
        public ScaleGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("El grupo es requerido.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(10).WithMessage("El código acepta como máximo 10 caracteres.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(200).WithMessage("El nombre acepta como máximo 200 caracteres.");
   
            RuleFor(x => x.Weighting)
                .NotNull().WithMessage("La ponderación es requerida.");
        }
    }
}