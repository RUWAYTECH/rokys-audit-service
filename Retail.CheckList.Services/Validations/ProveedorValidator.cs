using FluentValidation;
using Retail.CheckList.DTOs.Requests.Proveedor;

namespace Retail.CheckList.Services.Validations
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
