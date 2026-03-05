INSERT INTO [dbo].[ScaleCompany] (ScaleCompanyId,EnterpriseId,Code,[Name],MinValue,MaxValue,ColorCode,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,NormalizedScore,ExpectedDistribution,LevelOrder) VALUES
('59dec4f2-3344-4a84-a6df-a83be50f6c7b',NULL,'SC-0003','En Objetivo',91.00,100.00,'#417505',6,1,'admin','2026-02-02T12:05:31.8575408','admin','2026-02-02T12:05:31.8575420','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21',1.90,10.00,1),
('9ed122e4-56e2-4657-a893-5ad3aefb98e4',NULL,'SC-0004','Con Observaciones',81.00,90.00,'#f5a623',7,1,'admin','2026-02-02T12:06:24.3573434','admin','2026-02-02T12:06:24.3573444','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21',1.00,10.00,2),
('1a8ddaf2-028a-491a-aa66-d21d779ceb14',NULL,'SC-0005','Con Obs. Relevantes',0.00,80.00,'#d0021b',8,1,'admin','2026-02-02T12:07:00.8156766','admin','2026-02-02T12:07:00.8156771','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21',0.00,80.00,3);

INSERT INTO [dbo].[SubScale] (SubScaleId,EnterpriseGroupingId,Code,[Name],[Value],ColorCode,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate) VALUES
('29b9abde-9d2f-4b23-892e-69d2b2526629','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21','SC001','Malo',0.00,'#c70000',1,'admin','2026-02-02T11:58:01.533','admin','2026-02-02T11:58:01.533'),
('6a9955a4-7032-4b21-b093-8de8e5f8cd4c','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21','SC002','Regular',1.00,'#a35f00',1,'admin','2026-02-02T12:00:36.383','admin','2026-02-02T12:00:36.383'),
('2f2fe197-f98c-4403-a26a-80ea347c7f16','b8f3a2d1-6c4e-4f2b-9c71-1d4a8e9f0a21','SC003','Bueno',2.00,'#4CAF50',1,'admin','2026-02-02T12:00:55.363','admin','2026-02-02T12:00:55.363');

DECLARE @EnterpriseGroupingBrassId UNIQUEIDENTIFIER;

SELECT @EnterpriseGroupingBrassId = EnterpriseGroupingId
FROM dbo.EnterpriseGrouping
WHERE Code = 'EG003';

INSERT INTO dbo.ScaleCompany(ScaleCompanyId,EnterpriseId,Code,Name,MinValue,MaxValue,ColorCode,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,NormalizedScore,ExpectedDistribution,LevelOrder)
VALUES
(NEWID(),NULL,'SC-0002','Riesgo Bajo',91.00,95.00,'#4a90e2',2,1,'admin','2025-12-10 15:52:34.5266667','admin','2025-12-10 15:52:34.5266667',@EnterpriseGroupingBrassId,NULL,NULL,1),

(NEWID(),NULL,'SC-0003','Riesgo Moderado',81.00,90.00,'#d4c713',3,1,'admin','2025-12-10 15:52:34.5266667','admin','2025-12-10 15:52:34.5266667',@EnterpriseGroupingBrassId,NULL,NULL,1),

(NEWID(),NULL,'SC-0004','Riesgo Elevado',71.00,80.00,'#f5a623',4,1,'admin','2025-12-10 15:52:34.5266667','admin','2025-12-10 15:52:34.5266667',@EnterpriseGroupingBrassId,NULL,NULL,1),

(NEWID(),NULL,'SC-0005','Riesgo Crítico',0.00,70.00,'#d0021b',5,1,'admin','2025-12-10 15:52:34.5266667','admin','2025-12-10 15:52:34.5266667',@EnterpriseGroupingBrassId,NULL,NULL,1),

(NEWID(),NULL,'SC-0001','En Objetivo',96.00,100.00,'#417505',1,1,'system.admin','2025-12-10 15:52:34.5266667','admin','2025-12-10 15:52:34.5266667',@EnterpriseGroupingBrassId,NULL,NULL,1);
