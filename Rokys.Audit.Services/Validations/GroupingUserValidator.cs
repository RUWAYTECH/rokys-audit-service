using FluentValidation;
using Rokys.Audit.DTOs.Requests.GroupingUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Validations
{
    public class GroupingUserValidator : AbstractValidator<GroupingUserRequestDto>
    {
        public GroupingUserValidator() 
        {
            RuleFor(x => x.EnterpriseGroupingId)
                .NotNull().WithMessage("El Id del Grupo de Empresas es requerido.")
                .NotEmpty().WithMessage("El Id del Grupo de Empresas es requerido.");
            RuleFor(x => x.UserReferenceId)
                .NotNull().WithMessage("El Id del Usuario es requerido.")
                .NotEmpty().WithMessage("El Id del Usuario es requerido.");
        }
    }
}
