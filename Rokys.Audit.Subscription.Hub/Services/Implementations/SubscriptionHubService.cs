using Microsoft.Extensions.Logging;
using Rokys.Audit.Subscription.Hub.Constants;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Interfaces.Events;
using Ruway.Events.Command.Interfaces.Constants;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio principal del hub de suscripciones
    /// </summary>
    public class SubscriptionHubService : ISubscriptionHubService
    {
        private readonly IEventSubscriber _eventSubscriber;
        private readonly IEmployeeEventService _employeeEventService;
        private readonly IUserEventService _userEventService;
        private readonly ILogger<SubscriptionHubService> _logger;
        private bool _isRunning = false;

        public SubscriptionHubService(
            IEventSubscriber eventSubscriber,
            IEmployeeEventService employeeEventService,
            IUserEventService userEventService,
            IStoreEventService storeEventService,
            ILogger<SubscriptionHubService> logger)
        {
            _eventSubscriber = eventSubscriber;
            _employeeEventService = employeeEventService;
            _userEventService = userEventService;
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsRunning => _isRunning;

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning)
            {
                _logger.LogWarning("Subscription Hub is already running");
                return;
            }

            try
            {
                _logger.LogInformation("Starting Rokys Audit Subscription Hub...");

                // Suscribirse a eventos específicos de empleados
                await SubscribeToEmployeeEvents(cancellationToken);

                await SubscribeToUserEvents(cancellationToken);

                // Iniciar el listener de eventos
                await _eventSubscriber.StartListeningAsync(cancellationToken);

                _isRunning = true;
                _logger.LogInformation("Rokys Audit Subscription Hub started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Rokys Audit Subscription Hub");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_isRunning)
            {
                _logger.LogInformation("Subscription Hub is already stopped");
                return;
            }

            try
            {
                _logger.LogInformation("Stopping Rokys Audit Subscription Hub...");

                await _eventSubscriber.StopListeningAsync(cancellationToken);

                _isRunning = false;
                _logger.LogInformation("Rokys Audit Subscription Hub stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Rokys Audit Subscription Hub");
                throw;
            }
        }

        /// <summary>
        /// Configura las suscripciones a eventos de empleados
        /// </summary>
        private async Task SubscribeToEmployeeEvents(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting up employee event subscriptions...");

            // Configurar handlers específicos por tipo de evento usando los eventos existentes
            await _eventSubscriber.SubscribeAsync<EmployeeCreatedEvent>(async (employeeEvent) =>
            {
                if (_employeeEventService != null)
                    await _employeeEventService.HandleEmployeeCreatedAsync(employeeEvent);
            }, EventConstants.EmployeeEvents.EmployeeCreated);

            await _eventSubscriber.SubscribeAsync<EmployeeUpdatedEvent>(async (employeeEvent) =>
             {
                 if (_employeeEventService != null)
                     await _employeeEventService.HandleEmployeeUpdatedAsync(employeeEvent);
             }, EventConstants.EmployeeEvents.EmployeeUpdated);

             await _eventSubscriber.SubscribeAsync<EmployeeDeletedEvent>(async (employeeEvent) =>
             {
                 if (_employeeEventService != null)
                     await _employeeEventService.HandleEmployeeDeletedAsync(employeeEvent);
             }, EventConstants.EmployeeEvents.EmployeeDeleted);

            // Suscribirse a eventos genéricos de empleados (wildcard para capturar cualquier evento de empleado)
            /*await _eventSubscriber.SubscribeAsync(
               async (message, routingKey) => await _employeeEventService.HandleGenericEmployeeEventAsync(message, routingKey),
               "#",
               cancellationToken); */

            _logger.LogInformation("Employee event subscriptions configured successfully");
        }
        
         private async Task SubscribeToUserEvents(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting up user event subscriptions...");

            // Configurar handlers específicos por tipo de evento usando los eventos existentes
            await _eventSubscriber.SubscribeAsync<UserUpdatedEvent>(async (userEvent) =>
            {
                if (_userEventService != null)
                    await _userEventService.HandleUserUpdatedAsync(userEvent);
            }, EventConstants.UserEvents.UserUpdated);

            await _eventSubscriber.SubscribeAsync<UserDeletedEvent>(async (userEvent) =>
            {
                if (_userEventService != null)
                    await _userEventService.HandleUserDeletedAsync(userEvent);
            }, EventConstants.UserEvents.UserDeleted);



            _logger.LogInformation("Users event subscriptions configured successfully");
        }
    }
}