# Diccionario de Datos - Sistema de Auditoría Rokys

## Índice de Contenidos

1. [Tablas de Configuración](#tablas-de-configuración)
2. [Tablas de Criterios de Puntuación](#tablas-de-criterios-de-puntuación)
3. [Tablas de Proceso de Auditoría](#tablas-de-proceso-de-auditoría)
4. [Tablas de Resultados](#tablas-de-resultados)
5. [Tablas de Gestión de Usuarios](#tablas-de-gestión-de-usuarios)
6. [Tablas de Almacenamiento](#tablas-de-almacenamiento)

---

## Tablas de Configuración

### Enterprise
Tabla que almacena la información de las empresas del sistema.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| EnterpriseId | UNIQUEIDENTIFIER | No | Identificador único de la empresa | PRIMARY KEY, DEFAULT NEWID() |
| Name | NVARCHAR(255) | No | Nombre de la empresa | |
| Code | NVARCHAR(50) | No | Código único de la empresa | UNIQUE |
| Address | NVARCHAR(500) | Sí | Dirección de la empresa | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### Stores
Tabla que almacena la información de las tiendas asociadas a cada empresa.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| StoreId | UNIQUEIDENTIFIER | No | Identificador único de la tienda | PRIMARY KEY, DEFAULT NEWID() |
| Name | NVARCHAR(255) | No | Nombre de la tienda | |
| Code | NVARCHAR(50) | No | Código único de la tienda | UNIQUE |
| Address | NVARCHAR(500) | Sí | Dirección de la tienda | |
| EnterpriseId | UNIQUEIDENTIFIER | No | Identificador de la empresa | FK a Enterprise |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### ScaleCompany
Tabla que define las escalas de calificación por empresa.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| ScaleCompanyId | UNIQUEIDENTIFIER | No | Identificador único de la escala | PRIMARY KEY, DEFAULT NEWID() |
| EnterpriseId | UNIQUEIDENTIFIER | Sí | Identificador de la empresa | FK a Enterprise |
| Code | NVARCHAR(10) | No | Código de la escala | |
| Name | NVARCHAR(200) | No | Nombre de la escala | |
| MinValue | DECIMAL(10,2) | No | Valor mínimo de la escala | |
| MaxValue | DECIMAL(10,2) | No | Valor máximo de la escala | |
| ColorCode | NVARCHAR(20) | Sí | Código de color (Hexadecimal) | |
| SortOrder | INT | Sí | Orden de la escala | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### Group
Tabla que define los grupos de auditoría por empresa.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| GroupId | UNIQUEIDENTIFIER | No | Identificador único del grupo | PRIMARY KEY, DEFAULT NEWID() |
| EnterpriseId | UNIQUEIDENTIFIER | No | Identificador de la empresa | FK a Enterprise |
| Name | NVARCHAR(200) | No | Nombre del grupo | |
| Weighting | DECIMAL(5,2) | No | Ponderación del grupo | |
| SortOrder | INT | No | Orden del grupo | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### ScaleGroup
Tabla que define los subgrupos de escala dentro de cada grupo.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| ScaleGroupId | UNIQUEIDENTIFIER | No | Identificador único del grupo de escala | PRIMARY KEY, DEFAULT NEWID() |
| GroupId | UNIQUEIDENTIFIER | No | Identificador del grupo | FK a Group |
| Code | NVARCHAR(10) | No | Código del grupo de escala | |
| Name | NVARCHAR(200) | No | Nombre del grupo de escala | |
| HasSourceData | BIT | Sí | Indica si tiene datos fuente | DEFAULT 0 |
| SortOrder | INT | Sí | Orden del grupo de escala | DEFAULT 0 |
| Weighting | DECIMAL(5,2) | No | Ponderación del grupo | |
| Recommendation | NVARCHAR(1000) | Sí | Recomendación | |
| Impact | NVARCHAR(1000) | Sí | Impacto | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### TableScaleTemplate
Tabla que almacena las plantillas de tabla para las escalas.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| TableScaleTemplateId | UNIQUEIDENTIFIER | No | Identificador único de la plantilla | PRIMARY KEY, DEFAULT NEWID() |
| ScaleGroupId | UNIQUEIDENTIFIER | No | Identificador del grupo de escala | FK a ScaleGroup |
| Code | NVARCHAR(50) | No | Código de la plantilla | |
| Name | NVARCHAR(255) | No | Nombre de la plantilla | |
| Orientation | NVARCHAR(2) | Sí | Orientación: 'H' (horizontal) o 'V' (vertical) | DEFAULT 'V', CHECK IN ('H','V') |
| SortOrder | INT | Sí | Orden de la plantilla | DEFAULT 0 |
| TemplateData | NVARCHAR(MAX) | Sí | Datos de la plantilla en formato JSON | CHECK ISJSON() = 1 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### AuditTemplateFields
Tabla que define los campos de las plantillas de auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| AuditTemplateFieldId | UNIQUEIDENTIFIER | No | Identificador único del campo | PRIMARY KEY, DEFAULT NEWID() |
| TableScaleTemplateId | UNIQUEIDENTIFIER | No | Identificador de la plantilla | FK a TableScaleTemplate |
| FieldCode | NVARCHAR(100) | No | Código del campo | |
| FieldName | NVARCHAR(255) | No | Nombre del campo | |
| FieldType | NVARCHAR(50) | No | Tipo de campo: numeric, text, date, boolean, select, image | |
| IsCalculated | VARCHAR(50) | Sí | Indica si es un campo calculado | |
| CalculationFormula | NVARCHAR(2000) | Sí | Fórmula de cálculo | |
| AcumulationType | NVARCHAR(50) | Sí | Tipo de acumulación: SUM, COUNT | CHECK IN ('SUM','COUNT') |
| FieldOptions | NVARCHAR(MAX) | Sí | Opciones para campos tipo select (JSON) | CHECK ISJSON() = 1 |
| SortOrder | INT | Sí | Orden del campo | DEFAULT 0 |
| DefaultValue | NVARCHAR(MAX) | Sí | Valor por defecto | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_AuditFieldValues_FieldCode (FieldCode)

---

### MaintenanceTable
Tabla que define las tablas de mantenimiento del sistema.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| MaintenanceTableId | UNIQUEIDENTIFIER | No | Identificador único de la tabla de mantenimiento | PRIMARY KEY, DEFAULT NEWID() |
| Code | NVARCHAR(50) | No | Código de la tabla | |
| Description | NVARCHAR(255) | Sí | Descripción de la tabla | |
| IsSystem | BIT | No | Indica si es una tabla del sistema | DEFAULT 0 |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreationDate | DATETIME | No | Fecha de creación | DEFAULT GETDATE() |
| CreatedBy | NVARCHAR(100) | No | Usuario que creó el registro | |
| UpdateDate | DATETIME | Sí | Fecha de actualización | |
| UpdatedBy | NVARCHAR(100) | Sí | Usuario que actualizó el registro | |

---

### MaintenanceDetailTable
Tabla que almacena los detalles de las tablas de mantenimiento.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| MaintenanceDetailTableId | UNIQUEIDENTIFIER | No | Identificador único del detalle | PRIMARY KEY, DEFAULT NEWID() |
| MaintenanceTableId | UNIQUEIDENTIFIER | No | Identificador de la tabla de mantenimiento | FK a MaintenanceTable |
| Code | NVARCHAR(50) | No | Código del detalle | |
| Description | NVARCHAR(255) | Sí | Descripción del detalle | |
| JsonData | NVARCHAR(MAX) | Sí | Datos adicionales en formato JSON | |
| OrderRow | INT | Sí | Orden de la fila | |
| IsDefault | BIT | No | Indica si es el valor por defecto | DEFAULT 0 |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreationDate | DATETIME | No | Fecha de creación | DEFAULT GETDATE() |
| CreatedBy | NVARCHAR(100) | No | Usuario que creó el registro | |
| UpdateDate | DATETIME | Sí | Fecha de actualización | |
| UpdatedBy | NVARCHAR(100) | Sí | Usuario que actualizó el registro | |

---

### AuditStatus
Tabla que define los estados posibles de una auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| AuditStatusId | UNIQUEIDENTIFIER | No | Identificador único del estado | PRIMARY KEY, DEFAULT NEWID() |
| Name | NVARCHAR(255) | No | Nombre del estado | |
| Code | NVARCHAR(10) | No | Código único del estado | UNIQUE |
| ColorCode | NVARCHAR(10) | Sí | Código de color del estado | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

## Tablas de Criterios de Puntuación

### ScoringCriteria
Tabla que define los criterios de puntuación para las auditorías.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| ScoringCriteriaId | UNIQUEIDENTIFIER | No | Identificador único del criterio | PRIMARY KEY, DEFAULT NEWID() |
| ScaleGroupId | UNIQUEIDENTIFIER | No | Identificador del grupo de escala | FK a ScaleGroup |
| CriteriaCode | NVARCHAR(10) | No | Código del criterio (AUTO: SR-{4 dígitos}) | |
| CriteriaName | NVARCHAR(255) | No | Nombre del criterio | |
| ResultFormula | NVARCHAR(2000) | Sí | Fórmula para calcular el resultado | |
| ComparisonOperator | NVARCHAR(20) | No | Operador de comparación: =, !=, >, <, >=, <=, BETWEEN, IN, CONTAINS | |
| ExpectedValue | NVARCHAR(255) | No | Valor esperado | |
| Score | DECIMAL(10,2) | No | Puntaje otorgado si cumple | |
| SortOrder | INT | Sí | Orden de evaluación | DEFAULT 0 |
| ErrorMessage | NVARCHAR(500) | Sí | Mensaje si no cumple el criterio | |
| SuccessMessage | NVARCHAR(500) | Sí | Mensaje si cumple el criterio | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_ScoringCriteria_ScaleGroupId (ScaleGroupId)

---

### CriteriaSubResult
Tabla que define los sub-resultados de los criterios.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| CriteriaSubResultId | UNIQUEIDENTIFIER | No | Identificador único del sub-criterio | PRIMARY KEY, DEFAULT NEWID() |
| ScaleGroupId | UNIQUEIDENTIFIER | No | Identificador del grupo de escala | FK a ScaleGroup |
| CriteriaName | NVARCHAR(255) | No | Nombre del criterio | |
| CriteriaCode | NVARCHAR(10) | Sí | Código del criterio (AUTO: CSR-{4 dígitos}) | |
| ResultFormula | NVARCHAR(2000) | Sí | Fórmula para calcular el resultado | |
| ColorCode | NVARCHAR(20) | No | Código de color para la evaluación | |
| Score | DECIMAL(10,2) | Sí | Puntaje otorgado si cumple | |
| ForSummary | BIT | No | Indica si se incluye en el resumen | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_ScoringCriteria_ScaleGroupId (ScaleGroupId)

---

## Tablas de Gestión de Usuarios

### UserReference
Tabla que almacena la referencia de usuarios del sistema.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| UserReferenceId | UNIQUEIDENTIFIER | No | Identificador único del usuario | PRIMARY KEY, DEFAULT NEWID() |
| UserId | UNIQUEIDENTIFIER | Sí | Identificador del usuario en Security MS | |
| EmployeeId | UNIQUEIDENTIFIER | Sí | Identificador del empleado en Memos MS | |
| FirstName | NVARCHAR(200) | No | Nombre del usuario | |
| LastName | NVARCHAR(200) | No | Apellido del usuario | |
| Email | NVARCHAR(150) | Sí | Email corporativo | |
| PersonalEmail | NVARCHAR(150) | Sí | Email personal | |
| DocumentNumber | NVARCHAR(20) | Sí | Número de documento | |
| RoleCode | NVARCHAR(50) | Sí | Código del rol | |
| RoleName | NVARCHAR(100) | Sí | Nombre del rol | |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | NVARCHAR(100) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | No | Fecha de creación del registro | DEFAULT SYSUTCDATETIME() |
| UpdatedBy | NVARCHAR(100) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### EmployeeStores
Tabla que relaciona empleados con las tiendas asignadas.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| EmployeeStoreId | UNIQUEIDENTIFIER | No | Identificador único de la relación | PRIMARY KEY, DEFAULT NEWID() |
| UserReferenceId | UNIQUEIDENTIFIER | No | Identificador del usuario | FK a UserReference |
| StoreId | UNIQUEIDENTIFIER | No | Identificador de la tienda | FK a Stores |
| AssignmentDate | DATE | No | Fecha de asignación | DEFAULT GETDATE() |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Constraints:**
- UQ_EmployeeStore UNIQUE (UserReferenceId, StoreId)

---

### AuditRoleConfiguration
Tabla que configura los roles requeridos para las auditorías.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| AuditRoleConfigurationId | UNIQUEIDENTIFIER | No | Identificador único de la configuración | PRIMARY KEY, DEFAULT NEWID() |
| RoleCode | NVARCHAR(10) | No | Código del rol | UNIQUE |
| RoleName | NVARCHAR(100) | No | Nombre del rol | |
| IsRequired | BIT | Sí | Indica si el rol es requerido | DEFAULT 1 |
| AllowMultiple | BIT | Sí | Permite múltiples usuarios con este rol | DEFAULT 0 |
| SequenceOrder | INT | Sí | Orden de secuencia | |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | NVARCHAR(100) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | No | Fecha de creación del registro | DEFAULT SYSUTCDATETIME() |
| UpdatedBy | NVARCHAR(100) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_AuditRoleConfiguration_RoleCode (RoleCode)

---

### PeriodAuditParticipant
Tabla que almacena los participantes asignados a un período de auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditParticipantId | UNIQUEIDENTIFIER | No | Identificador único del participante | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditId | UNIQUEIDENTIFIER | No | Identificador del período de auditoría | FK a PeriodAudit |
| UserReferenceId | UNIQUEIDENTIFIER | No | Identificador del usuario | FK a UserReference |
| RoleCodeSnapshot | NVARCHAR(10) | No | Código del rol al momento de asignación | |
| RoleNameSnapshot | NVARCHAR(100) | No | Nombre del rol al momento de asignación | |
| Comments | NVARCHAR(500) | Sí | Comentarios adicionales | |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | NVARCHAR(100) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | No | Fecha de creación del registro | DEFAULT SYSUTCDATETIME() |
| UpdatedBy | NVARCHAR(100) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_PeriodAuditParticipant_PeriodAuditId (PeriodAuditId)
- IX_PeriodAuditParticipant_UserReferenceId (UserReferenceId)

---

### SubstitutionHistory
Tabla que registra la historia de suplencias de usuarios en auditorías.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| SubstitutionHistoryId | UNIQUEIDENTIFIER | No | Identificador único de la suplencia | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditId | UNIQUEIDENTIFIER | No | Identificador del período de auditoría | FK a PeriodAudit |
| RoleName | NVARCHAR(100) | No | Nombre del rol en el contexto de la auditoría | |
| PreviousUserReferenceId | UNIQUEIDENTIFIER | Sí | Identificador del usuario anterior | FK a UserReference |
| NewUserReferenceId | UNIQUEIDENTIFIER | No | Identificador del nuevo usuario asignado | FK a UserReference |
| ChangeReason | NVARCHAR(255) | Sí | Razón del cambio | |
| IsActive | BIT | No | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | NVARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | No | Fecha de creación del registro | DEFAULT SYSUTCDATETIME() |
| UpdatedBy | NVARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_SubstitutionHistory_PeriodAuditId (PeriodAuditId)
- IX_SubstitutionHistory_PreviousUserReferenceId (PreviousUserReferenceId)
- IX_SubstitutionHistory_NewUserReferenceId (NewUserReferenceId)

---

## Tablas de Proceso de Auditoría

### PeriodAudit
Tabla principal que almacena los períodos de auditoría (encabezado).

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditId | UNIQUEIDENTIFIER | No | Identificador único de la auditoría | PRIMARY KEY, DEFAULT NEWID() |
| StoreId | UNIQUEIDENTIFIER | Sí | Identificador de la tienda | FK a Stores |
| StartDate | DATE | No | Fecha de inicio del período | |
| EndDate | DATE | No | Fecha de fin del período | |
| ReportDate | DATE | Sí | Fecha del reporte | |
| AuditedDays | INT | Sí | Días auditados | |
| GlobalObservations | NVARCHAR(MAX) | No | Observaciones globales | |
| TotalWeighting | DECIMAL(5,2) | No | Ponderación total | |
| StatusId | UNIQUEIDENTIFIER | Sí | Identificador del estado | FK a AuditStatus |
| CorrelativeNumber | VARCHAR(50) | Sí | Número correlativo de la auditoría | |
| ScoreValue | DECIMAL(10,2) | No | Valor de puntuación obtenido | |
| ScaleCode | NVARCHAR(10) | Sí | Código de la escala | |
| ScaleName | NVARCHAR(100) | No | Nombre de la escala | |
| ScaleColor | NVARCHAR(10) | No | Color de la escala | |
| ScaleMinValue | DECIMAL(10,2) | No | Valor mínimo de la escala | |
| ScaleMaxValue | DECIMAL(10,2) | No | Valor máximo de la escala | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_PeriodAudit_ScaleCode_Optimized (ScaleCode, IsActive, StoreId, CreationDate) INCLUDE (ScaleName, ScaleColor, ScoreValue, ScaleMinValue, ScaleMaxValue, StatusId)
- IX_PeriodAudit_ScaleCode_DateRange (ScaleCode, CreationDate, IsActive) INCLUDE (StoreId, ScoreValue, StatusId) WHERE IsActive = 1
- IX_PeriodAudit_ScaleCode_Statistics (ScaleCode, StatusId, IsActive) INCLUDE (ScoreValue, StoreId, CreationDate)

---

### InboxItems
Tabla que representa la bandeja de auditorías para gestión de flujo de trabajo.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| InboxItemId | UNIQUEIDENTIFIER | No | Identificador único del ítem | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditId | UNIQUEIDENTIFIER | Sí | Identificador de la auditoría | FK a PeriodAudit ON DELETE CASCADE |
| PrevStatusId | UNIQUEIDENTIFIER | Sí | Identificador del estado anterior | FK a AuditStatus |
| NextStatusId | UNIQUEIDENTIFIER | Sí | Identificador del siguiente estado | FK a AuditStatus |
| PrevUserId | UNIQUEIDENTIFIER | Sí | Identificador del usuario anterior | FK a UserReference |
| NextUserId | UNIQUEIDENTIFIER | Sí | Identificador del siguiente usuario | FK a UserReference |
| ApproverId | UNIQUEIDENTIFIER | Sí | Identificador del usuario aprobador | FK a UserReference |
| Comments | NVARCHAR(MAX) | Sí | Comentarios | |
| UserId | UNIQUEIDENTIFIER | Sí | Usuario que registró la acción | FK a UserReference |
| Action | NVARCHAR(100) | Sí | Acción realizada: Aprobada, Cancelada, Devuelta | |
| SequenceNumber | INT | No | Número secuencial por PeriodAudit | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_InboxItems_PeriodAuditId (PeriodAuditId)
- IX_InboxItems_NextStatusId (NextStatusId)

---

## Tablas de Resultados

### PeriodAuditGroupResult
Tabla que almacena los resultados por grupo de una auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditGroupResultId | UNIQUEIDENTIFIER | No | Identificador único del resultado de grupo | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditId | UNIQUEIDENTIFIER | No | Identificador de la auditoría | FK a PeriodAudit |
| GroupId | UNIQUEIDENTIFIER | No | Identificador del grupo | FK a Group |
| ScoreValue | DECIMAL(10,2) | No | Valor obtenido | |
| SortOrder | INT | No | Orden | DEFAULT 0 |
| Observations | NVARCHAR(150) | Sí | Observaciones | |
| ScaleDescription | NVARCHAR(150) | Sí | Descripción de la escala | |
| TotalWeighting | DECIMAL(5,2) | No | Ponderación total | |
| ScaleColor | NVARCHAR(20) | Sí | Código de color del grupo | |
| HasEvidence | BIT | Sí | Indica si tiene evidencia | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### PeriodAuditScaleResult
Tabla que almacena los resultados por escala de grupo de una auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditScaleResultId | UNIQUEIDENTIFIER | No | Identificador único del resultado de escala | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditGroupResultId | UNIQUEIDENTIFIER | No | Identificador del resultado de grupo | FK a PeriodAuditGroupResult |
| ScaleGroupId | UNIQUEIDENTIFIER | No | Identificador del grupo de escala | FK a ScaleGroup |
| ScoreValue | DECIMAL(10,2) | No | Valor total | |
| AppliedWeighting | DECIMAL(5,2) | No | Ponderación aplicada | |
| SortOrder | INT | Sí | Orden | DEFAULT 0 |
| Observations | NVARCHAR(MAX) | Sí | Observaciones | |
| Impact | NVARCHAR(MAX) | Sí | Impacto | |
| Recommendation | NVARCHAR(MAX) | Sí | Recomendación | |
| Valorized | NVARCHAR(MAX) | Sí | Valorizado | |
| ScaleDescription | NVARCHAR(150) | Sí | Descripción de la escala | |
| ScaleColor | NVARCHAR(20) | Sí | Código de color de la escala | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### PeriodAuditTableScaleTemplateResult
Tabla que almacena los resultados de plantillas de tabla por escala.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditTableScaleTemplateResultId | UNIQUEIDENTIFIER | No | Identificador único del resultado de plantilla | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditScaleResultId | UNIQUEIDENTIFIER | No | Identificador del resultado de escala | FK a PeriodAuditScaleResult ON DELETE CASCADE |
| TableScaleTemplateId | UNIQUEIDENTIFIER | Sí | Identificador de la plantilla original | |
| Code | NVARCHAR(50) | No | Código de la plantilla | |
| Name | NVARCHAR(255) | No | Nombre de la plantilla | |
| Orientation | NVARCHAR(2) | Sí | Orientación: 'H' o 'V' | DEFAULT 'V', CHECK IN ('H','V') |
| TemplateData | NVARCHAR(MAX) | Sí | Datos de la plantilla en formato JSON | CHECK ISJSON() = 1 |
| SortOrder | INT | Sí | Orden | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### PeriodAuditFieldValues
Tabla que almacena los valores capturados de los campos de auditoría.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditFieldValueId | UNIQUEIDENTIFIER | No | Identificador único del valor de campo | PRIMARY KEY, DEFAULT NEWID() |
| AuditTemplateFieldId | UNIQUEIDENTIFIER | Sí | Identificador del campo de plantilla original | |
| PeriodAuditTableScaleTemplateResultId | UNIQUEIDENTIFIER | Sí | Identificador del resultado de plantilla | FK a PeriodAuditTableScaleTemplateResult ON DELETE CASCADE |
| FieldCode | NVARCHAR(100) | No | Código del campo | |
| FieldName | NVARCHAR(255) | No | Nombre del campo | |
| FieldType | NVARCHAR(50) | No | Tipo de campo | |
| IsCalculated | NVARCHAR(50) | Sí | Indica si es calculado | |
| CalculationFormula | NVARCHAR(2000) | Sí | Fórmula de cálculo | |
| AcumulationType | NVARCHAR(50) | Sí | Tipo de acumulación: SUM, COUNT | CHECK IN ('SUM','COUNT') |
| FieldOptions | NVARCHAR(MAX) | Sí | Opciones del campo (JSON) | |
| TableDataHorizontal | NVARCHAR(MAX) | Sí | Datos de tabla horizontal (JSON) | |
| SortOrder | INT | Sí | Orden | DEFAULT 0 |
| TextValue | NVARCHAR(MAX) | Sí | Valor de texto capturado | |
| NumericValue | DECIMAL(18,4) | Sí | Valor numérico capturado | |
| DefaultValue | VARCHAR(250) | Sí | Valor por defecto | |
| DateValue | DATETIME2 | Sí | Valor de fecha capturado | |
| BooleanValue | BIT | Sí | Valor booleano capturado | |
| ImageUrl | NVARCHAR(500) | Sí | URL o path de imagen | |
| FieldOptionsValue | NVARCHAR(255) | Sí | Valor seleccionado para campos tipo select | |
| ValidationStatus | NVARCHAR(50) | Sí | Estado de validación: valid, invalid, pending, warning | |
| ValidationMessage | NVARCHAR(500) | Sí | Mensaje de validación | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Índices:**
- IX_PeriodAuditFieldValues_FieldCode (FieldCode)
- IX_PeriodAuditFieldValues_NumericValue (NumericValue) WHERE NumericValue IS NOT NULL

---

### PeriodAuditScoringCriteriaResult
Tabla que almacena los resultados de evaluación de criterios de puntuación.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditScoringCriteriaResultId | UNIQUEIDENTIFIER | No | Identificador único del resultado de criterio | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditScaleResultId | UNIQUEIDENTIFIER | No | Identificador del resultado de escala | FK a PeriodAuditScaleResult ON DELETE CASCADE |
| CriteriaName | NVARCHAR(255) | No | Nombre del criterio | |
| CriteriaCode | NVARCHAR(10) | Sí | Código del criterio | |
| ResultFormula | NVARCHAR(2000) | Sí | Fórmula para calcular resultado | |
| ComparisonOperator | NVARCHAR(20) | No | Operador de comparación | |
| ExpectedValue | NVARCHAR(255) | No | Valor esperado | |
| Score | DECIMAL(10,2) | No | Puntaje otorgado si cumple | |
| SortOrder | INT | Sí | Orden de evaluación | DEFAULT 0 |
| ErrorMessage | NVARCHAR(500) | Sí | Mensaje si no cumple | |
| SuccessMessage | NVARCHAR(500) | Sí | Mensaje si cumple | |
| ResultObtained | DECIMAL(10,2) | Sí | Puntaje obtenido | |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

### PeriodAuditScaleSubResult
Tabla que almacena los resultados de evaluación de sub-criterios.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| PeriodAuditScaleSubResultId | UNIQUEIDENTIFIER | No | Identificador único del sub-resultado | PRIMARY KEY, DEFAULT NEWID() |
| PeriodAuditScaleResultId | UNIQUEIDENTIFIER | No | Identificador del resultado de escala | FK a PeriodAuditScaleResult ON DELETE CASCADE |
| CriteriaSubResultId | UNIQUEIDENTIFIER | No | Identificador del sub-criterio | FK a CriteriaSubResult |
| CriteriaCode | NVARCHAR(10) | Sí | Código del criterio | |
| CriteriaName | NVARCHAR(255) | No | Nombre del criterio | |
| EvaluatedValue | NVARCHAR(255) | Sí | Valor que se evaluó | |
| CalculatedResult | NVARCHAR(255) | Sí | Resultado de aplicar la fórmula | |
| AppliedFormula | NVARCHAR(2000) | Sí | Fórmula que se aplicó | |
| ScoreObtained | DECIMAL(10,2) | Sí | Puntaje obtenido | |
| ColorCode | NVARCHAR(20) | Sí | Código de color del resultado | |
| ForSummary | BIT | No | Indica si se incluye en el resumen | DEFAULT 0 |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

**Constraints:**
- UNIQUE (PeriodAuditScaleResultId, CriteriaSubResultId)

**Índices:**
- IX_PeriodAuditScaleSubResult_ScaleResultId (PeriodAuditScaleResultId)
- IX_PeriodAuditScaleSubResult_CriteriaId (CriteriaSubResultId)

---

## Tablas de Almacenamiento

### StorageFiles
Tabla que almacena información de archivos adjuntos y evidencias.

| Campo | Tipo de Dato | Nulable | Descripción | Constraints |
|-------|-------------|---------|-------------|-------------|
| StorageFileId | UNIQUEIDENTIFIER | No | Identificador único del archivo | PRIMARY KEY, DEFAULT NEWID() |
| EntityId | UNIQUEIDENTIFIER | Sí | Identificador de la entidad relacionada | |
| EntityName | NVARCHAR(255) | Sí | Nombre de la entidad relacionada | |
| ClassificationType | NVARCHAR(100) | Sí | Tipo de clasificación del archivo | |
| OriginalName | NVARCHAR(255) | No | Nombre original del archivo | |
| FileName | NVARCHAR(255) | No | Nombre del archivo almacenado | |
| FileUrl | NVARCHAR(500) | No | URL o path del archivo | |
| FileType | NVARCHAR(50) | Sí | Tipo de archivo (extensión) | |
| UploadedBy | VARCHAR(120) | Sí | Usuario que subió el archivo | |
| UploadDate | DATETIME2 | Sí | Fecha de carga del archivo | DEFAULT GETDATE() |
| IsActive | BIT | Sí | Indica si el registro está activo | DEFAULT 1 |
| CreatedBy | VARCHAR(120) | Sí | Usuario que creó el registro | |
| CreationDate | DATETIME2 | Sí | Fecha de creación del registro | DEFAULT GETDATE() |
| UpdatedBy | VARCHAR(120) | Sí | Usuario que actualizó el registro | |
| UpdateDate | DATETIME2 | Sí | Fecha de actualización del registro | |

---

## Relaciones Entre Tablas

### Relaciones Principales

1. **Enterprise → Stores** (1:N)
   - Una empresa puede tener múltiples tiendas

2. **Enterprise → ScaleCompany** (1:N)
   - Una empresa puede tener múltiples escalas de calificación

3. **Enterprise → Group** (1:N)
   - Una empresa puede tener múltiples grupos de auditoría

4. **Group → ScaleGroup** (1:N)
   - Un grupo puede tener múltiples subgrupos de escala

5. **ScaleGroup → TableScaleTemplate** (1:N)
   - Un subgrupo de escala puede tener múltiples plantillas de tabla

6. **TableScaleTemplate → AuditTemplateFields** (1:N)
   - Una plantilla puede tener múltiples campos

7. **ScaleGroup → ScoringCriteria** (1:N)
   - Un subgrupo de escala puede tener múltiples criterios de puntuación

8. **ScaleGroup → CriteriaSubResult** (1:N)
   - Un subgrupo de escala puede tener múltiples sub-criterios

9. **Stores → PeriodAudit** (1:N)
   - Una tienda puede tener múltiples períodos de auditoría

10. **PeriodAudit → PeriodAuditGroupResult** (1:N)
    - Una auditoría puede tener múltiples resultados de grupo

11. **PeriodAuditGroupResult → PeriodAuditScaleResult** (1:N)
    - Un resultado de grupo puede tener múltiples resultados de escala

12. **PeriodAuditScaleResult → PeriodAuditTableScaleTemplateResult** (1:N)
    - Un resultado de escala puede tener múltiples resultados de plantilla

13. **PeriodAuditTableScaleTemplateResult → PeriodAuditFieldValues** (1:N)
    - Un resultado de plantilla puede tener múltiples valores de campo

14. **UserReference → EmployeeStores** (1:N)
    - Un usuario puede estar asignado a múltiples tiendas

15. **PeriodAudit → PeriodAuditParticipant** (1:N)
    - Una auditoría puede tener múltiples participantes

16. **PeriodAudit → InboxItems** (1:N)
    - Una auditoría puede tener múltiples ítems en la bandeja

17. **PeriodAudit → SubstitutionHistory** (1:N)
    - Una auditoría puede tener múltiple historial de suplencias

---

## Notas Adicionales

### Campos de Auditoría Comunes
La mayoría de las tablas incluyen los siguientes campos de auditoría:
- **IsActive**: Indica si el registro está activo (soft delete)
- **CreatedBy**: Usuario que creó el registro
- **CreationDate**: Fecha y hora de creación
- **UpdatedBy**: Usuario que actualizó el registro por última vez
- **UpdateDate**: Fecha y hora de la última actualización

### Validaciones JSON
Varias tablas incluyen campos que almacenan datos en formato JSON con validación:
- **TableScaleTemplate.TemplateData**
- **AuditTemplateFields.FieldOptions**
- **PeriodAuditTableScaleTemplateResult.TemplateData**
- **PeriodAuditFieldValues.FieldOptions**
- **MaintenanceDetailTable.JsonData**

### Estrategia de Desnormalización
Para mantener la integridad histórica de las auditorías, varias tablas de resultados almacenan copias desnormalizadas de datos de configuración que pueden cambiar en el tiempo. Esto asegura que los reportes históricos reflejen exactamente los valores y configuraciones que estaban vigentes al momento de la auditoría.

### Índices de Rendimiento
Se han creado índices especializados en la tabla **PeriodAudit** para optimizar:
- Consultas por código de escala
- Filtrado por rangos de fecha
- Generación de estadísticas
- Consultas de reportes

---

**Última actualización**: Diciembre 2025  
**Versión**: 1.0
