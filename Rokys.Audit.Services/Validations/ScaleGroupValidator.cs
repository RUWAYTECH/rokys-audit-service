using FluentValidation;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.Infrastructure.Repositories;
using System.Security.Cryptography;

namespace Rokys.Audit.Services.Validations
{
    public class ScaleGroupValidator : AbstractValidator<ScaleGroupRequestDto>
    {
        private readonly IScaleGroupRepository _scaleGroupRepository;
        public ScaleGroupValidator(IScaleGroupRepository scaleGroupRepository, Guid? id = null)
        {
            _scaleGroupRepository = scaleGroupRepository;
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("El grupo es requerido.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido.")
                .MaximumLength(10).WithMessage("El código acepta como máximo 10 caracteres.")
                .MustAsync(async (dto, code, _) =>
                {
                    var exists = await _scaleGroupRepository.GetValidatorByGroupIdAsync(code, dto.GroupId, id);
                    return !exists;
                })
                .WithMessage("Ya existe un grupo de escala con este código.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(200).WithMessage("El nombre acepta como máximo 200 caracteres.");
   
            RuleFor(x => x.Weighting)
                .NotNull().WithMessage("La ponderación es requerida.");
        }
    }
}