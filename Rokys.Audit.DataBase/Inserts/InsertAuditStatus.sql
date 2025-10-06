INSERT INTO AuditStatus (AuditStatusId, Name, Code, ColorCode, IsActive)
VALUES
    (NEWID(), 'Finalizado', 'FIN', '#4CAF50', 1),
    (NEWID(), 'En proceso', 'PRO', '#2196F3', 1),
    (NEWID(), 'Cancelado', 'CAN', '#F44336', 1);
