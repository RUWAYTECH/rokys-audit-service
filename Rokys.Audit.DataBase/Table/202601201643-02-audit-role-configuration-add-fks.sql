--liquibase formatted sql

--changeset system:202601201643-add-fk-enterprise
ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT FK_AuditRoleConfiguration_Enterprise FOREIGN KEY (EnterpriseId) REFERENCES Enterprise (EnterpriseId);

--changeset system:202601201643-add-fk-enterprise-grouping
ALTER TABLE AuditRoleConfiguration ADD CONSTRAINT FK_AuditRoleConfiguration_EnterpriseGrouping FOREIGN KEY (EnterpriseGroupingId) REFERENCES EnterpriseGrouping (EnterpriseGroupingId);
