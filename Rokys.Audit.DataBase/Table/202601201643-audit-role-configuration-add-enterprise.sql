ALTER TABLE AuditRoleConfiguration ADD EnterpriseId [uniqueidentifier];

GO

ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT FK_AuditRoleConfiguration_Enterprise FOREIGN KEY (EnterpriseId) REFERENCES Enterprise (EnterpriseId);

GO

ALTER TABLE AuditRoleConfiguration DROP CONSTRAINT UQ_AuditRoleConfiguration_RoleCode;

GO

ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT UQ_AuditRoleConfiguration_RoleCode_Enterprise UNIQUE (EnterpriseId, RoleCode);
