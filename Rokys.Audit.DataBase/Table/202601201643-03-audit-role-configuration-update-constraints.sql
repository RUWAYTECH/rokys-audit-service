--liquibase formatted sql

--changeset system:202601201643-drop-old-unique-index
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_AuditRoleConfiguration_RoleCode' AND parent_object_id = OBJECT_ID('AuditRoleConfiguration') AND type = 'UQ')
BEGIN
    ALTER TABLE AuditRoleConfiguration DROP CONSTRAINT UQ_AuditRoleConfiguration_RoleCode;
END

--changeset system:202601201643-add-new-unique-constraint
ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT UQ_AuditRoleConfiguration_RoleCode_Enterprise UNIQUE (EnterpriseGroupingId, EnterpriseId, RoleCode);

--changeset system:202601201643-add-unique-index-no-enterprise
CREATE UNIQUE INDEX UQ_AuditRoleConfiguration_RoleCode_NoEnterprise 
ON AuditRoleConfiguration (EnterpriseGroupingId, RoleCode) 
WHERE EnterpriseId IS NULL;
