
-- Agregar constraint para validar ScaleType en Group
-- Fecha: 2026-01-23

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Group_ScaleType' AND parent_object_id = OBJECT_ID('[dbo].[Group]'))
    ALTER TABLE [dbo].[Group] ADD CONSTRAINT CK_Group_ScaleType 
        CHECK (ScaleType IN ('Escala Ponderada', 'Escala Normal'));
