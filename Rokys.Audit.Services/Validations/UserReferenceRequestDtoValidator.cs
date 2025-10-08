using FluentValidation;
using Rokys.Audit.DTOs.Requests.UserReference;
using Rokys.Audit.Infrastructure.Repositories;

namespace Rokys.Audit.Services.Validations
{
    public class UserReferenceRequestDtoValidator : AbstractValidator<UserReferenceRequestDto>
    {
        private readonly IUserReferenceRepository _userReferenceRepository;

        public UserReferenceRequestDtoValidator(IUserReferenceRepository userReferenceRepository)
        {
            _userReferenceRepository = userReferenceRepository;

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("El ID del usuario es requerido")
                .MustAsync(async (dto, userId, cancellation) => 
                {
                    // Solo validamos duplicados en creación (cuando no hay UserReferenceId)
                    return !await _userReferenceRepository.ExistsByUserIdAsync(userId);
                })
                .WithMessage("Ya existe un usuario con este ID del sistema de seguridad");

            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("El ID del empleado es requerido")
                .MustAsync(async (dto, employeeId, cancellation) => 
                {
                    // Solo validamos duplicados en creación (cuando no hay UserReferenceId)
                    return !await _userReferenceRepository.ExistsByEmployeeIdAsync(employeeId);
                })
                .WithMessage("Ya existe un usuario con este ID del sistema de empleados");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder los 200 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido es requerido")
                .MaximumLength(200)
                .WithMessage("El apellido no puede exceder los 200 caracteres");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Formato de correo electrónico inválido")
                .MaximumLength(150)
                .WithMessage("El correo no puede exceder los 150 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.DocumentNumber)
                .MaximumLength(20)
                .WithMessage("El número de documento no puede exceder los 20 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

            RuleFor(x => x.RoleCode)
                .MaximumLength(50)
                .WithMessage("El código del rol no puede exceder los 50 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.RoleCode));

            RuleFor(x => x.RoleName)
                .MaximumLength(100)
                .WithMessage("El nombre del rol no puede exceder los 100 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.RoleName));
        }
    }
}