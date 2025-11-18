using FluentValidation;
using Rokys.Audit.DTOs.Requests.SubstitutionHistory;

namespace Rokys.Audit.Services.Validations
{
    public class SubstitutionHistoryValidator : AbstractValidator<SubstitutionHistoryRequestDto>
    {
        public SubstitutionHistoryValidator()
        {
            RuleFor(x => x.PeriodAuditId)
                .NotEmpty().WithMessage("El ID de la auditoría es requerido.")
                .NotNull().WithMessage("El ID de la auditoría es requerido.");

            RuleFor(x => x.PreviousUserReferenceId)
                .NotEmpty().WithMessage("El usuario anterior es requerido para realizar una suplencia.")
                .NotNull().WithMessage("El usuario anterior es requerido para realizar una suplencia.");

            RuleFor(x => x.NewUserReferenceId)
                .NotEmpty().WithMessage("El nuevo usuario es requerido.")
                .NotNull().WithMessage("El nuevo usuario es requerido.");

            RuleFor(x => x.ChangeReason)
                .MaximumLength(255).WithMessage("La razón del cambio no puede exceder 255 caracteres.");

            // Validación: el usuario anterior debe ser diferente al nuevo
            RuleFor(x => x)
                .Must(x => x.PreviousUserReferenceId != x.NewUserReferenceId)
                .WithMessage("El usuario anterior y el nuevo usuario deben ser diferentes.");
        }
    }
}
