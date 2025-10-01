using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScaleCompany;

namespace Rokys.Audit.Services.Validations
{
    public class ScaleCompanyValidator : AbstractValidator<ScaleCompanyRequestDto>
    {
        public ScaleCompanyValidator() 
        {
            RuleFor(x => x.EnterpriseId).NotEmpty().WithMessage("La empresa es requerida.")
                .NotNull().WithMessage("La empresa es requerida.");
            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("La descripción solo acepta como máximo 250 caracteres.")
                .NotNull().WithMessage("La descripción es requerida")
                .NotEmpty().WithMessage("La descripción es requerida");
            RuleFor(x => x.ObjectiveValue).NotNull().WithMessage("El valor objetivo es requerido.")
                .NotEmpty().WithMessage("El valor objetivo es requerido");
            RuleFor(x => x.RiskLow).NotNull().WithMessage("El riesgo bajo es requerido.")
                .NotEmpty().WithMessage("El riesgo bajo es requerido");
            RuleFor(x => x.RiskModerate).NotNull().WithMessage("El riesgo moderado es requerido.")
                .NotEmpty().WithMessage("El riesgo moderado es requerido");
            RuleFor(x => x.RiskHigh).NotNull().WithMessage("El riesgo alto es requerido.")
                .NotEmpty().WithMessage("El riesgo alto es requerido");
            RuleFor(x => x.Weighting).NotNull().WithMessage("El ponderación es requerido.")
                .NotEmpty().WithMessage("El ponderación es requerido");
        }
    }
}
