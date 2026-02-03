-- Script para crear la tabla PeriodAuditPreEvaluation
CREATE TABLE PeriodAuditPreEvaluation
(
    PeriodAuditPreEvaluationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- ID principal
    PeriodAuditGroupResultId UNIQUEIDENTIFIER NOT NULL -- ID de Auditoría
        FOREIGN KEY REFERENCES PeriodAuditGroupResult(PeriodAuditGroupResultId),
    TotalWeighted DECIMAL(10,2) NOT NULL, -- Total ponderado
    ScaleValueJSON NVARCHAR(MAX) NULL, -- Valores de escala en JSON
    TotalAcumulation DECIMAL(10,2) NOT NULL, -- Total acumulado
    IsActive BIT DEFAULT 1, -- Está Activo
    CreatedBy VARCHAR(120) NULL, -- Creado Por
    CreationDate DATETIME2 DEFAULT GETDATE(), -- Fecha de Creación
    UpdatedBy VARCHAR(120) NULL, -- Actualizado Por
    UpdateDate DATETIME2 NULL, -- Fecha de Actualización
    CONSTRAINT UQ_PeriodAuditPreEvaluation_PeriodAuditGroupResultId UNIQUE (PeriodAuditGroupResultId)
);
