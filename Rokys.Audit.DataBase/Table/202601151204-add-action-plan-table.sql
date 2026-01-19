CREATE TABLE PeriodAuditActionPlan
(
    PeriodAuditActionPlanId UNIQUEIDENTIFIER PRIMARY KEY,
    DisiplinaryMeasureTypeId UNIQUEIDENTIFIER NOT NULL,
    ResponsibleUserId UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    DueDate DATETIME2 NOT NULL,
    ApplyMeasure BIT NOT NULL DEFAULT 0,
    CreatedBy VARCHAR(120) NULL,
    CreationDate DATETIME2 DEFAULT GETDATE(),
    UpdatedBy VARCHAR(120) NULL,
    UpdateDate DATETIME2 NULL,
    CONSTRAINT FK_PeriodAuditActionPlans_PeriodAuditScaleResults FOREIGN KEY (PeriodAuditActionPlanId) REFERENCES PeriodAuditScaleResults(PeriodAuditScaleResultId),
    CONSTRAINT FK_PeriodAuditActionPlans_Users_ResponsibleUser FOREIGN KEY (ResponsibleUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_PeriodAuditActionPlans_DisiplinaryMeasureTypes FOREIGN KEY (DisiplinaryMeasureTypeId) REFERENCES MaintenanceDetailTable(MaintenanceDetailTableId)
);