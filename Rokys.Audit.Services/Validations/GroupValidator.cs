using FluentValidation;
using Rokys.Audit.DTOs.Requests.Group;

namespace Rokys.Audit.Services.Validations
{
    public class GroupValidator : AbstractValidator<GroupRequestDto>
    {
        public GroupValidator() 
        {
            RuleFor(x => x.EnterpriseId).NotEmpty().WithMessage("La empresa es requerida.")
                .NotNull().WithMessage("La empresa es requerida.");
            
            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("La descripción solo acepta como máximo 200 caracteres.")
                .NotNull().WithMessage("La descripción es requerida")
                .NotEmpty().WithMessage("La descripción es requerida");
                
            RuleFor(x => x.ObjectiveValue)
                .GreaterThanOrEqualTo(0).WithMessage("El valor objetivo debe ser mayor o igual a 0.")
                .When(x => x.ObjectiveValue.HasValue);
            
            RuleFor(x => x.RiskLow)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo bajo debe ser mayor o igual a 0.")
                .When(x => x.RiskLow.HasValue);
            
            RuleFor(x => x.RiskModerate)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo moderado debe ser mayor o igual a 0.")
                .When(x => x.RiskModerate.HasValue);
            
            RuleFor(x => x.RiskHigh)
                .GreaterThanOrEqualTo(0).WithMessage("El riesgo alto debe ser mayor o igual a 0.")
                .When(x => x.RiskHigh.HasValue);
            
            RuleFor(x => x.GroupWeight)
                .GreaterThanOrEqualTo(0).WithMessage("La ponderación del grupo debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("La ponderación del grupo debe ser menor o igual a 100.")
                .When(x => x.GroupWeight.HasValue);
        }
    }
}