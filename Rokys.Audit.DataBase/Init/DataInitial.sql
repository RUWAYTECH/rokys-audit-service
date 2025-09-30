-- =============================================
-- TABLAS DE CONFIGURACIÓN
-- =============================================

-- Tabla: Escala por Empresa
CREATE TABLE [Group]
(
    GroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo
    EnterpriseId UNIQUEIDENTIFIER NOT NULL, -- ID de la Empresa

    Description NVARCHAR(200) NOT NULL, -- Descripción

    -- Objetivo general para el grupo
    ObjectiveValue DECIMAL(10,2) NULL, -- Valor Objetivo

    -- Umbrales para el grupo
    RiskLow DECIMAL(10,2) NULL, -- Riesgo Bajo
    RiskModerate DECIMAL(10,2) NULL, -- Riesgo Moderado
    RiskHigh DECIMAL(10,2) NULL, -- Riesgo Alto

    -- Riesgo crítico = mayor a RiesgoElevado

    -- Ponderación del grupo
    GroupWeight DECIMAL(5,2) NULL, -- Peso del Grupo

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
    ObjectiveValue DECIMAL(10,2) NULL, -- Valor Objetivo

    -- Thresholds for the group
    LowRisk DECIMAL(10,2) NULL, -- Riesgo Bajo
    ModerateRisk DECIMAL(10,2) NULL, -- Riesgo Moderado
    HighRisk DECIMAL(10,2) NULL, -- Riesgo Alto
    
    -- Critical risk = greater than HighRisk

    -- Group weighting
    Weighting DECIMAL(5,2) NULL, -- Ponderación

    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Risk Scale Group
CREATE TABLE RiskScaleGroup
(
    RiskScaleGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo de Escala de Riesgo
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala
        FOREIGN KEY REFERENCES [ScaleGroup](ScaleGroupId),
    Description NVARCHAR(200) NOT NULL, -- Descripción

    -- General objective for the group
    ObjectiveValue DECIMAL(10,2) NULL, -- Valor Objetivo

    -- Thresholds for the group
    LowRisk DECIMAL(10,2) NULL, -- Riesgo Bajo
    ModerateRisk DECIMAL(10,2) NULL, -- Riesgo Moderado
    HighRisk DECIMAL(10,2) NULL, -- Riesgo Alto
    
    -- Critical risk = greater than HighRisk

    -- Group weighting
    Weighting DECIMAL(5,2) NULL, -- Ponderación

    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Risk Scale (Detail)
CREATE TABLE RiskScale
(
    RiskScaleId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Escala de Riesgo

    -- Relationship with the group
    RiskScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala de Riesgo
        FOREIGN KEY REFERENCES RiskScaleGroup(RiskScaleGroupId),

    Code NVARCHAR(50) NOT NULL, -- Código
    Description NVARCHAR(500) NOT NULL, -- Descripción
    ShortDescription NVARCHAR(100) NULL, -- Descripción Corta

    ObjectiveValue DECIMAL(10,2) NULL, -- Valor Objetivo

    -- Thresholds per auditable point
    LowRisk DECIMAL(10,2) NULL, -- Riesgo Bajo
    ModerateRisk DECIMAL(10,2) NULL, -- Riesgo Moderado
    HighRisk DECIMAL(10,2) NULL, -- Riesgo Alto

    Weighting DECIMAL(5,2) NULL, -- Ponderación
    NonToleratedRisk BIT NOT NULL DEFAULT 0, -- Riesgo No Tolerado

    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- =============================================
-- AUDIT PROCESS TABLES
-- =============================================

-- Table: Monthly Audit (Header)
CREATE TABLE [Audit]
(
    AuditId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Auditoría

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
    GlobalObservations NVARCHAR(MAX) NULL, -- Observaciones Globales
    TotalWeighting DECIMAL(5,2) NULL, -- Ponderación Total

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Result per Auditable Point
CREATE TABLE AuditResult
(
    AuditResultId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Resultado de Auditoría
    AuditId INT NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES Audit(AuditId),
    RiskScaleId INT NOT NULL -- ID de Escala de Riesgo
        FOREIGN KEY REFERENCES RiskScale(RiskScaleId),

    -- Calculation data at the time of audit
    ObtainedValue DECIMAL(10,2) NOT NULL, -- Valor Obtenido
    RiskLevel NVARCHAR(20) NULL, -- Nivel de Riesgo

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NULL, -- Ponderación Aplicada
    AppliedLowThreshold DECIMAL(10,2) NULL, -- Umbral Bajo Aplicado
    AppliedModerateThreshold DECIMAL(10,2) NULL, -- Umbral Moderado Aplicado
    AppliedHighThreshold DECIMAL(10,2) NULL, -- Umbral Alto Aplicado

    Observations NVARCHAR(MAX) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Table: Aggregated Result by Risk Group
CREATE TABLE AuditResultGroup
(
    AuditResultGroupId INT IDENTITY(1,1) PRIMARY KEY, -- ID de Resultado de Grupo de Auditoría
    AuditId INT NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES [Audit](AuditId),
    RiskScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala de Riesgo
        FOREIGN KEY REFERENCES RiskScaleGroup(RiskScaleGroupId),

    -- Calculation data at the time of audit
    TotalValue DECIMAL(10,2) NULL, -- Valor Total
    RiskLevel NVARCHAR(20) NULL, -- Nivel de Riesgo

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NULL, -- Ponderación Aplicada
    AppliedLowThreshold DECIMAL(10,2) NULL, -- Umbral Bajo Aplicado
    AppliedModerateThreshold DECIMAL(10,2) NULL, -- Umbral Moderado Aplicado
    AppliedHighThreshold DECIMAL(10,2) NULL, -- Umbral Alto Aplicado

    Observations NVARCHAR(MAX) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);