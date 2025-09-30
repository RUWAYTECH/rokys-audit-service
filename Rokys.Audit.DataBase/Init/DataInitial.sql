-- =============================================
-- TABLAS DE CONFIGURACIÓN
-- =============================================

--Company
--Store


CREATE TABLE [ScaleCompany](
    ScaleCompanyId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de la Escala por Empresa
    EnterpriseId NVARCHAR(200) NOT NULL, -- Nombre de la Empresa

    Description NVARCHAR(200) NOT NULL, -- Descripción

    -- Objetivo general para la empresa
    ObjectiveValue DECIMAL(10,2) NOT NULL, -- Valor Objetivo

    -- Umbrales para la empresa
    RiskLow DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    RiskModerate DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    RiskHigh DECIMAL(10,2) NOT NULL, -- Riesgo Alto

    -- Riesgo crítico = mayor a RiesgoElevado

    -- Ponderación de la empresa
    Weighting DECIMAL(5,2) NOT NULL, -- Peso de la Empresa

    -- Auditoría    
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
)

-- Tabla: Escala por Empresa
CREATE TABLE [Group]
(
    GroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo
    EnterpriseId UNIQUEIDENTIFIER NOT NULL, -- ID de la Empresa

    Description NVARCHAR(200) NOT NULL, -- Descripción

    -- Objetivo general para el grupo
    ObjectiveValue DECIMAL(10,2) NOT NULL, -- Valor Objetivo

    -- Umbrales para el grupo
    RiskLow DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    RiskModerate DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    RiskHigh DECIMAL(10,2) NOT NULL, -- Riesgo Alto

    -- Riesgo crítico = mayor a RiesgoElevado

    -- Ponderación del grupo
    Weighting DECIMAL(5,2) NOT NULL, -- Peso del Grupo

    -- Auditoría
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

CREATE TABLE ScaleGroup
(
    ScaleGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo de Escala
    GroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo
        FOREIGN KEY REFERENCES [Group](GroupId),
    Description NVARCHAR(200) NOT NULL, -- Descripción

    -- General objective for the group
    ObjectiveValue DECIMAL(10,2) NOT NULL, -- Valor Objetivo

    -- Thresholds for the group
    LowRisk DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    ModerateRisk DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    HighRisk DECIMAL(10,2) NOT NULL, -- Riesgo Alto
    
    -- Critical risk = greater than HighRisk

    -- Group weighting
    Weighting DECIMAL(5,2) NOT NULL, -- Ponderación

    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);


--TODO



-- =============================================
-- AUDIT PROCESS TABLES
-- =============================================

-- Table: PeriodAudit Audit (Header)
CREATE TABLE [PeriodAudit]
(
    PeriodAuditId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Auditoría

    -- Store / audit identification
    StoreId INT NULL, -- ID de Tienda
    StoreName NVARCHAR(150) NOT NULL, -- Nombre de Tienda

    -- Participants
    Administrator NVARCHAR(150) NULL, -- Administrador
    Assistant NVARCHAR(150) NULL, -- Asistente
    OperationManagers NVARCHAR(200) NULL, -- Gerentes de Operación
    FloatingAdministrator NVARCHAR(150) NULL, -- Administrador Flotante
    ResponsibleAuditor NVARCHAR(150) NULL, -- Auditor Responsable

    -- Dates
    StartDate DATE NOT NULL, -- Fecha de Inicio
    EndDate DATE NOT NULL, -- Fecha de Fin
    ReportDate DATE NULL, -- Fecha de Reporte

    -- Additional information
    AuditedDays INT NULL, -- Días Auditados
    GlobalObservations NVARCHAR(MAX) NOT NULL, -- Observaciones Globales
    TotalWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Total

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Result per Auditable Point
CREATE TABLE PeriodAuditResult
(
    PeriodAuditResultId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Resultado de Auditoría
    PeriodAuditId INT NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAudit(PeriodAuditId),
    RiskScaleId INT NOT NULL -- ID de Escala de Riesgo
        FOREIGN KEY REFERENCES RiskScale(RiskScaleId),

    -- Calculation data at the time of audit
    ObtainedValue DECIMAL(10,2) NOT NULL, -- Valor Obtenido
    RiskLevel NVARCHAR(20) NULL, -- Nivel de Riesgo

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Aplicada
    AppliedLowThreshold DECIMAL(10,2) NOT NULL, -- Umbral Bajo Aplicado
    AppliedModerateThreshold DECIMAL(10,2) NOT NULL, -- Umbral Moderado Aplicado
    AppliedHighThreshold DECIMAL(10,2) NOT NULL, -- Umbral Alto Aplicado

    Observations NVARCHAR(MAX) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Aggregated Result by Risk Group
CREATE TABLE PeriodAuditResultGroup
(
    PeriodAuditResultGroupId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Resultado de Grupo de Auditoría
    PeriodAuditId INT NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES [PeriodAudit](PeriodAuditId),
    RiskScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala de Riesgo
        FOREIGN KEY REFERENCES RiskScaleGroup(RiskScaleGroupId),

    -- Calculation data at the time of audit
    TotalValue DECIMAL(10,2) NOT NULL, -- Valor Total
    RiskLevel NVARCHAR(20) NULL, -- Nivel de Riesgo

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Aplicada
    AppliedLowThreshold DECIMAL(10,2) NOT NULL, -- Umbral Bajo Aplicado
    AppliedModerateThreshold DECIMAL(10,2) NOT NULL, -- Umbral Moderado Aplicado
    AppliedHighThreshold DECIMAL(10,2) NOT NULL, -- Umbral Alto Aplicado

    Observations NVARCHAR(MAX) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);