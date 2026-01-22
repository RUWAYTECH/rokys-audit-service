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
            RuleFor(x => x.EnterpriseId)
                .NotEmpty()
                .WithMessage("El ID de la empresa es requerido")
                .NotEqual(Guid.Empty)
                .WithMessage("El ID de la empresa no puede ser un GUID vacío");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("El ID del grupo es requerido")
                .NotEqual(Guid.Empty)
                .WithMessage("El ID del grupo no puede ser un GUID vacío");
        }
    }
}