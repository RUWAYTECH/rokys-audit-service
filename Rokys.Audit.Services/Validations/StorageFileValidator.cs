using FluentValidation;
using Rokys.Audit.DTOs.Requests.StorageFiles;

namespace Rokys.Audit.Services.Validations
{
    public class StorageFileValidator : AbstractValidator<StorageFileRequestDto>
    {
        public StorageFileValidator()
        {
            RuleFor(x => x.EntityId).NotEmpty().WithMessage("El identificador es obligatorio.");
            RuleFor(x => x.EntityName).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.EntityName));
            RuleFor(x => x.ClassificationType).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.ClassificationType));
        }
    }
}
