using FluentValidation;
using Rokys.Audit.DTOs.Requests.AuditStatus;

namespace Rokys.Audit.Services.Validations
{
    public class AuditStatusValidator : AbstractValidator<AuditStatusRequestDto>
    {
        public AuditStatusValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El campo 'Name' es obligatorio.")
                .MaximumLength(255).WithMessage("El campo 'Name' no debe exceder los 255 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El campo 'Code' es obligatorio.")
                .MaximumLength(10).WithMessage("El campo 'Code' no debe exceder los 10 caracteres.");

            RuleFor(x => x.ColorCode)
                .MaximumLength(10).WithMessage("El campo 'ColorCode' no debe exceder los 10 caracteres.");
        }
    }
}
