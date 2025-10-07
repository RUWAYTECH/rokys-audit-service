-- Insert AuditStatus values
INSERT INTO AuditStatus (AuditStatusId, Name, Code, ColorCode, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES
    (NEWID(), 'Finalizado', 'FIN', '#4CAF50', 1, 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), 'En proceso', 'PRO', '#2196F3', 1, 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), 'Cancelado', 'CAN', '#F44336', 1, 'Admin', GETDATE(), 'Admin', GETDATE());

-- Insert Escala into MaintenanceTable
DECLARE @EscalaId UNIQUEIDENTIFIER = NEWID();
INSERT INTO MaintenanceTable (MaintenanceTableId, Code, Description, IsSystem, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES (@EscalaId, 'ESC', 'Tabla de Escalas de Riesgo', 1, 1, 'Admin', GETDATE(), 'Admin', GETDATE());

-- Insert details into MaintenanceDetailTable
INSERT INTO MaintenanceDetailTable (MaintenanceDetailTableId, MaintenanceTableId, Code, Description, IsDefault, IsActive, OrderRow, JsonData, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES
    (NEWID(), @EscalaId, 'OBJ', 'En objetivo', 0, 1, 1, '{"ColorCode":"#4CAF50","Icon":"target"}', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @EscalaId, 'RB', 'Riesgo bajo', 0, 1, 2, '{"ColorCode":"#8BC34A","Icon":"trending_down"}', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @EscalaId, 'RM', 'Riesgo moderado', 0, 1, 3, '{"ColorCode":"#FFC107","Icon":"warning"}', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @EscalaId, 'RE', 'Riesgo elevado', 0, 1, 4, '{"ColorCode":"#FF9800","Icon":"arrow_upward"}', 'Admin', GETDATE(), 'Admin', GETDATE()),
    (NEWID(), @EscalaId, 'RC', 'Riesgo crítico', 0, 1, 5, '{"ColorCode":"#F44336","Icon":"error"}', 'Admin', GETDATE(), 'Admin', GETDATE());
