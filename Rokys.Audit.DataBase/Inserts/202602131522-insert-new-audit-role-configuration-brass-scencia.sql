DECLARE @EnterpriseGroupingBrassId UNIQUEIDENTIFIER;

SELECT @EnterpriseGroupingBrassId = EnterpriseGroupingId
FROM dbo.EnterpriseGrouping
WHERE Code = 'EG003';

INSERT INTO dbo.AuditRoleConfiguration (AuditRoleConfigurationId,RoleCode,RoleName,IsRequired,AllowMultiple,SequenceOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseId,EnterpriseGroupingId) VALUES
(NEWID(),'A009','Gerente Adjunto',1,0,10,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A008','Gerente de RRHH',1,0,12,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A007','Gerente de Unidad',1,0,13,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A005','Auditor',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A004','Asistente Administrativo',1,0,16,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A003','Gerente de Operaciones',1,0,16,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A002','Administrador de tienda',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId),
(NEWID(),'A001','Jefe de Area',1,1,17,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingBrassId);


DECLARE @EnterpriseGroupingScenciaId UNIQUEIDENTIFIER;

SELECT @EnterpriseGroupingScenciaId = EnterpriseGroupingId
FROM dbo.EnterpriseGrouping
WHERE Code = 'EG004';

INSERT INTO dbo.AuditRoleConfiguration (AuditRoleConfigurationId,RoleCode,RoleName,IsRequired,AllowMultiple,SequenceOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseId,EnterpriseGroupingId) VALUES
(NEWID(),'A008','Gerente de RRHH',1,0,12,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A007','Gerente de Unidad',1,0,13,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A006','Supervisor',0,1,14,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A005','Auditor',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A003','Gerente de Operaciones',1,0,16,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A002','Administrador de tienda',1,1,15,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId),
(NEWID(),'A001','Jefe de Area',1,1,17,1,'SYSTEM',GETDATE(),NULL,NULL,NULL,@EnterpriseGroupingScenciaId);
