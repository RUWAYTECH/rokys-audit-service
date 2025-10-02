using FluentValidation;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using System.Text.Json;

namespace Rokys.Audit.Services.Validations
{
    public class TableScaleTemplateValidator : AbstractValidator<TableScaleTemplateRequestDto>
    {
        public TableScaleTemplateValidator()
        {
            RuleFor(x => x.ScaleGroupId)
                .NotEmpty().WithMessage("El ID del grupo de escala es requerido.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(50).WithMessage("El código no puede exceder los 50 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");

            RuleFor(x => x.Title)
                .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

            RuleFor(x => x.TemplateData)
                .Must(BeValidJson).WithMessage("Los datos de la plantilla deben ser un JSON válido.")
                .When(x => !string.IsNullOrEmpty(x.TemplateData));
        }

        private bool BeValidJson(string? templateData)
        {
            if (string.IsNullOrEmpty(templateData))
                return true;

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