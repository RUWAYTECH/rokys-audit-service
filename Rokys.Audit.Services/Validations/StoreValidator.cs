using FluentValidation;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Validations
{
    public class StoreValidator : AbstractValidator<StoreRequestDto>
    {
        public StoreValidator(IStoreRepository storeRepository)
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("El campo nombre es requerido")
                .NotNull().MaximumLength(255)
                .MustAsync(async (name, cancellation) =>
                {
                    return !await storeRepository.AnyAsync(r => r.Name == name, cancellation);
                })
                .WithMessage("El nombre de la tienda ya esta en uso");
            RuleFor(r => r.Code)
                .NotEmpty().WithMessage("El c�digo de la tienda es requerido")
                .MaximumLength(50)
                .MustAsync(async (code, cancellation) =>
                {
                    return !await storeRepository.AnyAsync(u => u.Code == code, cancellation);
                })
                .WithMessage("El Código de la tienda ya esta en uso");
            RuleFor(r => r.Address).MaximumLength(500);
            RuleFor(r => r.EnterpriseId)
                .NotEmpty().WithMessage("La Empresa es requerido")
                .NotNull().WithMessage("La Empresa no puede ser vaci�");
        }
    }
}
