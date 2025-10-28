using FluentValidation;
using Rokys.Audit.DTOs.Requests.StorageFiles;

namespace Rokys.Audit.Services.Validations
{
    public class StorageFileValidator : AbstractValidator<StorageFileRequestDto>
    {
        public StorageFileValidator()
        {
            RuleFor(x => x.EntityId)
                .NotNull().WithMessage("El identificador es obligatorio.")
                .NotEmpty().WithMessage("El identificador es obligatorio.");
            RuleFor(x => x.EntityName)
                .NotNull().WithMessage("La entidad no puede ser vació")
                .NotEmpty().WithMessage("La entidad no puede ser vació")
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.EntityName));
            RuleFor(x => x.ClassificationType).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.ClassificationType));
        }
    }
}
