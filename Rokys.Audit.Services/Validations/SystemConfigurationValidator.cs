using FluentValidation;
using Rokys.Audit.DTOs.Requests.SystemConfiguration;

namespace Rokys.Audit.Services.Validations
{
    public class SystemConfigurationValidator : AbstractValidator<SystemConfigurationRequestDto>
    {
        public SystemConfigurationValidator()
        {
            RuleFor(r => r.ConfigKey)
                .NotEmpty().WithMessage("La Llave de Configuración es requerida")
                .NotNull().WithMessage("La Llave de Configuración no puede ser vacía")
                .MaximumLength(100);
            
            RuleFor(r => r.DataType)
                .MaximumLength(50);
            
            RuleFor(r => r.Description)
                .MaximumLength(500);
            
            RuleFor(r => r.ReferenceType)
                .MaximumLength(50);
            
            RuleFor(r => r.ReferenceCode)
                .MaximumLength(50);
        }
    }
}
