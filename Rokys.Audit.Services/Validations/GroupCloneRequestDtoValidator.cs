using FluentValidation;
using Rokys.Audit.DTOs.Requests.Group;

namespace Rokys.Audit.Services.Validations
{
    /// <summary>
    /// Validador para GroupCloneRequestDto
    /// </summary>
    public class GroupCloneRequestDtoValidator : AbstractValidator<GroupCloneRequestDto>
    {
        public GroupCloneRequestDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.EnterpriseId.HasValue || x.EnterpriseGroupingId.HasValue)
                .WithMessage("Debe especificar una Empresa o un Grupo de Empresas.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("El ID del grupo es requerido")
                .NotEqual(Guid.Empty)
                .WithMessage("El ID del grupo no puede ser un GUID vacío");
        }
    }
}