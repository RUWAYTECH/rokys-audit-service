--liquibase formatted sql

--changeset system:202601201643-drop-old-unique-index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_AuditRoleConfiguration_RoleCode' AND object_id = OBJECT_ID('AuditRoleConfiguration'))
BEGIN
    DROP INDEX UQ_AuditRoleConfiguration_RoleCode ON AuditRoleConfiguration;
END

--changeset system:202601201643-add-new-unique-constraint
ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT UQ_AuditRoleConfiguration_RoleCode_Enterprise UNIQUE (EnterpriseId, RoleCode);

--changeset system:202601201643-add-unique-index-no-enterprise
CREATE UNIQUE INDEX UQ_AuditRoleConfiguration_RoleCode_NoEnterprise 
ON AuditRoleConfiguration (RoleCode) 
WHERE EnterpriseId IS NULL;
