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
    RiskCritical DECIMAL(10,2) NOT NULL,

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

    Name NVARCHAR(200) NOT NULL, -- Nombre

    -- Objetivo general para el grupo
    ObjectiveValue DECIMAL(10,2) NOT NULL, -- Valor Objetivo

    -- Umbrales para el grupo
    RiskLow DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    RiskModerate DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    RiskHigh DECIMAL(10,2) NOT NULL, -- Riesgo Alto

    -- Riesgo crítico = mayor a RiesgoElevado
    RiskCritical DECIMAL(10,2) NOT NULL,
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
    RiskCritical DECIMAL(10,2) NOT NULL,

    -- Group weighting
    Weighting DECIMAL(5,2) NOT NULL, -- Ponderación

    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);



CREATE TABLE TableScaleTemplate (
    TableScaleTemplateId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Title NVARCHAR(255),
    TemplateData NVARCHAR(MAX) NULL, -- JSON almacenado como texto
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    -- Constraint para validar que sea JSON válido
    CONSTRAINT CK_TemplateData_IsJson CHECK (ISJSON(TemplateData) = 1)
);




CREATE TABLE AuditTemplateFields (
    AuditTemplateFieldId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TableScaleTemplateId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TableScaleTemplate(TableScaleTemplateId),

    -- Información del Campo
    FieldCode NVARCHAR(100) NOT NULL,
    FieldName NVARCHAR(255) NOT NULL,
    FieldType NVARCHAR(50) NOT NULL, -- numeric, text, date, boolean, select, image
    IsCalculated BIT DEFAULT 0, -- Si es un campo calculado
    CalculationFormula NVARCHAR(500), -- Fórmula para calcular el valor (si es calculado)
    AcumulationType NVARCHAR(50) NULL, -- Tipo de Acumulación: 'NA', 'SUM', 'AVERAGE', 'MAX', 'MIN', 'COUNT'
    CONSTRAINT CK_AuditTemplateFields_AcumulationType 
    CHECK (AcumulationType IN ('SUM', 'COUNT') OR AcumulationType IS NULL), -- Tipo de Acumulación: 'sum', 'average', 'max', 'min', 'count'
    FieldOptions NVARCHAR(MAX), -- Opciones para campos tipo 'select' (JSON
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
    
    -- Identificación del Criterio
    CriteriaName NVARCHAR(255) NOT NULL, -- Nombre del Criterio
    CriteriaCode NVARCHAR(10), -- Código único del criterio (opcional)
    
    
    -- Fórmula y Evaluación
    ResultFormula NVARCHAR(500), -- Fórmula para calcular resultado del campo
    ComparisonOperator NVARCHAR(20) NOT NULL, -- Operador: '=', '!=', '>', '<', '>=', '<=', 'BETWEEN', 'IN', 'CONTAINS'
    ExpectedValue NVARCHAR(255) NOT NULL, -- Valor esperado
    
    -- Puntuación
    Score DECIMAL(10,2) NOT NULL, -- Puntaje otorgado si cumple
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
);

CREATE TABLE CriteriaSubResult (
    CriteriaSubResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),
  
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
    INDEX IX_ScoringCriteria_ScaleGroupId (ScaleGroupId)
);


CREATE TABLE AuditStatus (
    AuditStatusId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    Code NVARCHAR(10) UNIQUE NOT NULL,
    ColorCode NVARCHAR(10) NULL,
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
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
    FloatingAdministratorId UNIQUEIDENTIFIER NULL, -- Administrador Suplente
    ResponsibleAuditorId UNIQUEIDENTIFIER NULL, -- Auditor Responsable

    -- Dates
    StartDate DATE NOT NULL, -- Fecha de Inicio
    EndDate DATE NOT NULL, -- Fecha de Fin
    ReportDate DATE NULL, -- Fecha de Reporte

    -- Additional information
    AuditedDays INT NULL, -- Días Auditados
    GlobalObservations NVARCHAR(MAX) NOT NULL, -- Observaciones Globales
    TotalWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Total

    StatusId UNIQUEIDENTIFIER NOT NULL -- ID de Estado
        FOREIGN KEY REFERENCES AuditStatus(AuditStatusId),

    
    -- Objetivo general para la empresa
    ObjectiveValue DECIMAL(10,2) NOT NULL, -- Valor Objetivo

    -- Umbrales para la empresa
    RiskLow DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    RiskModerate DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    RiskHigh DECIMAL(10,2) NOT NULL, -- Riesgo Alto

    -- Riesgo crítico = mayor a RiesgoElevado
    RiskCritical DECIMAL(10,2) NOT NULL,


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

    -- Historical weighting and thresholds
    AppliedRiskLow DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    AppliedRiskModerate DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    AppliedRiskHigh DECIMAL(10,2) NOT NULL, -- Riesgo Alto
    AppliedRiskCritical DECIMAL(10,2) NOT NULL, -- Ponderación del grupo
    AppliedWeighting DECIMAL(5,2) NOT NULL, -- Peso del Grupo

    Observations NVARCHAR(150) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);


CREATE TABLE EvidenceFiles (
    EvidenceFileId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditResultId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES PeriodAuditResult(PeriodAuditResultId) ON DELETE CASCADE,
    OriginalName NVARCHAR(255) NOT NULL, -- Nombre original del archivo
    FileName NVARCHAR(255) NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL, -- URL o path del archivo
    FileType NVARCHAR(50) NULL, -- Tipo de archivo (opcional)
    UploadedBy VARCHAR(120) NULL,
    UploadDate DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
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


    -- Historical weighting and thresholds
    AppliedLowRisk DECIMAL(10,2) NOT NULL, -- Riesgo Bajo
    AppliedModerateRisk DECIMAL(10,2) NOT NULL, -- Riesgo Moderado
    AppliedHighRisk DECIMAL(10,2) NOT NULL, -- Riesgo Alto
    AppliedRiskCritical DECIMAL(10,2) NOT NULL,
    AppliedWeighting DECIMAL(5,2) NOT NULL, -- Ponderación


    Observations NVARCHAR(MAX) NULL, -- Observaciones

    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

CREATE TABLE PeriodAuditTableScaleTemplateResult (
    PeriodAuditTableScaleTemplateResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditScaleResultId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES PeriodAuditScaleResult(PeriodAuditScaleResultId) ON DELETE CASCADE,
    TableScaleTemplateId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES TableScaleTemplate(TableScaleTemplateId),
    TemplateData NVARCHAR(MAX) NULL, -- JSON almacenado como texto
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    -- Constraint para validar que sea JSON válido
    CONSTRAINT CK_PeriodAuditTableScaleTemplate_TemplateData_IsJson CHECK (ISJSON(TemplateData) = 1)
);

CREATE TABLE PeriodAuditFieldValues (
    PeriodAuditFieldValueId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AuditTemplateFieldId UNIQUEIDENTIFIER NULL 
        FOREIGN KEY REFERENCES AuditTemplateFields(AuditTemplateFieldId),

    PeriodAuditTableScaleTemplateResultId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES PeriodAuditTableScaleTemplateResult(PeriodAuditTableScaleTemplateResultId) ON DELETE CASCADE,

    -- Información del Campo (desnormalizado)
    FieldCode NVARCHAR(100) NOT NULL,
    FieldName NVARCHAR(255) NOT NULL,
    FieldType NVARCHAR(50) NOT NULL, -- numeric, text, date, boolean, select, image
    IsCalculated BIT DEFAULT 0, -- Si es un campo calculado
    CalculationFormula NVARCHAR(500), -- Fórmula para calcular el valor (si es calculado)
    AcumulationType NVARCHAR(50) NULL, -- Tipo de Acumulación: 'NA', 'SUM', 'AVERAGE', 'MAX', 'MIN', 'COUNT'
    CONSTRAINT CK_AuditTemplateFields_AcumulationTypeResult 
    CHECK (AcumulationType IN ('SUM', 'COUNT') OR AcumulationType IS NULL), -- Tipo de Acumulación: 'sum', 'average', 'max', 'min', 'count'
    FieldOptions NVARCHAR(MAX), -- Opciones para campos tipo 'select' (JSON


    
    -- ⚠️ VALORES CAPTURADOS - Descomenta estas líneas
    TextValue NVARCHAR(MAX),
    NumericValue DECIMAL(18,4),
    DateValue DATETIME2,
    BooleanValue BIT,
    ImageUrl NVARCHAR(500), -- Para almacenar URL o path de imagen
    FieldOptionsValue NVARCHAR(255), -- Valor seleccionado para campos tipo 'select'
    

    ValidationStatus NVARCHAR(50), -- 'valid', 'invalid', 'pending', 'warning'
    ValidationMessage NVARCHAR(500),
    
    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    -- Índices para optimización
    INDEX IX_PeriodAuditFieldValues_FieldCode (FieldCode),
    INDEX IX_PeriodAuditFieldValues_NumericValue (NumericValue) WHERE NumericValue IS NOT NULL
);


CREATE TABLE PeriodAuditScoringCriteriaResult (
    PeriodAuditScoringCriteriaResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditScaleResultId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES PeriodAuditScaleResult(PeriodAuditScaleResultId) ON DELETE CASCADE,

   -- Identificación del Criterio
    CriteriaName NVARCHAR(255) NOT NULL, -- Nombre del Criterio
    CriteriaCode NVARCHAR(10), -- Código único del criterio (opcional)
    
    
    -- Fórmula y Evaluación
    ResultFormula NVARCHAR(500), -- Fórmula para calcular resultado del campo
    ComparisonOperator NVARCHAR(20) NOT NULL, -- Operador: '=', '!=', '>', '<', '>=', '<=', 'BETWEEN', 'IN', 'CONTAINS'
    ExpectedValue NVARCHAR(255) NOT NULL, -- Valor esperado
    
    -- Puntuación
    Score DECIMAL(10,2) NOT NULL, -- Puntaje otorgado si cumple
    SortOrder INT DEFAULT 0, -- Orden de evaluación

   
    ErrorMessage NVARCHAR(500), -- Mensaje si no cumple
    SuccessMessage NVARCHAR(500), -- Mensaje si cumple

    ResultObtained DECIMAL(10,2) NULL, -- Puntaje obtenido

    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);
-- =============================================
-- RESULTADOS DE EVALUACIÓN DE SUB-CRITERIOS
-- =============================================
CREATE TABLE PeriodAuditScaleSubResult (
    PeriodAuditScaleSubResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditScaleResultId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES PeriodAuditScaleResult(PeriodAuditScaleResultId) ON DELETE CASCADE,
    CriteriaSubResultId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES CriteriaSubResult(CriteriaSubResultId),
    
    -- Identificación del Criterio (desnormalizado)
    CriteriaCode NVARCHAR(10),
    CriteriaName NVARCHAR(255) NOT NULL,
    
    -- Valores evaluados
    EvaluatedValue NVARCHAR(255), -- Valor que se evaluó
    CalculatedResult NVARCHAR(255), -- Resultado de aplicar la fórmula
    AppliedFormula NVARCHAR(500), -- Fórmula que se aplicó (histórico)
    
    -- Resultado de la evaluación
    ScoreObtained DECIMAL(10,2) NULL, -- Puntaje obtenido
    ColorCode NVARCHAR(20), -- Código de color del resultado
    

    
    -- Auditoría
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    -- Índices
    INDEX IX_PeriodAuditScaleSubResult_ScaleResultId (PeriodAuditScaleResultId),
    INDEX IX_PeriodAuditScaleSubResult_CriteriaId (CriteriaSubResultId),
    UNIQUE (PeriodAuditScaleResultId, CriteriaSubResultId) -- Un sub-resultado por criterio por resultado de escala
);
