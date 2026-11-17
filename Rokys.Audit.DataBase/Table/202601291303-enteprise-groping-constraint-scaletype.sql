
-- Agregar constraint para validar ScaleType en ScaleCompany
-- Fecha: 2026-01-23

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_EnterpriseGrouping_ScaleType' AND parent_object_id = OBJECT_ID('[dbo].[EnterpriseGrouping]'))
    ALTER TABLE [dbo].[EnterpriseGrouping] ADD CONSTRAINT CK_EnterpriseGrouping_ScaleType 
        CHECK (ScaleType IN ('Escala Ponderada', 'Escala Normal'));
