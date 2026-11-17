-- Paso 8: Agregar columna ScaleType
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[EnterpriseGrouping]') AND name = 'ScaleType')
    ALTER TABLE [dbo].[EnterpriseGrouping] ADD ScaleType NVARCHAR(50) NOT NULL DEFAULT 'Escala Normal' WITH VALUES;