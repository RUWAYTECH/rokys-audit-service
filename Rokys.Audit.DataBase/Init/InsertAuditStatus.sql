-- Insert AuditStatus values
INSERT INTO AuditStatus (AuditStatusId, Name, Code, ColorCode, IsActive)
VALUES
    (NEWID(), 'Finalizado', 'FIN', '#4CAF50', 1),
    (NEWID(), 'En proceso', 'PRO', '#2196F3', 1),
    (NEWID(), 'Cancelado', 'CAN', '#F44336', 1);

-- Insert Escala into MaintenanceTable
DECLARE @EscalaId UNIQUEIDENTIFIER = NEWID();
INSERT INTO MaintenanceTable (MaintenanceTableId, Code, Description, IsSystem, IsActive)
VALUES (@EscalaId, 'ESCALA', 'Tabla de Escalas de Riesgo', 1, 1);

-- Insert details into MaintenanceDetailTable
INSERT INTO MaintenanceDetailTable (MaintenanceDetailTableId, MaintenanceTableId, Code, Description, IsDefault, IsActive, OrderRow, JsonData)
VALUES
    (NEWID(), @EscalaId, 'OBJ', 'En objetivo', 0, 1, 1, '{"ColorCode":"#4CAF50","Icon":"target"}'),
    (NEWID(), @EscalaId, 'RB', 'Riesgo bajo', 0, 1, 2, '{"ColorCode":"#8BC34A","Icon":"trending_down"}'),
    (NEWID(), @EscalaId, 'RM', 'Riesgo moderado', 0, 1, 3, '{"ColorCode":"#FFC107","Icon":"warning"}'),
    (NEWID(), @EscalaId, 'RE', 'Riesgo elevado', 0, 1, 4, '{"ColorCode":"#FF9800","Icon":"arrow_upward"}'),
    (NEWID(), @EscalaId, 'RC', 'Riesgo crítico', 0, 1, 5, '{"ColorCode":"#F44336","Icon":"error"}');
