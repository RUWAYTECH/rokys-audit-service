using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rokys.Audit.Subscription.Hub.Configuration;
using Rokys.Audit.Subscription.Hub.Services.Implementations;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Configuration;
using Ruway.Events.Command.Events;
using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Extensions
{
    /// <summary>
    /// Extensiones para registrar el Subscription Hub en el contenedor de DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra todos los servicios del Subscription Hub
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddSubscriptionHub(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Configurar opciones
            services.Configure<SubscriptionHubOptions>(
                configuration.GetSection(SubscriptionHubOptions.SectionName));

            // Configurar RabbitMQ (reutilizando la configuración existente)
            services.Configure<RabbitMQSettings>(
                configuration.GetSection("RabbitMQ"));

            // Registrar EventSubscriber de Rokys.Events.Command
            services.AddSingleton<IEventSubscriber, EventSubscriber>();

            // Registrar servicios del hub
            services.AddSingleton<IEmployeeEventService, EmployeeEventService>();
            services.AddSingleton<ISubscriptionHubService, SubscriptionHubService>();
            services.AddSingleton<IUserEventService, UserEventService>();
            // Registrar como servicio hospedado
            services.AddHostedService<SubscriptionHubHostedService>();

            return services;
        }

        /// <summary>
        /// Registra el Subscription Hub con configuración personalizada
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configureOptions">Acción para configurar las opciones</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddSubscriptionHub(
            this IServiceCollection services,
            Action<SubscriptionHubOptions> configureOptions)
        {
            services.Configure(configureOptions);

            // Registrar EventSubscriber
            services.AddSingleton<IEventSubscriber, EventSubscriber>();

            // Registrar servicios del hub
            services.AddSingleton<IEmployeeEventService, EmployeeEventService>();
            services.AddSingleton<ISubscriptionHubService, SubscriptionHubService>();

            // Registrar como servicio hospedado
            services.AddHostedService<SubscriptionHubHostedService>();

            return services;
        }

        /// <summary>
        /// Registra solo los servicios del hub sin el servicio hospedado (para control manual)
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddSubscriptionHubServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurar opciones
            services.Configure<SubscriptionHubOptions>(
                configuration.GetSection(SubscriptionHubOptions.SectionName));

            // Configurar RabbitMQ
            services.Configure<RabbitMQSettings>(
                configuration.GetSection("RabbitMQ"));

            // Registrar EventSubscriber
            services.AddSingleton<IEventSubscriber, EventSubscriber>();

            // Registrar servicios del hub
            services.AddSingleton<IEmployeeEventService, EmployeeEventService>();
            services.AddSingleton<ISubscriptionHubService, SubscriptionHubService>();

            return services;
        }
    }
}