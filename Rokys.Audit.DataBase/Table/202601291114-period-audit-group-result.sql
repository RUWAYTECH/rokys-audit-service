IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[PeriodAuditGroupResult]') AND name = 'Code')
    ALTER TABLE [dbo].[PeriodAuditGroupResult] ADD Code NVARCHAR(10) NULL;