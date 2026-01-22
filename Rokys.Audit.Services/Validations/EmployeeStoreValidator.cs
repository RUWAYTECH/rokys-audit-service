using FluentValidation;
using Rokys.Audit.DTOs.Requests.EmployeeStore;

namespace Rokys.Audit.Services.Validations
{
    public class EmployeeStoreValidator : AbstractValidator<EmployeeStoreRequestDto>
    {
        public EmployeeStoreValidator() 
        {
            RuleFor(r => r.UserReferenceId)
                .NotEmpty().WithMessage("El Id del Usuario es requerido")
                .NotNull().WithMessage("El Id del Usuario no puede ser vació");
            RuleFor(r => r.StoreId)
                .NotEmpty().WithMessage("El Id de la Tienda es requerido")
                .NotNull().WithMessage("El Id de la Tienda no puede ser vació");
            RuleFor(r => r.AssignmentDate)
                .NotEmpty().WithMessage("La Fecha de Asignación es requerida")
                .NotNull().WithMessage("La Fecha de Asignación no puede ser vació");
        }
    }
}
