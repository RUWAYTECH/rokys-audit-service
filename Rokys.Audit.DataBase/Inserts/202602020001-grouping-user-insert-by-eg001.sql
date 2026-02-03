-- Script para insertar usuarios en GroupingUser para el EnterpriseGrouping EG001
-- Fecha: 2026-02-02

DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER;
SELECT @EnterpriseGroupingId = EnterpriseGroupingId
FROM EnterpriseGrouping
WHERE Code = 'EG001';

INSERT INTO dbo.GroupingUser (
    GroupingUserId,
    EnterpriseGroupingId,
    UserReferenceId,
    RolesCodes,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
)
SELECT
    NEWID() AS GroupingUserId,
    @EnterpriseGroupingId AS EnterpriseGroupingId,
    ur.UserReferenceId,
    ur.RoleCode AS RolesCodes,
    1 AS IsActive,
    ur.CreatedBy,
    ur.CreationDate,
    ur.UpdatedBy,
    ur.UpdateDate
FROM UserReference ur
WHERE NOT EXISTS (
    SELECT 1 FROM GroupingUser gu
    WHERE gu.UserReferenceId = ur.UserReferenceId
      AND gu.EnterpriseGroupingId = @EnterpriseGroupingId
) and RoleCode is  not null;
