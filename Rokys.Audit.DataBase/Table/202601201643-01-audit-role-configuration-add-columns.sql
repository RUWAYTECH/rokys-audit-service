--liquibase formatted sql

--changeset system:202601201643-add-enterprise-id
ALTER TABLE AuditRoleConfiguration ADD EnterpriseId [uniqueidentifier] NULL;

--changeset system:202601201643-add-enterprise-grouping-id
ALTER TABLE AuditRoleConfiguration ADD EnterpriseGroupingId [uniqueidentifier];
