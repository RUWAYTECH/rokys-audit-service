using FluentValidation;
using Rokys.Audit.DTOs.Requests.InboxItems;

namespace Rokys.Audit.Services.Validations
{
    public class InboxItemValidator : AbstractValidator<InboxItemRequestDto>
    {
        public InboxItemValidator()
        {
            RuleFor(x => x.Comments)
                .MaximumLength(2000).WithMessage("Los comentarios solo aceptan como máximo 2000 caracteres.");

            // Priority removed from InboxItem
        }
    }
}
