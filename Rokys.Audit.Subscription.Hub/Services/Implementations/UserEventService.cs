using Microsoft.Extensions.Logging;
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
            _logger.LogInformation("[SUBSCRIPTION-TRACE] UserDeleted event received - UserId: {UserId}, EventId: {EventId}", 
                UserEvent.UserId, UserEvent.EventId);
            
             if(UserEvent.ApplicationCode != CommonConstants.ApplicationCode)
             {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] Event ignored - wrong application: {ApplicationCode}", UserEvent.ApplicationCode);
                return;
             }

            var exist = await _userReferenceService.GetByUserId(UserEvent.UserId);
            if (exist.Data != null)
            {
                await _userReferenceService.Delete(exist.Data.UserReferenceId);
                _logger.LogInformation("[SUBSCRIPTION-TRACE] UserDeleted processed successfully - UserId: {UserId}", UserEvent.UserId);
            }
        }

        public async Task HandleUserUpdatedAsync(UserUpdatedEvent UserEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] UserUpdated event received - UserId: {UserId}, EventId: {EventId}", 
                UserEvent.UserId, UserEvent.EventId);
            
            if(UserEvent.ApplicationCode != CommonConstants.ApplicationCode)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] Event ignored - wrong application: {ApplicationCode}", UserEvent.ApplicationCode);
                return;
            }

            var exist = await _userReferenceService.GetByUserId(UserEvent.UserId);
            if (exist.Data == null)
            {
                if (UserEvent.EmployeeId.HasValue)
                {
                    var employeeExist = await _userReferenceService.GetByEmployeeId(UserEvent.EmployeeId.Value);
                    if (employeeExist.Data != null)
                    {
                        await UpdateUser(employeeExist.Data.UserReferenceId, UserEvent);
                    }
                    else
                    {
                        await CreateUser(UserEvent);
                    }
                }
                else
                {
                    await CreateUser(UserEvent);
                }
            }
            else
            {
                await UpdateUser(exist.Data.UserReferenceId, UserEvent);
            }

            _logger.LogInformation("[SUBSCRIPTION-TRACE] UserUpdated processed successfully - UserId: {UserId}", UserEvent.UserId);
        }

        private async Task CreateUser(UserUpdatedEvent UserEvent)
        {
            await _userReferenceService.Create(new DTOs.Requests.UserReference.UserReferenceRequestDto
            {
                UserId = UserEvent.UserId,
                EmployeeId = UserEvent.EmployeeId,
                FirstName = UserEvent.FirstName,
                LastName = UserEvent.LastName,
                Email = UserEvent.Email,
                RoleCode = UserEvent.RoleCodes,
                RoleName = UserEvent.RoleNames,
            });
        }

        private async Task UpdateUser(Guid userReferenceId, UserUpdatedEvent userEvent)
        {
            await _userReferenceService.UpdateByUser(userReferenceId, new DTOs.Requests.UserReference.UserReferenceRequestDto
            {
                UserId = userEvent.UserId,
                EmployeeId = userEvent.EmployeeId,
                FirstName = userEvent.FirstName,
                LastName = userEvent.LastName,
                Email = userEvent.Email,
                RoleCode = userEvent.RoleCodes,
                RoleName = userEvent.RoleNames,
            });
        }
    }
}