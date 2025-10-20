using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;

namespace Rokys.Audit.Services.Validations
{
    public class ScoringCriteriaValidator : AbstractValidator<ScoringCriteriaRequestDto>
    {
        public ScoringCriteriaValidator()
        {
            RuleFor(x => x.ScaleGroupId)
                .NotEmpty().WithMessage("El campo 'ScaleGroupId' es obligatorio.");

            RuleFor(x => x.CriteriaName)
                .NotEmpty().WithMessage("El campo 'CriteriaName' es obligatorio.")
                .MaximumLength(255).WithMessage("El campo 'CriteriaName' no debe exceder los 255 caracteres.");

            RuleFor(x => x.ComparisonOperator)
                .NotEmpty().WithMessage("El campo 'ComparisonOperator' es obligatorio.")
                .MaximumLength(20).WithMessage("El campo 'ComparisonOperator' no debe exceder los 20 caracteres.")
                .Must(op => new[] { "=", "!=", ">", "<", ">=", "<=", "BETWEEN", "IN", "CONTAINS" }
                    .Contains(op))
                .WithMessage("El valor de 'ComparisonOperator' no es válido.");

            RuleFor(x => x.ExpectedValue)
                .NotEmpty().WithMessage("El campo 'ExpectedValue' es obligatorio.")
                .MaximumLength(255).WithMessage("El campo 'ExpectedValue' no debe exceder los 255 caracteres.");

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0).WithMessage("El puntaje debe ser mayor o igual a 0.");

            RuleFor(x => x.ResultFormula)
                .MaximumLength(500).WithMessage("El campo 'ResultFormula' no debe exceder los 500 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.ResultFormula));

            RuleFor(x => x.ErrorMessage)
                .MaximumLength(500).WithMessage("El campo 'ErrorMessage' no debe exceder los 500 caracteres.");

            RuleFor(x => x.SuccessMessage)
                .MaximumLength(500).WithMessage("El campo 'SuccessMessage' no debe exceder los 500 caracteres.");
        }
    }
}
