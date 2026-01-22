-- Script para insertar registros de tipo de medida disciplinaria
-- Fecha: 2026-01-22
DECLARE @MeasureTypeId UNIQUEIDENTIFIER = NEWID();
-- Insert Escala into MaintenanceTable
DECLARE @MeasureTypeId UNIQUEIDENTIFIER = NEWID();
INSERT INTO MaintenanceTable (MaintenanceTableId, Code, Description, IsSystem, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES (@MeasureTypeId, 'TMD', 'Tipo de medida disciplinaria', 1, 1, 'Admin', GETDATE(), 'Admin', GETDATE());

-- Insert details into MaintenanceDetailTable
INSERT INTO MaintenanceDetailTable (MaintenanceDetailTableId, MaintenanceTableId, Code, Description, IsDefault, IsActive, OrderRow, JsonData, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES
    (NEWID(), @MeasureTypeId, 'AMV', 'AMONESTACION VERBAL', 0, 1, 1, '', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @MeasureTypeId, 'AME', 'AMONESTACION ESCRITA', 0, 1, 2, '', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @MeasureTypeId, 'SUS', 'SUSPENSION', 0, 1, 3, '', 'Admin', GETDATE(), 'Admin', GETDATE());