
-- Script para modificar tabla ScaleCompany y agregar nuevos campos
-- Fecha: 2026-01-23

-- Paso 1: Eliminar FK de EnterpriseId si existe
IF OBJECT_ID('dbo.FK_ScaleCompany_Enterprise', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ScaleCompany] DROP CONSTRAINT FK_ScaleCompany_Enterprise;

-- Paso 2: Modificar la columna EnterpriseId para permitir NULL
ALTER TABLE [dbo].[ScaleCompany] ALTER COLUMN EnterpriseId UNIQUEIDENTIFIER NULL;

-- Paso 3: Recrear la clave foránea permitiendo NULL
ALTER TABLE [dbo].[ScaleCompany] ADD CONSTRAINT FK_ScaleCompany_Enterprise
    FOREIGN KEY (EnterpriseId) REFERENCES [dbo].[Enterprise](EnterpriseId);

-- Paso 4: Agregar columna EnterpriseGroupingId con clave foránea
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ScaleCompany]') AND name = 'EnterpriseGroupingId')
BEGIN
    ALTER TABLE [dbo].[ScaleCompany] ADD EnterpriseGroupingId UNIQUEIDENTIFIER NULL;
    
    ALTER TABLE [dbo].[ScaleCompany] ADD CONSTRAINT FK_ScaleCompany_EnterpriseGrouping
        FOREIGN KEY (EnterpriseGroupingId) REFERENCES [dbo].[EnterpriseGrouping](EnterpriseGroupingId);
END

-- Paso 5: Agregar columna NormalizedScore
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ScaleCompany]') AND name = 'NormalizedScore')
    ALTER TABLE [dbo].[ScaleCompany] ADD NormalizedScore DECIMAL(10,2) NULL;

-- Paso 6: Agregar columna ExpectedDistribution
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ScaleCompany]') AND name = 'ExpectedDistribution')
    ALTER TABLE [dbo].[ScaleCompany] ADD ExpectedDistribution DECIMAL(10,2) NULL;

-- Paso 7: Agregar columna LevelOrder
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ScaleCompany]') AND name = 'LevelOrder')
    ALTER TABLE [dbo].[ScaleCompany] ADD LevelOrder INT NOT NULL DEFAULT 1;

-- Paso 8: Agregar columna ScaleType
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ScaleCompany]') AND name = 'ScaleType')
    ALTER TABLE [dbo].[ScaleCompany] ADD ScaleType NVARCHAR(50) NOT NULL DEFAULT 'Escala Normal' WITH VALUES;
