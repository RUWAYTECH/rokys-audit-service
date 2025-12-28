# Documentación Técnica - Sistema de Auditoría Rokys

## 📑 Tabla de Contenidos

1. [Visión General del Proyecto](#visión-general-del-proyecto)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Tecnologías y Frameworks](#tecnologías-y-frameworks)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Modelo de Datos](#modelo-de-datos)
6. [Componentes Principales](#componentes-principales)
7. [API y Endpoints](#api-y-endpoints)
8. [Seguridad y Autenticación](#seguridad-y-autenticación)
9. [Integración con Microservicios](#integración-con-microservicios)
10. [Configuración y Despliegue](#configuración-y-despliegue)
11. [Guías de Desarrollo](#guías-de-desarrollo)

---

## Visión General del Proyecto

### 🎯 Propósito

El **Sistema de Auditoría Rokys** es una aplicación empresarial diseñada para gestionar y ejecutar auditorías periódicas en tiendas de la organización. El sistema permite:

- ✅ **Configurar criterios de auditoría** personalizados por empresa
- ✅ **Ejecutar auditorías periódicas** con plantillas configurables
- ✅ **Evaluar resultados** automáticamente según criterios de puntuación
- ✅ **Generar reportes** en PDF y Excel
- ✅ **Gestionar flujo de aprobaciones** mediante bandeja de entrada
- ✅ **Notificar eventos** por email
- ✅ **Sincronizar datos** con otros microservicios (Security, Memos)

### 🏢 Alcance

El sistema cubre el ciclo completo de auditorías:
1. **Configuración**: Empresas, tiendas, escalas, grupos, criterios
2. **Ejecución**: Creación de auditorías, captura de datos, evidencias
3. **Evaluación**: Cálculo automático de puntuaciones y escalas
4. **Aprobación**: Flujo de trabajo con múltiples roles
5. **Reportería**: Generación de documentos PDF/Excel
6. **Notificaciones**: Alertas por email en puntos clave del proceso

---

## Arquitectura del Sistema

### 🏗️ Patrón de Arquitectura: Clean Architecture / DDD

El proyecto sigue los principios de **Clean Architecture** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                    Rokys.Audit.WebAPI                       │
│  (Capa de Presentación - Controllers, Middleware, Filters) │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────┴────────────────────────────────┐
│                  Rokys.Audit.Services                       │
│      (Capa de Aplicación - Business Logic, Validators)     │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────┴────────────────────────────────┐
│              Rokys.Audit.Infrastructure                     │
│    (Capa de Infraestructura - Repositories, Persistence)   │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────┴────────────────────────────────┐
│                   Rokys.Audit.Model                         │
│              (Capa de Dominio - Entities, DTOs)             │
└─────────────────────────────────────────────────────────────┘
```

### 📦 Capas del Sistema

#### 1. **Capa de Presentación** (WebAPI)
- **Responsabilidad**: Exponer endpoints REST, manejar peticiones HTTP
- **Componentes**: Controllers, Filters, Middleware, Startup
- **Tecnologías**: ASP.NET Core 9.0, Swagger/OpenAPI

#### 2. **Capa de Aplicación** (Services)
- **Responsabilidad**: Lógica de negocio, orquestación de casos de uso
- **Componentes**: Services, Validators, DTOs
- **Tecnologías**: FluentValidation, AutoMapper

#### 3. **Capa de Infraestructura** (Infrastructure/Persistence)
- **Responsabilidad**: Acceso a datos, integración con servicios externos
- **Componentes**: Repositories (EF Core, Dapper), Email Service
- **Tecnologías**: Entity Framework Core 7, Dapper, SMTP

#### 4. **Capa de Dominio** (Model)
- **Responsabilidad**: Entidades de negocio, objetos de valor
- **Componentes**: Entities, Value Objects
- **Tecnologías**: .NET 9.0

#### 5. **Capa de Integración** (Subscription.Hub)
- **Responsabilidad**: Comunicación con otros microservicios
- **Componentes**: Event Handlers, RabbitMQ Consumers
- **Tecnologías**: RabbitMQ, Ruway.Events

### 🔄 Flujo de Datos

```
┌─────────┐      ┌─────────────┐      ┌──────────┐      ┌──────────┐
│ Client  │─────▶│ Controller  │─────▶│ Service  │─────▶│Repository│
│ (HTTP)  │◀─────│  (WebAPI)   │◀─────│(Business)│◀─────│  (Data)  │
└─────────┘      └─────────────┘      └──────────┘      └──────────┘
                        │                   │
                        │                   ▼
                        │            ┌──────────────┐
                        │            │ External     │
                        │            │ Services     │
                        │            │ (Email, etc) │
                        │            └──────────────┘
                        ▼
                 ┌──────────────┐
                 │ RabbitMQ     │
                 │ Event Bus    │
                 └──────────────┘
```

---

## Tecnologías y Frameworks

### 🛠️ Stack Tecnológico Principal

| Categoría | Tecnología | Versión | Uso |
|-----------|-----------|---------|-----|
| **Framework** | .NET | 9.0 | Runtime y SDK |
| **Web API** | ASP.NET Core | 9.0 | Framework web |
| **ORM** | Entity Framework Core | 7.0.2 | Acceso a datos principal |
| **Micro ORM** | Dapper | 2.1.35 | Consultas optimizadas |
| **Base de Datos** | SQL Server | 2019+ | Almacenamiento |
| **Mapeo** | AutoMapper | 12.0.1 | Mapeo DTO-Entity |
| **Validación** | FluentValidation | 11.4.0 | Validaciones de negocio |
| **IoC** | Autofac | 6.5.0 | Inyección de dependencias |
| **Documentación API** | Swagger/Swashbuckle | 6.6.2 | Documentación OpenAPI |
| **Logging** | Serilog | 2.12.0 | Registro de logs |
| **Autenticación** | JWT Bearer | 6.0.13 | Autenticación basada en tokens |
| **Mensajería** | RabbitMQ | - | Bus de eventos |
| **Reportes PDF** | QuestPDF | 2025.7.4 | Generación de PDFs |
| **Reportes Excel** | ClosedXML | 0.105.0 | Generación de Excel |
| **Templates** | Scriban | 6.2.1 | Motor de templates |

### 📚 Librerías Adicionales

- **Newtonsoft.Json**: Serialización JSON
- **NeuroSpeech.RetroCoreFit**: Cliente HTTP tipado
- **Microsoft.Extensions.***: Extensiones de .NET

---

## Estructura del Proyecto

### 📂 Organización de Proyectos

```
rokys-audit-service/
├── 📁 Rokys.Audit.WebAPI/                    # API REST (Entry Point)
│   ├── Controllers/                          # Endpoints REST
│   ├── Middleware/                           # Middleware personalizado
│   ├── Filters/                              # Filtros de acción
│   ├── Configuration/                        # Configuraciones
│   ├── DependencyInjection/                  # Registro de servicios
│   ├── Template/Mail/                        # Plantillas de email
│   ├── Program.cs                            # Punto de entrada
│   └── Startup.cs                            # Configuración de servicios
│
├── 📁 Rokys.Audit.Services/                  # Lógica de Negocio
│   ├── Services/                             # Implementación de servicios
│   │   ├── *Service.cs                       # Servicios de dominio
│   │   ├── Emails/                           # Servicios de email
│   │   ├── Pdf/                              # Generación de PDFs
│   │   └── ReportUtils/                      # Utilidades de reportes
│   └── Validations/                          # Validadores FluentValidation
│
├── 📁 Rokys.Audit.Services.Interfaces/       # Contratos de Servicios
│   └── I*Service.cs                          # Interfaces de servicios
│
├── 📁 Rokys.Audit.Infrastructure/            # Infraestructura Base
│   ├── IRepository.cs                        # Contrato genérico de repositorio
│   └── Common interfaces                     # Interfaces compartidas
│
├── 📁 Rokys.Audit.Infrastructure.Persistence/ # Acceso a Datos Base
│   └── Base repository implementations
│
├── 📁 Rokys.Audit.Infrastructure.Persistence.EF/ # Entity Framework
│   ├── Storage/ApplicationDbContext.cs       # Contexto de EF
│   ├── Repositories/                         # Repositorios EF
│   └── Configurations/                       # Configuraciones de entidades
│
├── 📁 Rokys.Audit.Infrastructure.Persistence.Dp/ # Dapper
│   ├── ContextDp.cs                          # Contexto de Dapper
│   └── Repositories/                         # Repositorios Dapper
│
├── 📁 Rokys.Audit.Infrastructure.Mapping.AM/ # AutoMapper
│   └── AMMapper.cs                           # Perfiles de mapeo
│
├── 📁 Rokys.Audit.Model/                     # Entidades de Dominio
│   └── Tables/                               # Entidades de tablas
│       ├── AuditEntity.cs                    # Entidad base de auditoría
│       ├── Enterprise.cs
│       ├── Stores.cs
│       ├── PeriodAudit.cs
│       └── ... (otras entidades)
│
├── 📁 Rokys.Audit.DTOs/                      # Data Transfer Objects
│   ├── Common/                               # DTOs comunes
│   ├── Requests/                             # DTOs de petición
│   └── Responses/                            # DTOs de respuesta
│
├── 📁 Rokys.Audit.External.Services/         # Servicios Externos
│   └── EmailService.cs                       # Servicio de correo
│
├── 📁 Rokys.Audit.External.Services.Interfaces/ # Contratos Externos
│   └── IEmailService.cs
│
├── 📁 Rokys.Audit.Subscription.Hub/          # Hub de Eventos
│   ├── Services/                             # Manejadores de eventos
│   ├── Configuration/                        # Configuración del hub
│   └── Extensions/                           # Extensiones DI
│
├── 📁 Rokys.Audit.Globalization/             # Internacionalización
│   └── ValidationMessage.resx                # Mensajes de validación
│
├── 📁 Rokys.Audit.Common/                    # Utilidades Compartidas
│   ├── Constant/                             # Constantes
│   ├── Extensions/                           # Métodos de extensión
│   └── Helpers/                              # Clases auxiliares
│
├── 📁 Rokys.Audit.DataBase/                  # Scripts de Base de Datos
│   ├── Init/DataInitial.sql                  # Script de creación inicial
│   ├── Inserts/                              # Scripts de datos iniciales
│   ├── changelog.xml                         # Changelog de Liquibase
│   └── liquibase.properties                  # Configuración Liquibase
│
├── 📁 publish/                                # Archivos publicados
├── global.json                                # Versión del SDK
├── Rokys.Audit.Services.sln                   # Solución Visual Studio
└── README.md                                  # Documentación general
```

### 🎯 Responsabilidades por Proyecto

#### **Rokys.Audit.WebAPI**
- Exponer API REST
- Autenticación y autorización
- Validación de entrada
- Manejo de errores HTTP
- Documentación Swagger
- Logging de requests

#### **Rokys.Audit.Services**
- Lógica de negocio
- Validaciones de dominio
- Orquestación de operaciones
- Generación de reportes
- Envío de emails
- Cálculo de puntuaciones

#### **Rokys.Audit.Infrastructure.Persistence.EF**
- Operaciones CRUD con EF Core
- Transacciones de base de datos
- Migraciones
- Configuraciones de entidades

#### **Rokys.Audit.Infrastructure.Persistence.Dp**
- Consultas optimizadas con Dapper
- Reportes complejos
- Operaciones de lectura masiva
- Procedimientos almacenados

#### **Rokys.Audit.Subscription.Hub**
- Escuchar eventos de RabbitMQ
- Sincronizar datos de otros servicios
- Procesar eventos de empleados
- Mantener consistencia eventual

---

## Modelo de Datos

### 🗄️ Esquema de Base de Datos

Para la documentación completa del modelo de datos, consultar:
- [Diccionario de Datos](Rokys.Audit.DataBase/README.md)
- [Script de Creación](Rokys.Audit.DataBase/Init/DataInitial.sql)

### 📊 Diagrama ER Simplificado

```
┌─────────────┐       ┌──────────┐       ┌──────────────┐
│ Enterprise  │──1:N──│  Stores  │──1:N──│ PeriodAudit  │
└─────────────┘       └──────────┘       └──────────────┘
      │                                          │
      │1:N                                       │1:N
      ▼                                          ▼
┌─────────────┐                       ┌────────────────────────┐
│    Group    │                       │ PeriodAuditGroupResult │
└─────────────┘                       └────────────────────────┘
      │                                          │
      │1:N                                       │1:N
      ▼                                          ▼
┌─────────────┐                       ┌────────────────────────┐
│ ScaleGroup  │                       │ PeriodAuditScaleResult │
└─────────────┘                       └────────────────────────┘
      │                                          │
      │1:N                                       │1:N
      ▼                                          ▼
┌──────────────────────┐           ┌─────────────────────────────────┐
│ TableScaleTemplate   │           │ PeriodAuditTableScaleTemplate   │
└──────────────────────┘           └─────────────────────────────────┘
      │                                          │
      │1:N                                       │1:N
      ▼                                          ▼
┌──────────────────────┐           ┌──────────────────────────┐
│ AuditTemplateFields  │           │ PeriodAuditFieldValues   │
└──────────────────────┘           └──────────────────────────┘
```

### 🔑 Entidades Principales

#### **Enterprise** (Empresa)
Representa la organización o empresa que utiliza el sistema.

#### **Stores** (Tiendas)
Tiendas asociadas a una empresa donde se realizan las auditorías.

#### **PeriodAudit** (Auditoría por Período)
Encabezado de una auditoría realizada en una tienda durante un período específico.

#### **Group** (Grupo)
Agrupación de criterios de auditoría (ej: Limpieza, Seguridad, Operaciones).

#### **ScaleGroup** (Subgrupo de Escala)
Subdivisión de un grupo con criterios específicos de evaluación.

#### **TableScaleTemplate** (Plantilla de Tabla)
Define la estructura de tablas de captura de datos (horizontal/vertical).

#### **AuditTemplateFields** (Campos de Plantilla)
Define los campos individuales dentro de una plantilla (texto, número, fecha, etc.).

#### **ScoringCriteria** (Criterios de Puntuación)
Criterios de evaluación que determinan la puntuación final.

#### **PeriodAuditFieldValues** (Valores Capturados)
Valores reales capturados durante la ejecución de la auditoría.

---

## Componentes Principales

### 🎮 Controllers (API Layer)

#### Estructura de Controllers

Todos los controllers heredan de `ControllerBase` y están decorados con:
- `[ApiController]`
- `[Route("api/[controller]")]`
- `[Authorize]` (cuando se requiere autenticación)

#### Controllers Principales

| Controller | Ruta Base | Descripción |
|-----------|-----------|-------------|
| **EnterpriseController** | `/api/Enterprise` | CRUD de empresas |
| **StoreController** | `/api/Store` | CRUD de tiendas |
| **PeriodAuditController** | `/api/PeriodAudit` | Gestión de auditorías |
| **GroupController** | `/api/Group` | CRUD de grupos |
| **ScaleGroupController** | `/api/ScaleGroup` | CRUD de subgrupos |
| **TableScaleTemplateController** | `/api/TableScaleTemplate` | Gestión de plantillas |
| **AuditTemplateFieldController** | `/api/AuditTemplateField` | Campos de plantillas |
| **ScoringCriteriaController** | `/api/ScoringCriteria` | Criterios de puntuación |
| **InboxItemsController** | `/api/InboxItems` | Bandeja de aprobaciones |
| **ReportsController** | `/api/Reports` | Generación de reportes |
| **StorageFilesController** | `/api/StorageFiles` | Gestión de archivos |
| **UserReferenceController** | `/api/UserReference` | Usuarios sincronizados |

### 🔧 Services (Business Logic Layer)

#### Patrón de Servicios

Cada servicio sigue el patrón:
```csharp
public interface I[Entity]Service
{
    Task<IEnumerable<[Entity]>> GetAll();
    Task<[Entity]> GetById(Guid id);
    Task<[Entity]> Create([Entity]Dto dto);
    Task<[Entity]> Update(Guid id, [Entity]Dto dto);
    Task<bool> Delete(Guid id);
}

public class [Entity]Service : I[Entity]Service
{
    private readonly IRepository<[Entity]> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<[Entity]Service> _logger;
    
    // Implementación...
}
```

#### Servicios Especializados

**PeriodAuditService**
- Creación y gestión de auditorías
- Cálculo de puntuaciones
- Aplicación de criterios de evaluación
- Generación de resultados

**ReportsService**
- Generación de PDFs con QuestPDF
- Generación de Excel con ClosedXML
- Consultas optimizadas para reportes

**InboxItemsService**
- Gestión de flujo de aprobaciones
- Transiciones de estado
- Notificaciones de cambios

**EmailService** (External)
- Envío de emails SMTP
- Plantillas HTML con Scriban
- Manejo de adjuntos

### 🗃️ Repositories (Data Access Layer)

#### Patrón Repository

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(object id);
    Task<T> Add(T entity);
    Task<T> Update(T entity);
    Task<bool> Delete(object id);
}
```

#### Implementaciones

**Entity Framework Repositories**
- Operaciones CRUD estándar
- Soporte de transacciones
- Tracking de cambios
- Navegación de relaciones

**Dapper Repositories**
- Consultas SQL raw optimizadas
- Mapeo ligero
- Rendimiento superior en lecturas
- Consultas complejas con joins

### 🔄 AutoMapper Profiles

**AMMapper.cs** contiene los perfiles de mapeo:

```csharp
public class AMMapper : Profile
{
    public AMMapper()
    {
        CreateMap<Enterprise, EnterpriseDto>().ReverseMap();
        CreateMap<Stores, StoreDto>().ReverseMap();
        CreateMap<PeriodAudit, PeriodAuditDto>().ReverseMap();
        // ... más mapeos
    }
}
```

### ✅ Validators (FluentValidation)

**Ejemplo de Validator:**

```csharp
public class PeriodAuditValidator : AbstractValidator<PeriodAuditDto>
{
    public PeriodAuditValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty()
            .WithMessage("La tienda es requerida");
            
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("La fecha de inicio debe ser menor a la fecha fin");
            
        RuleFor(x => x.ScoreValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El puntaje no puede ser negativo");
    }
}
```

---

## API y Endpoints

### 📡 Documentación de API

La API está documentada con **Swagger/OpenAPI** y está disponible en:
- **Desarrollo**: `http://localhost:5000/swagger`
- **Producción**: `http://172.16.10.12:8084/swagger`

### 🔐 Autenticación

Todos los endpoints (excepto `/api/Diagnostics`) requieren autenticación mediante **JWT Bearer Token**.

**Header requerido:**
```http
Authorization: Bearer <token>
```

### 📝 Endpoints Principales

#### **Enterprise** (Empresas)

```http
GET    /api/Enterprise              # Listar todas las empresas
GET    /api/Enterprise/{id}         # Obtener empresa por ID
POST   /api/Enterprise              # Crear nueva empresa
PUT    /api/Enterprise/{id}         # Actualizar empresa
DELETE /api/Enterprise/{id}         # Eliminar empresa
```

**Request Body (POST/PUT):**
```json
{
  "name": "Rokys S.A.",
  "code": "ROKYS01",
  "address": "Av. Principal 123",
  "isActive": true
}
```

#### **Stores** (Tiendas)

```http
GET    /api/Store                   # Listar todas las tiendas
GET    /api/Store/{id}              # Obtener tienda por ID
GET    /api/Store/ByEnterprise/{id} # Tiendas por empresa
POST   /api/Store                   # Crear nueva tienda
PUT    /api/Store/{id}              # Actualizar tienda
DELETE /api/Store/{id}              # Eliminar tienda
```

**Request Body (POST/PUT):**
```json
{
  "name": "Tienda San Miguel",
  "code": "TDA001",
  "address": "Av. La Marina 2000",
  "enterpriseId": "uuid-empresa",
  "isActive": true
}
```

#### **PeriodAudit** (Auditorías)

```http
GET    /api/PeriodAudit                        # Listar auditorías
GET    /api/PeriodAudit/{id}                   # Obtener auditoría por ID
GET    /api/PeriodAudit/ByStore/{storeId}      # Auditorías por tienda
POST   /api/PeriodAudit                        # Crear nueva auditoría
PUT    /api/PeriodAudit/{id}                   # Actualizar auditoría
PUT    /api/PeriodAudit/{id}/Submit            # Enviar a aprobación
PUT    /api/PeriodAudit/{id}/Approve           # Aprobar auditoría
PUT    /api/PeriodAudit/{id}/Reject            # Rechazar auditoría
DELETE /api/PeriodAudit/{id}                   # Eliminar auditoría
```

**Request Body (POST):**
```json
{
  "storeId": "uuid-tienda",
  "startDate": "2025-01-01",
  "endDate": "2025-01-31",
  "auditedDays": 31,
  "globalObservations": "Auditoría mensual enero",
  "participants": [
    {
      "userReferenceId": "uuid-usuario",
      "roleCode": "AUD",
      "roleName": "Auditor"
    }
  ]
}
```

#### **Reports** (Reportes)

```http
GET    /api/Reports/Audit/{id}/Pdf              # Generar PDF de auditoría
GET    /api/Reports/Audit/{id}/Excel            # Generar Excel de auditoría
GET    /api/Reports/Store/{storeId}/Summary     # Resumen por tienda
GET    /api/Reports/Enterprise/{id}/Dashboard   # Dashboard empresarial
```

**Respuesta:**
- PDF: `Content-Type: application/pdf`
- Excel: `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`

#### **InboxItems** (Bandeja)

```http
GET    /api/InboxItems/MyInbox                  # Mi bandeja de entrada
GET    /api/InboxItems/ByAudit/{auditId}        # Historial de una auditoría
POST   /api/InboxItems/Approve                  # Aprobar ítem
POST   /api/InboxItems/Reject                   # Rechazar ítem
POST   /api/InboxItems/Return                   # Devolver ítem
```

#### **StorageFiles** (Archivos)

```http
GET    /api/StorageFiles/{id}                   # Obtener archivo
GET    /api/StorageFiles/ByEntity/{entityId}    # Archivos por entidad
POST   /api/StorageFiles/Upload                 # Subir archivo
DELETE /api/StorageFiles/{id}                   # Eliminar archivo
```

**Request (Upload):**
```http
POST /api/StorageFiles/Upload
Content-Type: multipart/form-data

--boundary
Content-Disposition: form-data; name="file"; filename="evidencia.pdf"
Content-Type: application/pdf

[binary data]
--boundary
Content-Disposition: form-data; name="entityId"

uuid-entidad
--boundary
Content-Disposition: form-data; name="entityName"

PeriodAudit
--boundary--
```

### 📊 Códigos de Respuesta HTTP

| Código | Descripción |
|--------|-------------|
| 200 OK | Operación exitosa |
| 201 Created | Recurso creado exitosamente |
| 204 No Content | Operación exitosa sin contenido |
| 400 Bad Request | Datos de entrada inválidos |
| 401 Unauthorized | No autenticado |
| 403 Forbidden | No autorizado |
| 404 Not Found | Recurso no encontrado |
| 409 Conflict | Conflicto de negocio |
| 500 Internal Server Error | Error del servidor |

### 📋 Formato de Respuesta de Error

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["El nombre es requerido"],
    "Code": ["El código ya existe"]
  },
  "traceId": "00-trace-id-00"
}
```

---

## Seguridad y Autenticación

### 🔐 Estrategia de Seguridad

El sistema implementa múltiples capas de seguridad:

1. **Autenticación JWT**
2. **Autorización basada en roles**
3. **Validación de tokens con IdentityServer**
4. **CORS configurado**
5. **HTTPS en producción**

### 🎫 Autenticación JWT

**Configuración en appsettings.json:**

```json
{
  "JwtSettings": {
    "Issuer": "http://172.16.10.12:8082",
    "Audience": "rokys-audit-api",
    "Key": "9ba622c5-cb74-4c01-b33a-d24db6dcd1fc",
    "ExpirationInMinute": 10
  }
}
```

**Configuración en Startup.cs:**

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
```

### 🛡️ Integración con IdentityServer

**Configuración:**

```json
{
  "IdentityServer": {
    "Authority": "http://172.16.10.12:8082/",
    "Audience": "rokys-audit-api",
    "ClientId": "rokys-audit-api",
    "ClientSecret": "rokys-audit-secret",
    "RequireHttpsMetadata": false
  }
}
```

**Middleware de Validación:**

```csharp
// CustomJwtValidationMiddleware.cs
public class CustomJwtValidationMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();
            
        if (token != null)
        {
            await ValidateToken(token, context);
        }
        
        await _next(context);
    }
}
```

### 👥 Autorización basada en Roles

**Roles del Sistema:**

| Código | Nombre | Permisos |
|--------|--------|----------|
| **ADM** | Administrador | Todos los permisos |
| **AUD** | Auditor | Crear y ejecutar auditorías |
| **SUP** | Supervisor | Aprobar/rechazar auditorías |
| **OPE** | Operaciones | Ver reportes |
| **GER** | Gerente | Dashboard ejecutivo |

**Uso en Controllers:**

```csharp
[Authorize(Roles = "ADM,SUP")]
[HttpPost("Approve/{id}")]
public async Task<IActionResult> ApproveAudit(Guid id)
{
    // Solo administradores y supervisores
}

[Authorize(Roles = "AUD")]
[HttpPost]
public async Task<IActionResult> CreateAudit([FromBody] PeriodAuditDto dto)
{
    // Solo auditores
}
```

### 🌐 Configuración CORS

```csharp
// Startup.cs
app.UseCors(x => x
    .WithOrigins(domains) // Configurado en appsettings
    .AllowAnyMethod()
    .AllowAnyHeader()
);
```

**Dominios permitidos (appsettings.json):**

```json
{
  "AllowedHosts": "http://localhost:4200;http://172.16.10.12:8085"
}
```

### 🔒 Seguridad de Archivos

**Validación de archivos subidos:**

```json
{
  "FileSettings": {
    "MaxFileSize": 10485760,
    "AllowedFileTypes": [".pdf", ".xlsx", ".jpg", ".png", ".jpeg"],
    "Path": "D:\\AuditUploads"
  }
}
```

**Validación en código:**

```csharp
public async Task<IActionResult> Upload(IFormFile file)
{
    if (file.Length > _fileSettings.MaxFileSize)
        return BadRequest("Archivo demasiado grande");
        
    var extension = Path.GetExtension(file.FileName);
    if (!_fileSettings.AllowedFileTypes.Contains(extension))
        return BadRequest("Tipo de archivo no permitido");
        
    // Procesar archivo...
}
```

---

## Integración con Microservicios

### 🔗 Arquitectura de Microservicios

El Sistema de Auditoría forma parte de un ecosistema de microservicios:

```
┌──────────────────┐       ┌──────────────────┐
│  Security MS     │◀─────▶│   Audit MS       │
│  (Usuarios)      │       │  (Este sistema)  │
└──────────────────┘       └──────────────────┘
                                    ▲
                                    │
                           ┌────────┴────────┐
                           │   RabbitMQ      │
                           │   Event Bus     │
                           └────────┬────────┘
                                    │
                                    ▼
┌──────────────────┐       ┌──────────────────┐
│   Memos MS       │◀─────▶│   Other MS       │
│  (Empleados)     │       │                  │
└──────────────────┘       └──────────────────┘
```

### 🐰 RabbitMQ Event Bus

**Configuración:**

```json
{
  "RabbitMQ": {
    "HostName": "172.16.10.17",
    "Port": 5672,
    "UserName": "owner",
    "Password": "P4ss@78_#%a9",
    "EventsExchange": "rokys.events",
    "MicroserviceName": "audit",
    "ConnectionTimeout": 30000,
    "EnableRetries": true,
    "MaxRetries": 3
  }
}
```

### 📨 Eventos Publicados

El sistema **publica** los siguientes eventos:

| Evento | Routing Key | Descripción |
|--------|-------------|-------------|
| **AuditCreatedEvent** | `audit.created` | Nueva auditoría creada |
| **AuditSubmittedEvent** | `audit.submitted` | Auditoría enviada a aprobación |
| **AuditApprovedEvent** | `audit.approved` | Auditoría aprobada |
| **AuditRejectedEvent** | `audit.rejected` | Auditoría rechazada |
| **AuditCompletedEvent** | `audit.completed` | Auditoría completada |

**Estructura de evento:**

```json
{
  "eventId": "uuid",
  "eventType": "AuditCreatedEvent",
  "timestamp": "2025-12-26T10:30:00Z",
  "source": "audit",
  "data": {
    "auditId": "uuid",
    "storeId": "uuid",
    "createdBy": "uuid",
    "status": "Draft"
  }
}
```

### 📬 Eventos Suscritos

El sistema **escucha** los siguientes eventos:

| Evento | Routing Key | Acción |
|--------|-------------|--------|
| **EmployeeCreatedEvent** | `memos.employee.created` | Crear UserReference |
| **EmployeeUpdatedEvent** | `memos.employee.updated` | Actualizar UserReference |
| **EmployeeDeletedEvent** | `memos.employee.deleted` | Desactivar UserReference |
| **UserCreatedEvent** | `security.user.created` | Sincronizar usuario |
| **UserUpdatedEvent** | `security.user.updated` | Actualizar usuario |

**Configuración de suscripciones:**

```json
{
  "RabbitMQ": {
    "Subscriptions": [
      {
        "RoutingKey": "memos.employee.created",
        "QueueName": "audit.employee_created_handler",
        "Description": "Audita cuando se crean empleados en Memos"
      },
      {
        "RoutingKey": "security.user.created",
        "QueueName": "audit.user_created_handler",
        "Description": "Audita cuando Security crea usuarios"
      }
    ]
  }
}
```

### 🔄 Subscription Hub

**EmployeeEventService.cs** maneja los eventos de empleados:

```csharp
public class EmployeeEventService : IEmployeeEventService
{
    private readonly IUserReferenceService _userReferenceService;
    private readonly ILogger<EmployeeEventService> _logger;

    public async Task HandleEmployeeCreated(EmployeeCreatedEvent evt)
    {
        _logger.LogInformation(
            "Processing EmployeeCreatedEvent for employee {EmployeeId}", 
            evt.EmployeeId);

        var userRef = new UserReferenceDto
        {
            EmployeeId = evt.EmployeeId,
            FirstName = evt.FirstName,
            LastName = evt.LastName,
            Email = evt.Email,
            DocumentNumber = evt.DocumentNumber,
            IsActive = true
        };

        await _userReferenceService.Create(userRef);
        
        _logger.LogInformation(
            "UserReference created for employee {EmployeeId}", 
            evt.EmployeeId);
    }
}
```

### 🔌 Integración con IdentityServer

**IIdentityServerService.cs** permite validar tokens y obtener información de usuarios:

```csharp
public interface IIdentityServerService
{
    Task<bool> ValidateToken(string token);
    Task<UserInfo> GetUserInfo(string token);
    Task<List<string>> GetUserRoles(string userId);
}
```

**Uso:**

```csharp
public class CustomJwtValidationMiddleware
{
    private readonly IIdentityServerService _identityServer;

    public async Task InvokeAsync(HttpContext context)
    {
        var token = GetTokenFromHeader(context);
        
        if (token != null)
        {
            var isValid = await _identityServer.ValidateToken(token);
            
            if (!isValid)
            {
                context.Response.StatusCode = 401;
                return;
            }
            
            var userInfo = await _identityServer.GetUserInfo(token);
            context.Items["UserInfo"] = userInfo;
        }
        
        await _next(context);
    }
}
```

---

## Configuración y Despliegue

### ⚙️ Configuración de Ambientes

El proyecto soporta múltiples ambientes:

- **Development** (appsettings.Development.json)
- **Production** (appsettings.Production.json)

### 📝 Variables de Configuración

#### **Cadenas de Conexión**

```json
{
  "ConnectionStrings": {
    "Main": "Server=172.16.10.12;Database=DBAuditQA;User=memo;Password=***;TrustServerCertificate=True;"
  }
}
```

#### **Seguridad**

```json
{
  "Security": {
    "Enabled": true
  },
  "JwtSettings": {
    "Issuer": "http://172.16.10.12:8082",
    "Audience": "rokys-audit-api",
    "Key": "your-secret-key",
    "ExpirationInMinute": 10
  }
}
```

#### **Email**

```json
{
  "Email": {
    "SmtpServer": "smtp.office365.com",
    "SmtpPort": 587,
    "Username": "trazabilidad.rrhh.gr@rokys.pe",
    "Password": "***",
    "FromName": "Notificacion Rokys",
    "FromEmail": "trazabilidad.rrhh.gr@rokys.pe"
  }
}
```

#### **Archivos**

```json
{
  "FileSettings": {
    "MaxFileSize": 10485760,
    "AllowedFileTypes": [".pdf", ".xlsx", ".jpg", ".png", ".jpeg"],
    "Path": "D:\\AuditUploads"
  }
}
```

#### **RabbitMQ**

```json
{
  "RabbitMQ": {
    "HostName": "172.16.10.17",
    "Port": 5672,
    "UserName": "owner",
    "Password": "***",
    "EventsExchange": "rokys.events",
    "MicroserviceName": "audit"
  }
}
```

### 🚀 Despliegue

#### **Requisitos del Sistema**

- **.NET 9.0 SDK** o superior
- **SQL Server 2019** o superior
- **RabbitMQ 3.x** o superior
- **Windows Server** o **Linux**
- **IIS 10** (Windows) o **Nginx/Kestrel** (Linux)

#### **Pasos de Despliegue**

**1. Publicar la aplicación:**

```bash
dotnet publish -c Release -o ./publish
```

**2. Configurar Base de Datos:**

```bash
# Ejecutar scripts SQL
sqlcmd -S server -d DBAudit -i DataInitial.sql

# O usar Liquibase
liquibase --changeLogFile=changelog.xml update
```

**3. Configurar IIS (Windows):**

- Crear Application Pool con .NET CLR: No Managed Code
- Crear Website apuntando a la carpeta publish
- Configurar bindings (puerto, SSL, etc.)
- Asignar permisos a la carpeta de archivos

**4. Configurar Kestrel (Linux):**

```bash
# Crear servicio systemd
sudo nano /etc/systemd/system/rokys-audit.service

[Unit]
Description=Rokys Audit API

[Service]
WorkingDirectory=/var/www/rokys-audit
ExecStart=/usr/bin/dotnet /var/www/rokys-audit/Rokys.Audit.WebAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=rokys-audit
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target

# Habilitar e iniciar
sudo systemctl enable rokys-audit.service
sudo systemctl start rokys-audit.service
```

**5. Configurar Nginx (Reverse Proxy):**

```nginx
server {
    listen 80;
    server_name audit.rokys.pe;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 📦 Docker (Opcional)

**Dockerfile:**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Rokys.Audit.WebAPI/Rokys.Audit.WebAPI.csproj", "Rokys.Audit.WebAPI/"]
RUN dotnet restore "Rokys.Audit.WebAPI/Rokys.Audit.WebAPI.csproj"
COPY . .
WORKDIR "/src/Rokys.Audit.WebAPI"
RUN dotnet build "Rokys.Audit.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rokys.Audit.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rokys.Audit.WebAPI.dll"]
```

**docker-compose.yml:**

```yaml
version: '3.8'

services:
  audit-api:
    image: rokys-audit-api:latest
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8084:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Main=Server=sql-server;Database=DBAudit;User=sa;Password=***
    depends_on:
      - sql-server
      - rabbitmq
    networks:
      - rokys-network

  sql-server:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    networks:
      - rokys-network

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    networks:
      - rokys-network

networks:
  rokys-network:
    driver: bridge
```

### 🔍 Monitoreo y Logging

**Serilog** está configurado para escribir logs en:

- **Consola**: Desarrollo
- **Archivo**: `LogError/log-{Date}.txt`
- **SQL Server**: (Opcional, configurar Serilog.Sinks.MSSqlServer)

**Configuración de Serilog:**

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("LogError/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### 🩺 Health Checks

**DiagnosticsController** provee endpoints de diagnóstico:

```http
GET /api/Diagnostics/Health        # Estado del servicio
GET /api/Diagnostics/Database      # Estado de la BD
GET /api/Diagnostics/RabbitMQ      # Estado de RabbitMQ
GET /api/Diagnostics/Version       # Versión de la API
```

---

## Guías de Desarrollo

### 🛠️ Configuración del Entorno de Desarrollo

#### **Requisitos**

- **Visual Studio 2022** o **Visual Studio Code** con extensión de C#
- **.NET 9.0 SDK**
- **SQL Server 2019** o superior (o SQL Server Express)
- **SQL Server Management Studio** (SSMS)
- **Git**
- **Postman** o similar (para pruebas de API)

#### **Clonar el Repositorio**

```bash
git clone https://github.com/RUWAYTECH/rokys-audit-service.git
cd rokys-audit-service
```

#### **Restaurar Paquetes NuGet**

```bash
dotnet restore
```

#### **Configurar Base de Datos Local**

1. Crear base de datos:
```sql
CREATE DATABASE DBAuditDev;
```

2. Ejecutar script inicial:
```bash
sqlcmd -S localhost -d DBAuditDev -i Rokys.Audit.DataBase/Init/DataInitial.sql
```

3. Actualizar appsettings.Development.json:
```json
{
  "ConnectionStrings": {
    "Main": "Server=localhost;Database=DBAuditDev;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

#### **Ejecutar la Aplicación**

```bash
cd Rokys.Audit.WebAPI
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

### 📋 Estándares de Código

#### **Convenciones de Nombres**

- **Clases**: PascalCase (`PeriodAuditService`)
- **Métodos**: PascalCase (`GetById`)
- **Variables**: camelCase (`userId`)
- **Constantes**: UPPER_SNAKE_CASE (`MAX_FILE_SIZE`)
- **Interfaces**: I + PascalCase (`IRepository`)

#### **Estructura de Archivos**

- Un archivo por clase
- Nombre del archivo = Nombre de la clase
- Organizar por feature/dominio

#### **Comentarios**

```csharp
/// <summary>
/// Obtiene una auditoría por su ID
/// </summary>
/// <param name="id">ID único de la auditoría</param>
/// <returns>Datos de la auditoría</returns>
public async Task<PeriodAudit> GetById(Guid id)
{
    // Implementación
}
```

### 🧪 Pruebas

#### **Estructura de Pruebas**

```
Rokys.Audit.Tests/
├── Unit/
│   ├── Services/
│   │   └── PeriodAuditServiceTests.cs
│   └── Validators/
│       └── PeriodAuditValidatorTests.cs
├── Integration/
│   ├── Controllers/
│   │   └── PeriodAuditControllerTests.cs
│   └── Repositories/
│       └── PeriodAuditRepositoryTests.cs
└── E2E/
    └── AuditWorkflowTests.cs
```

#### **Ejemplo de Prueba Unitaria**

```csharp
[Fact]
public async Task GetById_WithValidId_ReturnsAudit()
{
    // Arrange
    var mockRepo = new Mock<IRepository<PeriodAudit>>();
    var audit = new PeriodAudit { PeriodAuditId = Guid.NewGuid() };
    mockRepo.Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(audit);
    
    var service = new PeriodAuditService(mockRepo.Object, null, null);
    
    // Act
    var result = await service.GetById(audit.PeriodAuditId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(audit.PeriodAuditId, result.PeriodAuditId);
}
```

### 🔄 Flujo de Trabajo con Git

#### **Branches**

- `main`: Producción
- `develop`: Desarrollo
- `feature/[nombre]`: Nueva funcionalidad
- `bugfix/[nombre]`: Corrección de bug
- `hotfix/[nombre]`: Corrección urgente en producción

#### **Workflow**

```bash
# Crear feature branch desde develop
git checkout develop
git pull origin develop
git checkout -b feature/nueva-funcionalidad

# Hacer commits
git add .
git commit -m "feat: agregar nueva funcionalidad"

# Push y crear Pull Request
git push origin feature/nueva-funcionalidad

# Después de revisión, merge a develop
# Cuando esté listo, merge a main para producción
```

#### **Mensajes de Commit**

Seguir convención [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: nueva funcionalidad
fix: corrección de bug
docs: cambios en documentación
style: cambios de formato
refactor: refactorización de código
test: agregar o modificar tests
chore: tareas de mantenimiento
```

### 📝 Proceso de Desarrollo de Features

#### **1. Crear Nueva Entidad**

**Modelo (Tables/):**
```csharp
public class NewEntity : AuditEntity
{
    public Guid NewEntityId { get; set; }
    public string Name { get; set; }
    // ... otros campos
}
```

**DTO (DTOs/):**
```csharp
public class NewEntityDto
{
    public Guid? NewEntityId { get; set; }
    public string Name { get; set; }
}
```

**Validator (Validations/):**
```csharp
public class NewEntityValidator : AbstractValidator<NewEntityDto>
{
    public NewEntityValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```

#### **2. Crear Servicios**

**Interface (Services.Interfaces/):**
```csharp
public interface INewEntityService
{
    Task<IEnumerable<NewEntity>> GetAll();
    Task<NewEntity> GetById(Guid id);
    Task<NewEntity> Create(NewEntityDto dto);
    Task<NewEntity> Update(Guid id, NewEntityDto dto);
    Task<bool> Delete(Guid id);
}
```

**Implementación (Services/):**
```csharp
public class NewEntityService : INewEntityService
{
    private readonly IRepository<NewEntity> _repository;
    private readonly IMapper _mapper;
    
    public NewEntityService(
        IRepository<NewEntity> repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    // Implementar métodos...
}
```

#### **3. Crear Repository**

**EF Core (Persistence.EF/):**
```csharp
public class NewEntityRepository : Repository<NewEntity>
{
    public NewEntityRepository(ApplicationDbContext context) 
        : base(context)
    {
    }
    
    // Métodos adicionales si es necesario
}
```

#### **4. Crear Controller**

**Controller (WebAPI/Controllers/):**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NewEntityController : ControllerBase
{
    private readonly INewEntityService _service;
    
    public NewEntityController(INewEntityService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAll();
        return Ok(items);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetById(id);
        if (item == null) return NotFound();
        return Ok(item);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NewEntityDto dto)
    {
        var item = await _service.Create(dto);
        return CreatedAtAction(nameof(GetById), 
            new { id = item.NewEntityId }, item);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] NewEntityDto dto)
    {
        var item = await _service.Update(id, dto);
        return Ok(item);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}
```

#### **5. Registrar en DI Container**

**DependencyConfig.cs:**
```csharp
builder.RegisterType<NewEntityService>()
       .As<INewEntityService>()
       .InstancePerLifetimeScope();
       
builder.RegisterType<NewEntityRepository>()
       .As<IRepository<NewEntity>>()
       .InstancePerLifetimeScope();
```

#### **6. Agregar Mapper Profile**

**AMMapper.cs:**
```csharp
CreateMap<NewEntity, NewEntityDto>().ReverseMap();
```

### 🐛 Debugging

#### **Visual Studio**

1. Establecer breakpoints (F9)
2. Iniciar debug (F5)
3. Inspeccionar variables
4. Step Over (F10), Step Into (F11)

#### **Logs**

```csharp
_logger.LogInformation("Processing audit {AuditId}", auditId);
_logger.LogWarning("Store {StoreId} not found", storeId);
_logger.LogError(ex, "Error creating audit");
```

#### **SQL Profiler**

Usar SQL Server Profiler para analizar queries de EF Core y Dapper.

### 📚 Recursos Adicionales

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)

---

## 📞 Contacto y Soporte

Para soporte técnico o preguntas sobre el proyecto:

- **Equipo**: Ruwaytech Development Team
- **Repositorio**: https://github.com/RUWAYTECH/rokys-audit-service
- **Branch Principal**: `main`
- **Branch de Desarrollo**: `develop`

---

## 📄 Licencia

© 2025 Ruwaytech. Todos los derechos reservados.

---

**Última actualización**: Diciembre 26, 2025  
**Versión del documento**: 1.0.0  
**Versión de la aplicación**: 1.0.0
