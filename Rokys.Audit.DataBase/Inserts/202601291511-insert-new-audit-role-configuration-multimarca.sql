DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER;

SELECT @EnterpriseGroupingId = EnterpriseGroupingId
FROM dbo.EnterpriseGrouping
WHERE Code = 'EG002';

INSERT INTO dbo.AuditRoleConfiguration (AuditRoleConfigurationId,RoleCode,RoleName,IsRequired,AllowMultiple,SequenceOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseId,EnterpriseGroupingId) VALUES
(NEWID(),'A010','Jefe de Area de Multimarca',0,1,18,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A009','Gerente Adjunto',1,0,10,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A008','Gerente de RRHH',1,0,12,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A007','Gerente de Unidad',1,0,13,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A006','Supervisor',0,1,14,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A005','Auditor',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A003','Gerente de Operaciones',1,0,16,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId),
(NEWID(),'A001','Jefe de Area',1,1,17,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingId);