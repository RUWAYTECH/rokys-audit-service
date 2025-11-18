-- Script de creación de la tabla SubstitutionHistory
-- =============================================
-- Tabla para registrar la historia de suplencias de usuarios en auditorías
-- =============================================
CREATE TABLE SubstitutionHistory (
    SubstitutionHistoryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    -- Auditoría asociada
    PeriodAuditId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES PeriodAudit(PeriodAuditId),

    -- Rol en el contexto de la auditoría (informativo, no cambia)
    RoleName NVARCHAR(100) NOT NULL,

    -- Usuario anterior (si existía)
    PreviousUserReferenceId UNIQUEIDENTIFIER NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId),

    -- Nuevo usuario asignado (obligatorio)
    NewUserReferenceId UNIQUEIDENTIFIER NOT NULL
        FOREIGN KEY REFERENCES UserReference(UserReferenceId),

    -- Información adicional de la suplencia
    ChangeReason NVARCHAR(255) NULL,

    -- Auditoría del registro
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy NVARCHAR(120) NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedBy NVARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,

    -- Índices
    INDEX IX_SubstitutionHistory_PeriodAuditId (PeriodAuditId),
    INDEX IX_SubstitutionHistory_PreviousUserReferenceId (PreviousUserReferenceId),
    INDEX IX_SubstitutionHistory_NewUserReferenceId (NewUserReferenceId)
);
