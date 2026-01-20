using FluentValidation;
using Rokys.Audit.DTOs.Requests.AuditRoleConfiguration;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Validations
{
    public class AuditRoleConfigurationValidator : AbstractValidator<AuditRoleConfigurationRequestDto>
    {
        private readonly IAuditRoleConfigurationRepository _auditRoleConfigurationRepository;
        
        public AuditRoleConfigurationValidator(IAuditRoleConfigurationRepository auditRoleConfigurationRepository, Guid? id = null)
        {
            _auditRoleConfigurationRepository = auditRoleConfigurationRepository;
            
            RuleFor(x => x.RoleCode)
                .NotEmpty().WithMessage("El código de rol es requerido.")
                .MaximumLength(10).WithMessage("El código de rol acepta como máximo 10 caracteres.")
                .MustAsync(async (dto, code, _) =>
                {
                    var exists = await _auditRoleConfigurationRepository.ExistsByRoleCodeAsync(code, dto.EnterpriseId, id);
                    return !exists;
                })
                .WithMessage("Ya existe una configuración de rol con este código.");
                
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre del rol es requerido.")
                .MaximumLength(100).WithMessage("El nombre del rol acepta como máximo 100 caracteres.");
                
            RuleFor(x => x.SequenceOrder)
                .GreaterThan(0).WithMessage("El orden de secuencia debe ser mayor a 0.")
                .When(x => x.SequenceOrder.HasValue)
                .MustAsync(async (dto, sequenceOrder, _) =>
                {
                    if (!sequenceOrder.HasValue) return true;
                    var exists = await _auditRoleConfigurationRepository.ExistsBySequenceOrderAsync(sequenceOrder.Value, dto.EnterpriseId, id);
                    return !exists;
                })
                .WithMessage("Ya existe una configuración de rol con este orden de secuencia.");
        }
    }
}