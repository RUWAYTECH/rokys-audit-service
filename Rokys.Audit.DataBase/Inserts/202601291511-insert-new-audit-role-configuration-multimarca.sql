DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER;

SELECT @EnterpriseGroupingId = EnterpriseGroupingId
FROM dbo.EnterpriseGrouping
WHERE Code = 'EG002';

INSERT INTO dbo.AuditRoleConfiguration (AuditRoleConfigurationId,RoleCode,RoleName,IsRequired,AllowMultiple,SequenceOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseId,EnterpriseGroupingId) VALUES
(NEWID(),'M001','Gerente Adjunto',1,0,10,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M002','Gerente de RRHH',1,0,12,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M003','Gerente de Unidad',1,0,13,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M004','Supervisor',0,1,14,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M005','Auditor',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M006','Gerente de Operaciones',1,0,16,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M007','Jefe de Area',0,1,17,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'M008','Jefe de Area de Multimarca',0,1,18,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId);
