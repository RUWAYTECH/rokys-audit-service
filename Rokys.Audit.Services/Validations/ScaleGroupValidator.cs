using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScaleGroup;

namespace Rokys.Audit.Services.Validations
{
    public class ScaleGroupValidator : AbstractValidator<ScaleGroupRequestDto>
    {
        public ScaleGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(250).WithMessage("El nombre no puede exceder los 250 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(50).WithMessage("El código no puede exceder los 50 caracteres.");

            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("El grupo es requerido.");

            RuleFor(x => x.LowRisk)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo bajo debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El riesgo bajo no puede ser mayor a 100.");

            RuleFor(x => x.ModerateRisk)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo moderado debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El riesgo moderado no puede ser mayor a 100.");

            RuleFor(x => x.HighRisk)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo alto debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El riesgo alto no puede ser mayor a 100.");

            RuleFor(x => x.RiskCritical)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo crítico debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El riesgo crítico no puede ser mayor a 100.");

            RuleFor(x => x)
                .Must(x => x.LowRisk + x.ModerateRisk + x.HighRisk +x.RiskCritical == 100)
                .WithMessage("La suma de todos los niveles de riesgo debe ser igual a 100.");

        }
    }
}