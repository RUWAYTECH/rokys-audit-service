using FluentValidation;
using Rokys.Audit.DTOs.Requests.MaintenanceTable;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Services.Validations
{
    public class MaintenanceTableValidator : AbstractValidator<MaintenanceTableRequestDto>
    {
        public MaintenanceTableValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El campo 'Code' es obligatorio.")
                .MaximumLength(50).WithMessage("El campo 'Code' no debe exceder los 50 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("El campo 'Description' no debe exceder los 255 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.IsSystem)
                .NotNull().WithMessage("El campo 'IsSystem' es obligatorio.");
        }
    }
}
