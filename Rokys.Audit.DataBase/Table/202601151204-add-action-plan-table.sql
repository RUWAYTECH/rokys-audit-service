CREATE TABLE PeriodAuditActionPlan
(
    PeriodAuditActionPlanId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodAuditScaleResultId UNIQUEIDENTIFIER NOT NULL,
    DisiplinaryMeasureTypeId UNIQUEIDENTIFIER NULL,
    ResponsibleUserId UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    DueDate DATETIME2 NOT NULL,
    ApplyMeasure BIT NOT NULL DEFAULT 0,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    CONSTRAINT FK_PeriodAuditActionPlans_PeriodAuditScaleResults FOREIGN KEY (PeriodAuditScaleResultId) REFERENCES PeriodAuditScaleResult(PeriodAuditScaleResultId),
    CONSTRAINT FK_PeriodAuditActionPlans_Users_ResponsibleUser FOREIGN KEY (ResponsibleUserId) REFERENCES UserReference(UserReferenceId),
    CONSTRAINT FK_PeriodAuditActionPlans_DisiplinaryMeasureTypes FOREIGN KEY (DisiplinaryMeasureTypeId) REFERENCES MaintenanceDetailTable(MaintenanceDetailTableId)
);