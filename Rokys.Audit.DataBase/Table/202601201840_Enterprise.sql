CREATE TABLE EnterpriseTheme (
    EnterpriseThemeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EnterpriseId UNIQUEIDENTIFIER NOT NULL,
    
    -- Logo
    LogoData VARBINARY(MAX) NULL,
    LogoContentType NVARCHAR(100) NULL,
    LogoFileName NVARCHAR(255) NULL,
    
    -- Colores
    PrimaryColor NVARCHAR(7) NOT NULL DEFAULT '#0066CC',
    SecondaryColor NVARCHAR(7) NOT NULL DEFAULT '#333333',
    AccentColor NVARCHAR(7) NULL,
    BackgroundColor NVARCHAR(7) NULL,
    TextColor NVARCHAR(7) NULL,
    
    IsActive BIT DEFAULT 1,
    -- Auditoría
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    
    CONSTRAINT FK_EnterpriseTheme_Enterprise 
        FOREIGN KEY (EnterpriseId) REFERENCES Enterprise(EnterpriseThemeId) ON DELETE CASCADE,
    CONSTRAINT UQ_EnterpriseTheme_EnterpriseId UNIQUE (EnterpriseId)
);