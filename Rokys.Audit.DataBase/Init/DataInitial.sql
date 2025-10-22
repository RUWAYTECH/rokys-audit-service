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
    Code NVARCHAR(10)  NOT NULL, -- Código de la Escala
    Name NVARCHAR(200) NOT NULL, -- Nombre

    MinValue DECIMAL(10,2) NOT NULL, -- Valor Mínimo
    MaxValue DECIMAL(10,2) NOT NULL, -- Valor Máximo
    ColorCode NVARCHAR(20) NULL, -- Código de Color (Hexadecimal o nombre)
    Icon NVARCHAR(100) NULL, -- Icono asociado a la escala
    SortOrder INT DEFAULT 0, -- Orden de la Escala

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
    EnterpriseId UNIQUEIDENTIFIER NOT NULL -- ID de la Empresa
        FOREIGN KEY REFERENCES Enterprise(EnterpriseId),
    Name NVARCHAR(200) NOT NULL, -- Nombre

    -- Ponderación del grupo
    Weighting DECIMAL(5,2) NOT NULL, -- Peso del Grupo

    -- Auditoría
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- =============================================
-- INBOX: Bandeja de auditorías
-- =============================================
CREATE TABLE InboxItems (
    InboxItemId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES PeriodAudit(PeriodAuditId) ON DELETE CASCADE,
    PrevStatusId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES AuditStatus(AuditStatusId),
    NextStatusId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES AuditStatus(AuditStatusId),
    PrevUserId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId),
    NextUserId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId),
    ApproverId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId),
    Comments NVARCHAR(MAX) NULL,
    UserId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- quien registró la acción
    Action NVARCHAR(100) NULL, -- acción realizada: 'Aprobada','Cancelada','Devuelta', etc.
    SequenceNumber INT NOT NULL DEFAULT 0, -- número secuencial por PeriodAudit para identificar el último creado

    -- Auditoría común
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL
);

CREATE INDEX IX_InboxItems_PeriodAuditId ON InboxItems(PeriodAuditId);
CREATE INDEX IX_InboxItems_NextStatusId ON InboxItems(NextStatusId);

CREATE TABLE ScaleGroup
(
    ScaleGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo de Escala
    GroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo
        FOREIGN KEY REFERENCES [Group](GroupId),
    Code NVARCHAR(10)  NOT NULL, -- Código del Grupo
    Name NVARCHAR(200) NOT NULL, -- Nombre
    
    HasSourceData BIT DEFAULT 0, -- Tiene Datos Fuente

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
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Orientation NVARCHAR(2) DEFAULT 'V', -- 'vertical' o 'horizontal'
    SortOrder INT DEFAULT 0,
    CONSTRAINT CK_AuditTemplateFields_Orientation
    CHECK (Orientation IN ('H', 'V') OR Orientation IS NULL), --
    TemplateData NVARCHAR(MAX) NULL, -- JSON almacenado como texto
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    -- Constraint para validar que sea JSON válido
    CONSTRAINT CK_TemplateData_IsJson 
        CHECK (TemplateData IS NULL OR ISJSON(TemplateData) = 1)
);




CREATE TABLE AuditTemplateFields (
    AuditTemplateFieldId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TableScaleTemplateId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TableScaleTemplate(TableScaleTemplateId),

    -- Información del Campo
    FieldCode NVARCHAR(100) NOT NULL,
    FieldName NVARCHAR(255) NOT NULL,
    FieldType NVARCHAR(50) NOT NULL, -- numeric, text, date, boolean, select, image
    IsCalculated VARCHAR(50), -- Si es un campo calculado
    CalculationFormula NVARCHAR(500), -- Fórmula para calcular el valor (si es calculado)
    AcumulationType NVARCHAR(50) NULL, -- Tipo de Acumulación: 'NA', 'SUM', 'AVERAGE', 'MAX', 'MIN', 'COUNT'
    CONSTRAINT CK_AuditTemplateFields_AcumulationType 
    CHECK (AcumulationType IN ('SUM', 'COUNT') OR AcumulationType IS NULL), -- Tipo de Acumulación: 'sum', 'average', 'max', 'min', 'count'
    FieldOptions NVARCHAR(MAX), -- Opciones para campos tipo 'select' (JSON
    CONSTRAINT CK_FieldOptions_IsJson 
        CHECK (FieldOptions IS NULL OR ISJSON(FieldOptions) = 1),
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

CREATE TABLE MaintenanceTable (
    MaintenanceTableId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsSystem BIT NOT NULL DEFAULT(0),
    IsActive BIT NOT NULL DEFAULT(1),
    CreationDate DATETIME NOT NULL DEFAULT(GETDATE()),
    CreatedBy NVARCHAR(100) NOT NULL,
    UpdateDate DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL
);


CREATE TABLE MaintenanceDetailTable (
    MaintenanceDetailTableId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaintenanceTableId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES MaintenanceTable(MaintenanceTableId),
    Code NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    JsonData NVARCHAR(MAX) NULL,
    OrderRow INT NULL,
    IsDefault BIT NOT NULL DEFAULT(0),
    IsActive BIT NOT NULL DEFAULT(1),
    CreationDate DATETIME NOT NULL DEFAULT(GETDATE()),
    CreatedBy NVARCHAR(100) NOT NULL,
    UpdateDate DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL
);

-- =============================================
-- CRITERIOS DE PUNTUACIÓN
-- =============================================
CREATE TABLE ScoringCriteria (
    ScoringCriteriaId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL 
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),

    CriteriaCode NVARCHAR(10) NOT NULL, -- AUTO GENERATE PREFIX SR-{4 DIGITS}
    -- Identificación del Criterio
    CriteriaName NVARCHAR(255) NOT NULL, -- Nombre del Criterio
    
    
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
    CriteriaCode NVARCHAR(10), -- AUTO GENERATE PREFIX CSR-{4 DIGITS}
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


CREATE TABLE UserReference (
    UserReferenceId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL, -- Security MS
    EmployeeId UNIQUEIDENTIFIER NULL,-- Memos MS
    FirstName NVARCHAR(200) NOT NULL,
    LastName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150),
    PersonalEmail NVARCHAR(150),
    DocumentNumber NVARCHAR(20),
    RoleCode NVARCHAR(50),
    RoleName NVARCHAR(100),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy NVARCHAR(100) NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedBy NVARCHAR(100) NULL,
    UpdateDate DATETIME2 NULL
);


CREATE TABLE EmployeeStores (
    EmployeeStoreId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserReferenceId UNIQUEIDENTIFIER NOT NULL,
    StoreId UNIQUEIDENTIFIER NOT NULL,
    AssignmentDate DATE NOT NULL DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    CONSTRAINT FK_EmployeeStores_UserReference 
        FOREIGN KEY (UserReferenceId) REFERENCES UserReference(UserReferenceId),
    CONSTRAINT FK_EmployeeStores_Store 
        FOREIGN KEY (StoreId) REFERENCES Stores(StoreId),
    CONSTRAINT UQ_EmployeeStore UNIQUE (UserReferenceId, StoreId)
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

    -- Participants with FK to UserReference
    AdministratorId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_Administrator 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Administrador
    AssistantId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_Assistant 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Asistente
    OperationManagersId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_OperationManagers 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Gerentes de Operación
    FloatingAdministratorId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_FloatingAdministrator 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Administrador Suplente
    ResponsibleAuditorId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_ResponsibleAuditor 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Auditor Responsable
    SupervisorId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_PeriodAudit_Supervisor 
        FOREIGN KEY REFERENCES UserReference(UserReferenceId), -- Supervisor


    -- Dates
    StartDate DATE NOT NULL, -- Fecha de Inicio
    EndDate DATE NOT NULL, -- Fecha de Fin
    ReportDate DATE NULL, -- Fecha de Reporte

    -- Additional information
    AuditedDays INT NULL, -- Días Auditados
    GlobalObservations NVARCHAR(MAX) NOT NULL, -- Observaciones Globales
    TotalWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Total

    StatusId UNIQUEIDENTIFIER NULL -- ID de Estado
        FOREIGN KEY REFERENCES AuditStatus(AuditStatusId),

    CorrelativeNumber VARCHAR(50) NULL, -- Número Correlativo de la Auditoría

    -- Puntuación
    ScoreValue DECIMAL(10,2) NOT NULL,
    ScaleCode NVARCHAR(10) NOT NULL, -- Código de la Escala
    ScaleName NVARCHAR(100) NOT NULL, -- Nombre de la Escala
    ScaleIcon NVARCHAR(20) NOT NULL, -- Icon de la Escala
    ScaleColor NVARCHAR(10) NOT NULL, -- Valor de la Escala
    ScaleMinValue DECIMAL(10,2) NOT NULL, -- Valor Mínimo de la Escala
    ScaleMaxValue DECIMAL(10,2) NOT NULL, -- Valor Máximo de la Escala



    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME NULL -- Fecha de Actualización
);





-- Table: Result
CREATE TABLE PeriodAuditGroupResult
(
    PeriodAuditGroupResultId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),-- ID de Resultado de Grupo de Auditoría
    PeriodAuditId UNIQUEIDENTIFIER NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAudit(PeriodAuditId),
    GroupId UNIQUEIDENTIFIER NOT NULL -- ID de Escala de Riesgo
        FOREIGN KEY REFERENCES [Group](GroupId),

    ScoreValue DECIMAL(10,2) NOT NULL, -- Valor Obtenido

    Observations NVARCHAR(150) NULL, -- Observaciones
    ScaleDescription NVARCHAR(150) NULL, -- Descripción de la Escala
    TotalWeighting DECIMAL(5,2) NOT NULL, -- Ponderación Total

    ScaleColor NVARCHAR(20) NULL, -- Código de Color del Grupo
    HasEvidence BIT DEFAULT 0,
  
    -- Record audit
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);


CREATE TABLE StorageFiles (
    StorageFileId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EntityId UNIQUEIDENTIFIER NULL,
    EntityName NVARCHAR(255) NULL, 
    ClassificationType NVARCHAR(100) NULL, -- Tipo de Clasificación (opcional)
        
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
    PeriodAuditGroupResultId UNIQUEIDENTIFIER NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAuditGroupResult(PeriodAuditGroupResultId),
    ScaleGroupId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Escala de Riesgo
        FOREIGN KEY REFERENCES ScaleGroup(ScaleGroupId),

    -- Calculation data at the time of audit
    ScoreValue DECIMAL(10,2) NOT NULL, -- Valor Total

    AppliedWeighting DECIMAL(5,2) NOT NULL, -- Ponderación


    Observations NVARCHAR(MAX) NULL, -- Observaciones

    ScaleDescription NVARCHAR(150) NULL, -- Descripción de la Escala
    ScaleColor NVARCHAR(20) NULL, -- Código de Color de la Escala

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
    TableScaleTemplateId UNIQUEIDENTIFIER NULL,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Orientation NVARCHAR(2) DEFAULT 'V', -- 'vertical' o 'horizontal'
    CONSTRAINT CK_PeriodAuditTableScaleTemplateResult_Orientation
    CHECK (Orientation IN ('H', 'V') OR Orientation IS NULL),
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
    AuditTemplateFieldId UNIQUEIDENTIFIER NULL,

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
    FieldOptions NVARCHAR(MAX), -- Opciones para campos tipo 'select' (JSON)
    TableDataHorizontal NVARCHAR(MAX), -- Datos de la tabla si es plantilla horizontal (JSON)

    
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


-- Índice compuesto principal para ScaleCode con columnas de filtrado frecuente
CREATE NONCLUSTERED INDEX IX_PeriodAudit_ScaleCode_Optimized 
ON PeriodAudit (ScaleCode, IsActive, StoreId, CreationDate)
INCLUDE (ScaleName, ScaleColor, ScaleIcon, ScoreValue, ScaleMinValue, ScaleMaxValue, StatusId)
WITH (
    PAD_INDEX = OFF,
    STATISTICS_NORECOMPUTE = OFF,
    SORT_IN_TEMPDB = OFF,
    DROP_EXISTING = OFF,
    ONLINE = OFF,
    ALLOW_ROW_LOCKS = ON,
    ALLOW_PAGE_LOCKS = ON,
    FILLFACTOR = 90
);

-- Índice específico para consultas de reportes por rangos de fecha
CREATE NONCLUSTERED INDEX IX_PeriodAudit_ScaleCode_DateRange
ON PeriodAudit (ScaleCode, CreationDate, IsActive)
INCLUDE (StoreId, ScoreValue, StatusId)
WHERE IsActive = 1
WITH (FILLFACTOR = 95);

-- Índice para consultas de estadísticas por escala
CREATE NONCLUSTERED INDEX IX_PeriodAudit_ScaleCode_Statistics
ON PeriodAudit (ScaleCode, StatusId, IsActive)
INCLUDE (ScoreValue, StoreId, CreationDate)
WITH (FILLFACTOR = 90);