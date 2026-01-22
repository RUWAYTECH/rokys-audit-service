-- =============================================
-- INSERTS PARA AuditRoleConfiguration
-- Configuración de roles de auditoría
-- =============================================

-- Insertar configuraciones de roles de auditoría con orden secuencial
INSERT INTO AuditRoleConfiguration (
    AuditRoleConfigurationId,
    RoleCode,
    RoleName,
    IsRequired,
    AllowMultiple,
    SequenceOrder,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
) VALUES
-- A001 - Jefe de Area (Orden 1)
(NEWID(), 'A001', 'Jefe de Area', 1, 0, 1, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME()),

-- A002 - Asistente (Orden 2) 
(NEWID(), 'A002', 'Asistente', 1, 0, 2, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME()),

-- A003 - Jefe de operaciones (Orden 3)
(NEWID(), 'A003', 'Jefe de operaciones', 1, 0, 3, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME()),

-- A004 - Volante (Orden 4)
(NEWID(), 'A004', 'Volante', 1, 0, 4, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME()),

-- A005 - Auditor (Orden 5)
(NEWID(), 'A005', 'Auditor', 1, 0, 5, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME()),

-- A006 - JOB/Supervisor (Orden 6)
(NEWID(), 'A006', 'JOB/Supervisor', 1, 0, 6, 1, 'system.admin', SYSUTCDATETIME(), 'system.admin', SYSUTCDATETIME());

-- =============================================
-- VERIFICACIÓN DE LOS DATOS INSERTADOS
-- =============================================

-- Consulta para verificar los datos insertados
SELECT 
    RoleCode,
    RoleName,
    IsRequired,
    AllowMultiple,
    SequenceOrder,
    IsActive,
    CreatedBy,
    CreationDate
FROM AuditRoleConfiguration
ORDER BY SequenceOrder;

-- =============================================
-- ESTADÍSTICAS
-- =============================================
PRINT 'Total de configuraciones de roles insertadas: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
