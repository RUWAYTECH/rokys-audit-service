-- =============================================
-- TABLAS DE CONFIGURACIÓN
-- =============================================

--Company
--Store

CREATE TABLE Enterprise (
    EnterpriseId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Address NVARCHAR(500) NULL,
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);



-- Stores table
CREATE TABLE Stores (
    StoreId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Address NVARCHAR(500) NULL,
    EnterpriseId UNIQUEIDENTIFIER NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    CONSTRAINT FK_Stores_Enterprise 
        FOREIGN KEY (EnterpriseId) REFERENCES Enterprise(EnterpriseId)
);



CREATE TABLE [ScaleCompany](
    ScaleCompanyId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de la Escala por Empresa
    EnterpriseId UNIQUEIDENTIFIER NOT NULL REFERENCES Enterprise(EnterpriseId), -- Nombre de la Empresa

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


CREATE TABLE AuditScaleTemplate (
    AuditScaleTemplateId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    TemplateData NVARCHAR(MAX) NOT NULL, -- JSON almacenado como texto
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    -- Constraint para validar que sea JSON válido
    CONSTRAINT CK_TemplateData_IsJson CHECK (ISJSON(TemplateData) = 1)
);



-- Tabla: Escala por Empresa
CREATE TABLE [Group]
(
    GroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo
    EnterpriseId UNIQUEIDENTIFIER NOT NULL, -- ID de la Empresa

    Name NVARCHAR(200) NOT NULL, -- Nombre

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
    Code NVARCHAR(10)  NOT NULL, -- Código del Grupo
    Name NVARCHAR(200) NOT NULL, -- Nombre

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


CREATE TABLE AuditTemplateFields (
    AuditTemplateFieldId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
    AuditScaleTemplateId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES AuditScaleTemplate(AuditScaleTemplateId),
    -- Información del Grupo
    GroupCode NVARCHAR(100) NOT NULL,
    GroupName NVARCHAR(255) NOT NULL,
    Orientation VARCHAR(2) NOT NULL,
    
    -- Información del Campo
    FieldCode NVARCHAR(100) NOT NULL,
    FieldName NVARCHAR(255) NOT NULL,
    FieldType NVARCHAR(50) NOT NULL, -- numeric, text, date, boolean, select, image
    --FieldSortOrder INT DEFAULT 0,
    
    -- Valores (usa el apropiado según FieldType)
    --TextValue NVARCHAR(MAX),
    --NumericValue DECIMAL(18,4),
    --DateValue DATETIME2,
    --BooleanValue BIT,
    
    -- Metadatos
    --IsRequired BIT DEFAULT 0,
    DefaultValue NVARCHAR(MAX),

    -- Auditoría    
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
    -- Índices para optimización de consultas
    INDEX IX_AuditFieldValues_FieldCode (FieldCode)
);

-- =============================================
-- CRITERIOS DE PUNTUACIÓN
-- =============================================
CREATE TABLE ScoringCriteria (
    ScoringCriteriaId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
    AuditTemplateFieldId UNIQUEIDENTIFIER NULL 
        FOREIGN KEY REFERENCES AuditTemplateFields(AuditTemplateFieldId),
    
    -- Identificación del Criterio
    CriteriaName NVARCHAR(255) NOT NULL, -- Nombre del Criterio
    CriteriaCode NVARCHAR(100), -- Código único del criterio (opcional)
    Description NVARCHAR(500), -- Descripción del criterio
    
    -- Campo a Evaluar (referencia lógica)
    EvaluatedFieldCode NVARCHAR(100) NOT NULL, -- Código del campo a evaluar
    EvaluatedFieldName NVARCHAR(255), -- Nombre del campo (desnormalizado)
    EvaluatedFieldType NVARCHAR(50), -- Tipo del campo (numeric, text, date, boolean)
    
    -- Fórmula y Evaluación
    ResultFormula NVARCHAR(500), -- Fórmula para calcular resultado del campo
    ComparisonOperator NVARCHAR(20) NOT NULL, -- Operador: '=', '!=', '>', '<', '>=', '<=', 'BETWEEN', 'IN', 'CONTAINS'
    ExpectedValue NVARCHAR(255) NOT NULL, -- Valor esperado
    
    -- Puntuación
    Score DECIMAL(10,2) NOT NULL, -- Puntaje otorgado si cumple
    ScoreWeight DECIMAL(5,2) DEFAULT 1.00, -- Peso del criterio en el grupo
    
    -- Configuración adicional
    IsRequired BIT DEFAULT 1, -- Si es obligatorio evaluar
    SortOrder INT DEFAULT 0, -- Orden de evaluación
    ErrorMessage NVARCHAR(500), -- Mensaje si no cumple
    SuccessMessage NVARCHAR(500), -- Mensaje si cumple
    
    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    -- Índices
    INDEX IX_ScoringCriteria_ScaleGroupId (ScaleGroupId),
    INDEX IX_ScoringCriteria_FieldCode (EvaluatedFieldCode),
    INDEX IX_ScoringCriteria_TemplateFieldId (AuditTemplateFieldId)
);

CREATE TABLE CriteriaSubResult (
    CriteriaSubResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
    AuditTemplateFieldId UNIQUEIDENTIFIER NULL 
        FOREIGN KEY REFERENCES AuditTemplateFields(AuditTemplateFieldId),
    
    -- Identificación del Criterio
    CriteriaName NVARCHAR(255) NOT NULL, -- Nombre del Criterio
    CriteriaCode NVARCHAR(10), -- Código único del criterio (opcional)
    -- Fórmula y Evaluación
    ResultFormula NVARCHAR(500), -- Fórmula para calcular resultado del campo
    ColorCode NVARCHAR(20) NOT NULL, -- Código de color para la evaluación

  
    -- Puntuación
    Score DECIMAL(10,2) NULL, -- Puntaje otorgado si cumple
  
   
    
    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    -- Índices
    INDEX IX_ScoringCriteria_ScaleGroupId (ScaleGroupId),
    INDEX IX_ScoringCriteria_TemplateFieldId (AuditTemplateFieldId)
);


-- =============================================
-- AUDIT PROCESS TABLES
-- =============================================

-- Table: PeriodAudit Audit (Header)
CREATE TABLE [PeriodAudit]
(
    PeriodAuditId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de Auditoría

    -- Store / audit identification
    StoreId UNIQUEIDENTIFIER REFERENCES Stores(StoreId), -- ID de Tienda

    -- Participants
    AdministratorId UNIQUEIDENTIFIER NULL, -- Administrador
    AssistantId UNIQUEIDENTIFIER NULL, -- Asistente
    OperationManagersId UNIQUEIDENTIFIER NULL, -- Gerentes de Operación
    FloatingAdministratorId UNIQUEIDENTIFIER NULL, -- Administrador Flotante
    ResponsibleAuditorId UNIQUEIDENTIFIER NULL, -- Auditor Responsable

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


-- Table: Result
CREATE TABLE PeriodAuditResult
(
    PeriodAuditResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),-- ID de Resultado de Auditoría
    PeriodAuditId UNIQUEIDENTIFIER NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAudit(PeriodAuditId),
    GroupId UNIQUEIDENTIFIER NOT NULL -- ID de Escala de Riesgo
        FOREIGN KEY REFERENCES [Group](GroupId),

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

-- Table: PeriodAuditScaleResult
CREATE TABLE PeriodAuditScaleResult
(
    PeriodAuditScaleResultId  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de Resultado de Grupo de Auditoría
    PeriodAuditResultId UNIQUEIDENTIFIER NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAuditResult(PeriodAuditResultId),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala de Riesgo
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),

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

CREATE TABLE PeriodAuditFieldValues (
    PeriodAuditFieldValueId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AuditTemplateFieldId UNIQUEIDENTIFIER NULL 
        FOREIGN KEY REFERENCES AuditTemplateFields(AuditTemplateFieldId),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
    PeriodAuditScaleResultId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES PeriodAuditScaleResult(PeriodAuditScaleResultId) ON DELETE CASCADE,
    
    -- Información del Grupo (desnormalizado para performance)
    GroupCode NVARCHAR(100) NOT NULL,
    GroupName NVARCHAR(255) NOT NULL,
    Orientation NVARCHAR(1) NULL, -- horizontal, vertical
    
    -- Información del Campo (desnormalizado)
    FieldCode NVARCHAR(100) NOT NULL,
    FieldName NVARCHAR(255) NOT NULL,
    FieldType NVARCHAR(50) NOT NULL,
    
    -- ⚠️ VALORES CAPTURADOS - Descomenta estas líneas
    TextValue NVARCHAR(MAX),
    NumericValue DECIMAL(18,4),
    DateValue DATETIME2,
    BooleanValue BIT,
    ImageUrl NVARCHAR(500), -- Para almacenar URL o path de imagen
    
    -- Metadatos del valor capturado
    IsRequired BIT DEFAULT 0,
    ValidationStatus NVARCHAR(50), -- 'valid', 'invalid', 'pending', 'warning'
    ValidationMessage NVARCHAR(500),
    
    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    -- Índices para optimización
    INDEX IX_PeriodAuditFieldValues_Composite (GroupCode, FieldCode),
    INDEX IX_PeriodAuditFieldValues_ScaleGroupId (ScaleGroupId),
    INDEX IX_PeriodAuditFieldValues_NumericValue (NumericValue) WHERE NumericValue IS NOT NULL
);