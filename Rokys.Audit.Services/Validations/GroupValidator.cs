using FluentValidation;
using Rokys.Audit.DTOs.Requests.Group;

namespace Rokys.Audit.Services.Validations
{
    public class GroupValidator : AbstractValidator<GroupRequestDto>
    {
        public GroupValidator() 
        {
            RuleFor(x => x.EnterpriseId).NotEmpty().WithMessage("La empresa es requerida.")
                .NotNull().WithMessage("La empresa es requerida.");
            
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("La descripción solo acepta como máximo 200 caracteres.")
                .NotNull().WithMessage("La descripción es requerida")
                .NotEmpty().WithMessage("La descripción es requerida");
            RuleFor(x => x.Weighting)
                .NotEmpty().WithMessage("El ponderador es requerido.")
                .NotNull().WithMessage("El ponderador es requerido.")
                .GreaterThan(0).WithMessage("El ponderador debe ser mayor a 0.");
        }
    }
}