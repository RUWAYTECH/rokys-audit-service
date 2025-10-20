using FluentValidation;
using Rokys.Audit.DTOs.Requests.StorageFiles;

namespace Rokys.Audit.Services.Validations
{
    public class StorageFileValidator : AbstractValidator<StorageFileRequestDto>
    {
        public StorageFileValidator()
        {
            RuleFor(x => x.OriginalName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.FileUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.ClassificationType).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.ClassificationType));
            RuleFor(x => x.EntityId).NotEmpty();
        }
    }
}
