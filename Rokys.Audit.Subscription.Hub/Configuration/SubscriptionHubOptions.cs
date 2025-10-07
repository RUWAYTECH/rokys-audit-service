namespace Rokys.Audit.Subscription.Hub.Configuration
{
    /// <summary>
    /// Opciones de configuración para el Subscription Hub
    /// </summary>
    public class SubscriptionHubOptions
    {
        public const string SectionName = "SubscriptionHub";

        /// <summary>
        /// Indica si el hub debe iniciarse automáticamente
        /// </summary>
        public bool AutoStart { get; set; } = true;

        /// <summary>
        /// Tiempo de espera para el inicio del hub (en segundos)
        /// </summary>
        public int StartupTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Indica si debe procesar eventos de empleados
        /// </summary>
        public bool ProcessEmployeeEvents { get; set; } = true;

        /// <summary>
        /// Configuración específica para eventos de empleados
        /// </summary>
        public EmployeeEventOptions EmployeeEvents { get; set; } = new();

        /// <summary>
        /// Configuración de logging específica del hub
        /// </summary>
        public LoggingOptions Logging { get; set; } = new();
    }

    /// <summary>
    /// Configuración específica para eventos de empleados
    /// </summary>
    public class EmployeeEventOptions
    {
        /// <summary>
        /// Indica si debe procesar eventos de creación
        /// </summary>
        public bool ProcessCreatedEvents { get; set; } = true;

        /// <summary>
        /// Indica si debe procesar eventos de actualización
        /// </summary>
        public bool ProcessUpdatedEvents { get; set; } = true;

        /// <summary>
        /// Indica si debe procesar eventos de eliminación
        /// </summary>
        public bool ProcessDeletedEvents { get; set; } = true;

        /// <summary>
        /// Patrón de routing key para eventos de empleados
        /// </summary>
        public string RoutingKeyPattern { get; set; } = "employee.*";

        /// <summary>
        /// Tiempo de espera para procesar cada evento (en segundos)
        /// </summary>
        public int ProcessingTimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// Configuración de logging del hub
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Indica si debe logear eventos procesados exitosamente
        /// </summary>
        public bool LogSuccessfulEvents { get; set; } = true;

        /// <summary>
        /// Indica si debe logear errores detallados
        /// </summary>
        public bool LogDetailedErrors { get; set; } = true;

        /// <summary>
        /// Indica si debe logear estadísticas de rendimiento
        /// </summary>
        public bool LogPerformanceStats { get; set; } = false;
    }
}