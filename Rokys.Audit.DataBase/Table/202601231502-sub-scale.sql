
-- Script para crear tabla SubScale
-- Fecha: 2026-01-23

-- Crear tabla SubScale
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SubScale' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[SubScale] (
        SubScaleId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        EnterpriseGroupingId UNIQUEIDENTIFIER NOT NULL,
        Code NVARCHAR(10) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Value NVARCHAR(10) NOT NULL,
        ColorCode NVARCHAR(10) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1, -- Está Activo
        CreatedBy NVARCHAR(120) NOT NULL,
        CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedBy NVARCHAR(120) NULL,
        UpdateDate DATETIME NULL,
        CONSTRAINT FK_SubScale_EnterpriseGrouping FOREIGN KEY (EnterpriseGroupingId) 
            REFERENCES [dbo].[EnterpriseGrouping](EnterpriseGroupingId)
    );
END
