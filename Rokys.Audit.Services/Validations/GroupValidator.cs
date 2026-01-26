using FluentValidation;
using Rokys.Audit.DTOs.Requests.Group;

namespace Rokys.Audit.Services.Validations
{
    public class GroupValidator : AbstractValidator<GroupRequestDto>
    {
        public GroupValidator() 
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("La descripción solo acepta como máximo 200 caracteres.")
                .NotNull().WithMessage("La descripción es requerida")
                .NotEmpty().WithMessage("La descripción es requerida");
            
            RuleFor(x => x.Weighting)
                .NotNull().WithMessage("El ponderador es requerido.");

            RuleFor(x => x.Code)
                .MaximumLength(10).WithMessage("El código solo acepta como máximo 10 caracteres.");

            RuleFor(x => x)
                .Must(x => x.EnterpriseId.HasValue || x.EnterpriseGroupingId.HasValue)
                .WithMessage("Debe especificar una Empresa o un Grupo de Empresas.");
        }
    }
}