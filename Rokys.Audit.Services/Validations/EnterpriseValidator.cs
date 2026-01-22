using FluentValidation;
using Rokys.Audit.DTOs.Requests.Enterprise;

namespace Rokys.Audit.Services.Validations
{
    public class EnterpriseValidator : AbstractValidator<EnterpriseRequestDto>
    {
        public EnterpriseValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("El Nombre es requerido")
                .NotNull().WithMessage("El Nombre no puede ser vació")
                .MaximumLength(255);
            RuleFor(r => r.Code)
                .NotEmpty().WithMessage("El Código es requerido")
                .NotNull().WithMessage("El Código no puede ser vació")
                .MaximumLength(50);
            RuleFor(r => r.Address).MaximumLength(500);
        }
    }
}
