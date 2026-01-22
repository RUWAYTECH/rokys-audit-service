-- Script para agregar campo de fecha de completado de planes de acción en PeriodAudit
-- Fecha: 2026-01-21

ALTER TABLE [dbo].[PeriodAudit] ADD ActionPlanCompletedDate DATETIME2 NULL; -- Fecha en que se completaron los planes de acción
