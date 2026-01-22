
-- Tabla para definir grupos/categorías de empresas
CREATE TABLE EnterpriseGrouping
(
    EnterpriseGroupingId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID del Grupo de Empresas
    Code NVARCHAR(50) UNIQUE NOT NULL, -- Código único del grupo
    Name NVARCHAR(200) NOT NULL, -- Nombre del grupo
    Description NVARCHAR(500) NULL, -- Descripción del grupo
    
    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL -- Fecha de Actualización
);

-- Tabla de relación entre Empresas y Grupos de Empresas
CREATE TABLE EnterpriseGroup
(
    EnterpriseGroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de la Relación
    EnterpriseId UNIQUEIDENTIFIER NOT NULL -- ID de la Empresa
        FOREIGN KEY REFERENCES Enterprise(EnterpriseId),
    EnterpriseGroupingId UNIQUEIDENTIFIER NOT NULL -- ID del Grupo de Empresas
        FOREIGN KEY REFERENCES EnterpriseGrouping(EnterpriseGroupingId),
    
    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    
    -- Constraint para evitar duplicados
    CONSTRAINT UQ_EnterpriseGroup UNIQUE (EnterpriseId, EnterpriseGroupingId)
);

-- Tabla de configuración de llave-valor con referencia opcional
CREATE TABLE SystemConfiguration
(
    SystemConfigurationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID de la Configuración
    ConfigKey NVARCHAR(100) NOT NULL, -- Llave de configuración
    ConfigValue NVARCHAR(MAX) NULL, -- Valor de configuración
    DataType NVARCHAR(50) NULL, -- Tipo de dato: 'string', 'number', 'boolean', 'date', 'json', etc.
    Description NVARCHAR(500) NULL, -- Descripción de la configuración
    
    -- Campos de referencia opcional a otras tablas
    ReferenceType NVARCHAR(50) NULL, -- Tipo de referencia: 'EnterpriseGrouping', 'Enterprise', 'Store', etc.
    ReferenceCode NVARCHAR(50) NULL, -- Código de la tabla referenciada (ej: EnterpriseGrouping.Code)
    
    -- Audit fields
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    
    -- Constraint para llave única
    CONSTRAINT UQ_SystemConfiguration_Key UNIQUE (ConfigKey)
);