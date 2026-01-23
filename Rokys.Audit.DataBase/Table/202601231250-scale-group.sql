
-- Script para modificar tabla Group y agregar nuevos campos
-- Fecha: 2026-01-23

-- Paso 1: Eliminar FK de EnterpriseId si existe
IF OBJECT_ID('dbo.FK_Group_Enterprise', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Group] DROP CONSTRAINT FK_Group_Enterprise;

-- Paso 2: Modificar la columna EnterpriseId para permitir NULL
ALTER TABLE [dbo].[Group] ALTER COLUMN EnterpriseId UNIQUEIDENTIFIER NULL;

-- Paso 3: Recrear la clave foránea permitiendo NULL
ALTER TABLE [dbo].[Group] ADD CONSTRAINT FK_Group_Enterprise
    FOREIGN KEY (EnterpriseId) REFERENCES [dbo].[Enterprise](EnterpriseId);

-- Paso 5: Agregar columna EnterpriseGroupingId con clave foránea
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'EnterpriseGroupingId')
BEGIN
    ALTER TABLE [dbo].[Group] ADD EnterpriseGroupingId UNIQUEIDENTIFIER NULL;
    
    ALTER TABLE [dbo].[Group] ADD CONSTRAINT FK_Group_EnterpriseGrouping
        FOREIGN KEY (EnterpriseGroupingId) REFERENCES [dbo].[EnterpriseGrouping](EnterpriseGroupingId);
END

-- Paso 6: Agregar columna Code
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'Code')
    ALTER TABLE [dbo].[Group] ADD Code NVARCHAR(10) NULL;

-- Paso 7: Agregar columna NormalizedScore
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'NormalizedScore')
    ALTER TABLE [dbo].[Group] ADD NormalizedScore DECIMAL(10,2) NULL;

-- Paso 8: Agregar columna ExpectedDistribution
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'ExpectedDistribution')
    ALTER TABLE [dbo].[Group] ADD ExpectedDistribution DECIMAL(10,2) NULL;

-- Paso 9: Agregar columna LevelOrder
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'LevelOrder')
    ALTER TABLE [dbo].[Group] ADD LevelOrder INT NOT NULL DEFAULT 1;

-- Paso 10: Agregar columna ScaleType
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Group]') AND name = 'ScaleType')
    ALTER TABLE [dbo].[Group] ADD ScaleType NVARCHAR(50) NOT NULL DEFAULT 'Escala Normal' WITH VALUES;
