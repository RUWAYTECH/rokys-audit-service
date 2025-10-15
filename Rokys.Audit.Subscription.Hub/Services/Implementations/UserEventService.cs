using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Subscription.Hub.Constants;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{    
    public class UserEventService : IUserEventService
    {
        private readonly ILogger<UserEventService> _logger;
        private readonly IUserReferenceService _userReferenceService;

        public UserEventService(
            ILogger<UserEventService> logger,
            IUserReferenceService userReferenceService)
        {
            _logger = logger;
            _userReferenceService = userReferenceService;
        }

        public Task HandleGenericUserEventAsync(string message, string routingKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task HandleUserCreatedAsync(UserCreatedEvent UserEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task HandleUserDeletedAsync(UserDeletedEvent UserEvent, CancellationToken cancellationToken = default)
        {
             if(UserEvent.ApplicationCode != CommonConstants.ApplicationCode)
                return;

            var exist = await _userReferenceService.GetByUserId(UserEvent.UserId);
            if (exist.Data != null)
            {
                var user = exist.Data;
                await _userReferenceService.Update(user.UserReferenceId, new DTOs.Requests.UserReference.UserReferenceRequestDto
                {
                    UserId = UserEvent.UserId,
                    EmployeeId = user.EmployeeId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleCode = user.RoleCode,
                    RoleName = user.RoleName,
                    IsActive = false
                });
            }
        }

        public async Task HandleUserUpdatedAsync(UserUpdatedEvent UserEvent, CancellationToken cancellationToken = default)
        {
            if(UserEvent.ApplicationCode != CommonConstants.ApplicationCode)
                return;

            var exist = await _userReferenceService.GetByUserId(UserEvent.UserId);
            if (exist.Data == null)
            {

                await _userReferenceService.Create(new DTOs.Requests.UserReference.UserReferenceRequestDto
                {
                    UserId = UserEvent.UserId,
                    EmployeeId = UserEvent.EmployeeId,
                    FirstName = UserEvent.FirstName,
                    LastName = UserEvent.LastName,
                    Email = UserEvent.Email,
                });
            }
            else
            {
                var user = exist.Data;
                user.EmployeeId = UserEvent.EmployeeId;
                user.FirstName = UserEvent.FirstName;
                user.LastName = UserEvent.LastName;
                user.Email = UserEvent.Email;
                await _userReferenceService.Update(user.UserReferenceId, new DTOs.Requests.UserReference.UserReferenceRequestDto
                {
                    UserId = UserEvent.UserId,
                    EmployeeId = UserEvent.EmployeeId,
                    FirstName = UserEvent.FirstName,
                    LastName = UserEvent.LastName,
                    Email = UserEvent.Email,
                });
            }
        }
    }
}