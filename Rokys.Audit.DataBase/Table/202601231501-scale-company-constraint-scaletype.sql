
-- Agregar constraint para validar ScaleType en ScaleCompany
-- Fecha: 2026-01-23

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ScaleCompany_ScaleType' AND parent_object_id = OBJECT_ID('[dbo].[ScaleCompany]'))
    ALTER TABLE [dbo].[ScaleCompany] ADD CONSTRAINT CK_ScaleCompany_ScaleType 
        CHECK (ScaleType IN ('Escala Ponderada', 'Escala Normal'));
