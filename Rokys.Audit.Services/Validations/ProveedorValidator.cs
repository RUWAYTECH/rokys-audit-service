using FluentValidation;
using Rokys.Audit.DTOs.Requests.Proveedor;

namespace Rokys.Audit.Services.Validations
{
    public class ProveedorValidator: AbstractValidator<ProveedorRequestDto>
    {
        public ProveedorValidator()
        {
            RuleFor(r => r.RUC).NotEmpty().NotNull().MaximumLength(11);
            RuleFor(r => r.RazonSocial).NotEmpty().NotNull().MaximumLength(200);
        }

    }
}
