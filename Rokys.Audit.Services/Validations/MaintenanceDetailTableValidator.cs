using FluentValidation;
using Rokys.Audit.DTOs.Requests.MaintenanceDetailTable;

namespace Rokys.Audit.Services.Validations
{
    public class MaintenanceDetailTableValidator : AbstractValidator<MaintenanceDetailTableRequestDto>
    {
        public MaintenanceDetailTableValidator()
        {
            RuleFor(x => x.MaintenanceTableId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}