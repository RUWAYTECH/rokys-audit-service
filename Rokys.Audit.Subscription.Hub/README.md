# Rokys.Audit.Subscription.Hub

## 📋 Descripción

El **Rokys.Audit.Subscription.Hub** es un proyecto centralizado para manejar suscripciones a eventos en el ecosistema Rokys Audit. Este hub escucha eventos de empleados (creación, actualización, eliminación) y puede reutilizar servicios existentes como `IEmployeeService`.

## 🏗️ Arquitectura

```
Rokys.Audit.Subscription.Hub/
├── Events/                     # Definición de eventos de dominio
│   └── EmployeeEvents.cs
├── Services/
│   ├── Interfaces/            # Contratos de servicios
│   │   ├── ISubscriptionHubService.cs
│   │   └── IEmployeeEventService.cs
│   └── Implementations/       # Implementaciones
│       ├── SubscriptionHubService.cs
│       ├── EmployeeEventService.cs
│       └── SubscriptionHubHostedService.cs
├── Configuration/             # Opciones de configuración
│   └── SubscriptionHubOptions.cs
└── Extensions/               # Extensiones para DI
    └── ServiceCollectionExtensions.cs
```

## 🚀 Características

### ✅ **Eventos soportados:**
- **EmployeeCreatedEvent**: Empleado creado
- **EmployeeUpdatedEvent**: Empleado actualizado  
- **EmployeeDeletedEvent**: Empleado eliminado/desactivado

### 🔧 **Funcionalidades:**
- **Escucha automática** de eventos via RabbitMQ
- **Procesamiento tipado** de eventos con deserialización JSON
- **Manejo de errores** robusto con logging estructurado
- **Configuración flexible** via appsettings.json
- **Integración seamless** con WebAPI como servicio hospedado
- **Reutilización de servicios** existentes (IEmployeeService)

## ⚙️ Configuración

### appsettings.json
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest", 
    "Password": "guest",
    "VirtualHost": "/",
    "EventsExchange": "rokys.events"
  },
  "SubscriptionHub": {
    "AutoStart": true,
    "StartupTimeoutSeconds": 30,
    "ProcessEmployeeEvents": true,
    "EmployeeEvents": {
      "ProcessCreatedEvents": true,
      "ProcessUpdatedEvents": true,
      "ProcessDeletedEvents": true,
      "RoutingKeyPattern": "employee.*",
      "ProcessingTimeoutSeconds": 30
    },
    "Logging": {
      "LogSuccessfulEvents": true,
      "LogDetailedErrors": true,
      "LogPerformanceStats": false
    }
  }
}
```

## 🔌 Integración con WebAPI

### 1. Agregar referencia al proyecto:
```xml
<ProjectReference Include="..\Rokys.Audit.Subscription.Hub\Rokys.Audit.Subscription.Hub.csproj" />
```

### 2. Registrar en Startup.cs:
```csharp
using Rokys.Audit.Subscription.Hub.Extensions;

public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // ... otras configuraciones
    
    // Agregar Subscription Hub
    services.AddSubscriptionHub(Configuration);
    
    return DependencyConfig.Configure(Services, Configuration);
}
```

### 3. El hub se inicia automáticamente:
- ✅ Se registra como `IHostedService`
- ✅ Inicia automáticamente con la aplicación
- ✅ Se detiene limpiamente al shutdown

## 📝 Uso de servicios existentes

### Ejemplo de uso de IEmployeeService:
```csharp
public class EmployeeEventService : IEmployeeEventService
{
    private readonly IServiceProvider _serviceProvider;

    public async Task HandleEmployeeCreatedAsync(EmployeeCreatedEvent employeeEvent)
    {
        // Usar servicio existente si necesitas validar o obtener más información
        var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();
        var employeeDetails = await employeeService.GetByIdAsync(employeeEvent.EmployeeId);
        
        // Tu lógica personalizada aquí
        await ProcessEmployeeCreation(employeeDetails);
    }
}
```

## 🎯 Routing Keys soportados

| Evento | Routing Key | Descripción |
|--------|-------------|-------------|
| Creación | `employee.created` | Nuevo empleado registrado |
| Actualización | `employee.updated` | Datos de empleado modificados |
| Eliminación | `employee.deleted` | Empleado desactivado/eliminado |
| Wildcard | `employee.*` | Cualquier evento de empleado |

## 📊 Logging

El hub proporciona logging estructurado para:
- ✅ **Eventos procesados** exitosamente
- ⚠️ **Errores detallados** con stack traces
- 📈 **Estadísticas de rendimiento** (opcional)
- 🔍 **Debugging** de mensajes y routing keys

## 🛠️ Personalización

### Control manual del hub:
```csharp
// Registrar solo servicios (sin auto-start)
services.AddSubscriptionHubServices(configuration);

// Control manual
var hub = serviceProvider.GetRequiredService<ISubscriptionHubService>();
await hub.StartAsync();
await hub.StopAsync();
```

### Configuración personalizada:
```csharp
services.AddSubscriptionHub(options =>
{
    options.AutoStart = false;
    options.ProcessEmployeeEvents = true;
    options.EmployeeEvents.ProcessingTimeoutSeconds = 60;
});
```

## 🧪 Testing

El hub está diseñado para ser fácilmente testeable:
- **Interfaces bien definidas** para mocking
- **Separación de responsabilidades** clara
- **Inyección de dependencias** completa
- **Configuración flexible** para diferentes entornos

---

**Desarrollado para el ecosistema Rokys Audit** 🚀