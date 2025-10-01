using FluentValidation;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.Infrastructure.Repositories;
using System.Text.Json;

namespace Rokys.Audit.Services.Validations
{
    public class AuditScaleTemplateValidator : AbstractValidator<AuditScaleTemplateRequestDto>
    {
        private readonly IAuditScaleTemplateRepository _repository;

        public AuditScaleTemplateValidator(IAuditScaleTemplateRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código es requerido")
                .MaximumLength(50)
                .WithMessage("El código no debe exceder 50 caracteres")
                .MustAsync(BeUniqueCode)
                .WithMessage("Ya existe una plantilla con este código");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no debe exceder 200 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("La descripción no debe exceder 500 caracteres");

            RuleFor(x => x.TemplateData)
                .NotEmpty()
                .WithMessage("Los datos de la plantilla son requeridos")
                .Must(BeValidJson)
                .WithMessage("Los datos de la plantilla deben tener formato JSON válido");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _repository.ExistsByCodeAsync(code);
        }

        private bool BeValidJson(string? templateData)
        {
            if (string.IsNullOrWhiteSpace(templateData))
                return false;

            try
            {
                JsonDocument.Parse(templateData);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}