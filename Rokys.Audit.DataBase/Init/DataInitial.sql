-- =============================================
-- TABLAS DE CONFIGURACIÓN
-- =============================================

-- Tabla: Escala por Empresa
CREATE TABLE [Group]
(
    GroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EnterpriseId UNIQUEIDENTIFIER NOT NULL, 

    Description NVARCHAR(200) NOT NULL,

    -- Objetivo general para el grupo
    ObjectiveValue DECIMAL(10,2) NULL,

    -- Umbrales para el grupo
    RiskLow DECIMAL(10,2) NULL,
    RiskModerate DECIMAL(10,2) NULL,
    RiskHigh DECIMAL(10,2) NULL,

    -- Riesgo crítico = mayor a RiesgoElevado

    -- Ponderación del grupo
    GroupWeight DECIMAL(5,2) NULL,

    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

CREATE TABLE ScaleGroup
(
    ScaleGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES [Group](GroupId),
    Description NVARCHAR(200) NOT NULL,

    -- General objective for the group
    ObjectiveValue DECIMAL(10,2) NULL,

    -- Thresholds for the group
    LowRisk DECIMAL(10,2) NULL,
    ModerateRisk DECIMAL(10,2) NULL,
    HighRisk DECIMAL(10,2) NULL,
    
    -- Critical risk = greater than HighRisk

    -- Group weighting
    Weighting DECIMAL(5,2) NULL,


    -- Audit fields
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

-- Table: Risk Scale Group
CREATE TABLE RiskScaleGroup
(
    RiskScaleGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES [ScaleGroup](ScaleGroupId),
    Description NVARCHAR(200) NOT NULL,

    -- General objective for the group
    ObjectiveValue DECIMAL(10,2) NULL,

    -- Thresholds for the group
    LowRisk DECIMAL(10,2) NULL,
    ModerateRisk DECIMAL(10,2) NULL,
    HighRisk DECIMAL(10,2) NULL,
    
    -- Critical risk = greater than HighRisk

    -- Group weighting
    Weighting DECIMAL(5,2) NULL,

    -- Audit fields
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

-- Table: Risk Scale (Detail)
CREATE TABLE RiskScale
(
    RiskScaleId INT IDENTITY(1,1) PRIMARY KEY,

    -- Relationship with the group
    RiskScaleGroupId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES RiskScaleGroup(RiskScaleGroupId),

    Code NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    ShortDescription NVARCHAR(100) NULL,

    ObjectiveValue DECIMAL(10,2) NULL,

    -- Thresholds per auditable point
    LowRisk DECIMAL(10,2) NULL,
    ModerateRisk DECIMAL(10,2) NULL,
    HighRisk DECIMAL(10,2) NULL,

    Weighting DECIMAL(5,2) NULL,
    NonToleratedRisk BIT NOT NULL DEFAULT 0,

    -- Audit fields
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

-- =============================================
-- AUDIT PROCESS TABLES
-- =============================================

-- Table: Monthly Audit (Header)
CREATE TABLE Audit
(
    AuditId INT IDENTITY(1,1) PRIMARY KEY,

    -- Store / audit identification
    StoreId INT NULL,
    StoreName NVARCHAR(150) NOT NULL,

    -- Participants
    Administrator NVARCHAR(150) NULL,
    Assistant NVARCHAR(150) NULL,
    OperationManagers NVARCHAR(200) NULL,
    FloatingAdministrator NVARCHAR(150) NULL,
    ResponsibleAuditor NVARCHAR(150) NULL,

    -- Dates
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    ReportDate DATE NULL,

    -- Additional information
    AuditedDays INT NULL,
    GlobalObservations NVARCHAR(MAX) NULL,
    TotalWeighting DECIMAL(5,2) NULL,

    -- Record audit
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

-- Table: Result per Auditable Point
CREATE TABLE AuditResult
(
    AuditResultId INT IDENTITY(1,1) PRIMARY KEY,
    AuditId INT NOT NULL
        FOREIGN KEY REFERENCES Audit(AuditId),
    RiskScaleId INT NOT NULL
        FOREIGN KEY REFERENCES RiskScale(RiskScaleId),

    -- Calculation data at the time of audit
    ObtainedValue DECIMAL(10,2) NOT NULL,
    RiskLevel NVARCHAR(20) NULL,

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NULL,
    AppliedLowThreshold DECIMAL(10,2) NULL,
    AppliedModerateThreshold DECIMAL(10,2) NULL,
    AppliedHighThreshold DECIMAL(10,2) NULL,

    Observations NVARCHAR(MAX) NULL,

    -- Record audit
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

-- Table: Aggregated Result by Risk Group
CREATE TABLE AuditResultGroup
(
    AuditResultGroupId INT IDENTITY(1,1) PRIMARY KEY,
    MonthlyAuditId INT NOT NULL
        FOREIGN KEY REFERENCES MonthlyAudit(MonthlyAuditId),
    RiskScaleGroupId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES RiskScaleGroup(RiskScaleGroupId),

    -- Calculation data at the time of audit
    TotalValue DECIMAL(10,2) NULL,
    RiskLevel NVARCHAR(20) NULL,

    -- Historical weighting and thresholds
    AppliedWeighting DECIMAL(5,2) NULL,
    AppliedLowThreshold DECIMAL(10,2) NULL,
    AppliedModerateThreshold DECIMAL(10,2) NULL,
    AppliedHighThreshold DECIMAL(10,2) NULL,

    Observations NVARCHAR(MAX) NULL,

    -- Record audit
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);
