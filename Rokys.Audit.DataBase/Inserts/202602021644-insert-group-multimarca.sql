DECLARE @EnterpriseId UNIQUEIDENTIFIER;
SELECT @EnterpriseId = EnterpriseId FROM [dbo].[Enterprise]
WHERE [Code] = '20612441198' AND IsActive = 1;

DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER;
SELECT @EnterpriseGroupingId = EnterpriseGroupingId FROM [dbo].[EnterpriseGrouping]
WHERE [Code] = 'EG002' AND IsActive = 1;

INSERT INTO [dbo].[Group](GroupId,EnterpriseId,[Name],Weighting,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,Code)
VALUES('044c67f8-1120-4073-af68-1e35590b5906',@EnterpriseId,'Grupo independiente - DEKIRU',100.00,1,1,'admin','2026-01-30T12:05:03.9894311','admin','2026-01-30T17:21:15.5727590',@EnterpriseGroupingId,'AG006');

INSERT INTO [dbo].[ScaleGroup] (ScaleGroupId,GroupId,Code,[Name],HasSourceData,SortOrder,Weighting,Recommendation,Impact,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate) VALUES
('c5aa7d19-3dca-48ff-a613-957bac44b853','044c67f8-1120-4073-af68-1e35590b5906','EAR-1','Arqueos inopinados de asados y buffet',0,1,10.00,NULL,NULL,1,'admin','2026-01-30T17:07:27.8819555','admin','2026-01-30T17:07:27.8819567'),
('30140331-8726-4d48-a439-2f053a8373c6','044c67f8-1120-4073-af68-1e35590b5906','EAR-2','Consumos promedios de carne',0,2,10.00,NULL,NULL,1,'admin','2026-01-30T17:07:52.7850436','admin','2026-01-30T17:07:52.7850450'),
('504afdd8-a1a4-4312-a348-5884554c4927','044c67f8-1120-4073-af68-1e35590b5906','EAR-3','Diferencias de inventario de almacén',0,3,5.00,NULL,NULL,1,'admin','2026-01-30T17:08:10.5760185','admin','2026-01-30T17:08:10.5760196'),
('fa9a9df3-4fd5-4e8f-9f1e-5ad245c46d3b','044c67f8-1120-4073-af68-1e35590b5906','EAR-4','Registro facturas y/o transferencias de otros locales',0,4,5.00,NULL,NULL,1,'admin','2026-01-30T17:09:08.2555144','admin','2026-01-30T17:09:08.2555154'),
('7c151b33-c806-49ff-9b46-31f8305b724e','044c67f8-1120-4073-af68-1e35590b5906','EAR-5','Operatividad de cámaras de video vigilancia',0,5,5.00,NULL,NULL,1,'admin','2026-01-30T17:12:00.5080741','admin','2026-01-30T17:12:00.5080751'),
('7c111b32-c301-41ff-1b16-51f3325b734e','044c67f8-1120-4073-af68-1e35590b5906','EAR-6','Registro de saldos reales de áreas en los formatos establecidos',0,6,5.00,NULL,NULL,1,'admin','2026-01-30T17:12:00.5080741','admin','2026-01-30T17:12:00.5080751'),
('c6afa306-5360-4c96-ae89-34ddc1f57bb2','044c67f8-1120-4073-af68-1e35590b5906','EAR-7','Mermas salad bar frios y calientes',0,7,5.00,NULL,NULL,1,'admin','2026-01-30T17:24:15.8152730','admin','2026-01-30T17:24:15.8152741'),
('a428fe0f-7406-4dc4-ad51-e7bde77adde2','044c67f8-1120-4073-af68-1e35590b5906','EAR-8','Sobre stocks de insumos',0,8,10.00,NULL,NULL,1,'admin','2026-01-30T17:24:28.3329472','admin','2026-01-30T17:24:28.3329483'),
('0b4ec161-d07c-4920-8e86-77343b414769','044c67f8-1120-4073-af68-1e35590b5906','EAR-9','Liquidacion sistema comercial',0,9,5.00,NULL,NULL,1,'admin','2026-01-30T17:24:41.1250389','admin','2026-01-30T17:24:41.1250400'),
('6723d827-92ca-4b0e-a8aa-dbd19ad53947','044c67f8-1120-4073-af68-1e35590b5906','EAR-10','Pago de movilidades',0,10,5.00,NULL,NULL,1,'admin','2026-01-30T17:24:54.5716604','admin','2026-01-30T17:24:54.5716615'),
('d606c309-9d89-4866-aa86-1daa0032d335','044c67f8-1120-4073-af68-1e35590b5906','EAR-11','Merma de papa amarilla frita',0,11,5.00,NULL,NULL,1,'admin','2026-01-30T17:25:06.6836921','admin','2026-01-30T17:25:06.6836932'),
('0591d3a6-dd4d-4e39-9af0-e6ec828320ff','044c67f8-1120-4073-af68-1e35590b5906','EAR-12','Diferencias de pollo',0,12,10.00,NULL,NULL,1,'admin','2026-01-30T17:25:16.4595245','admin','2026-01-30T17:25:16.4595256'),
('951adf55-838a-4fc1-9be2-7640632eec72','044c67f8-1120-4073-af68-1e35590b5906','EAR-13','Salida de consumo Buffet/Sin receta',0,13,5.00,NULL,NULL,1,'admin','2026-01-30T17:25:28.1020078','admin','2026-01-30T17:25:28.1020089'),
('48613d9b-d0e5-48e2-9575-4da76b0a4e55','044c67f8-1120-4073-af68-1e35590b5906','EAR-14','Cumplimiento procedimiento aceite quemado',0,14,10.00,NULL,NULL,1,'admin','2026-01-30T17:25:44.9479988','admin','2026-01-30T17:25:44.9480000'),
('bac4eada-ff4a-4b89-a6e7-5f81e722bd35','044c67f8-1120-4073-af68-1e35590b5906','EAR-15','Diferencia de sazon',0,15,5.00,NULL,NULL,1,'admin','2026-01-30T17:25:54.5016546','admin','2026-01-30T17:25:54.5016561');

SELECT @EnterpriseId = EnterpriseId FROM [dbo].[Enterprise]
WHERE [Code] = '20517656217' AND IsActive = 1;


INSERT INTO [dbo].[Group](GroupId,EnterpriseId,[Name],Weighting,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,Code)
VALUES('6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91',@EnterpriseId,'Grupo independiente - Rodizio',100.00,1,1,'admin','2026-01-30T12:05:03.9894311','admin','2026-01-30T17:21:15.5727590',@EnterpriseGroupingId,'AG007');

INSERT INTO [dbo].[ScaleGroup]
(ScaleGroupId,GroupId,Code,[Name],HasSourceData,SortOrder,Weighting,Recommendation,Impact,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate) VALUES
('1f4c9a8d-0b8a-4a6e-9c5a-8d3e2f7a1b01','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-1','Arqueos inopinados de Rodizios',0,1,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('2a6b7c8d-1c9e-4d5f-9a01-b2c3d4e5f602','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-2','Consumos promedios de carne',0,2,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('3b7d9e1a-2f3c-4a8e-b501-c6d7e8f9a703','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-3','Diferencias de inventario de almacén',0,3,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('4c8e1f2a-3d5b-4e9a-b604-d7e8f9a0b804','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-4','Tickets anulados',0,4,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('5d9f2a3b-4c6e-4a0b-9e05-f8a1b2c3d905','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-5','Operatividad de cámaras de video vigilancia',0,5,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('6e0a3b4c-5d7f-4b1c-ae06-1b2c3d4e0a06','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-6','Registro de saldos reales de áreas en los formatos establecidos',0,6,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('7f1b4c5d-6e8a-4c2d-b107-2c3d4e5f1b07','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-7','Mermas salad bar frios y calientes',0,7,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('8a2c5d6e-7f9b-4d3e-c208-3d4e5f6a2c08','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-8','Sobre stocks de insumos',0,8,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('9b3d6e7f-8a0c-4e4f-d309-4e5f6a7b3c09','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-9','Liquidación sistema comercial',0,9,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a0c4e7f8-9b1d-4f50-e40a-5f6a7b8c4d10','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-10','Pago de movilidades',0,10,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('b1d5f8a9-0c2e-4a61-f50b-6a7b8c9d5e11','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-11','Merma de papa amarilla frita',0,11,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('c2e6a9b0-1d3f-4b72-a60c-7b8c9d0e6f12','6f3a9d42-8a6c-4c4a-9c6a-3f9d8a2c7e91','ROD-12','Cumplimiento de procedimiento, quiebres de aceite quemado',0,12,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE());

SELECT @EnterpriseId = EnterpriseId FROM [dbo].[Enterprise]
WHERE [Code] = '20601824265' AND IsActive = 1;

DECLARE @GroupId UNIQUEIDENTIFIER = '9d2f6a84-3b1c-4e7f-8a92-0c5f3e1d6b44';

INSERT INTO [dbo].[Group](GroupId,EnterpriseId,[Name],Weighting,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,Code)
VALUES(@GroupId,@EnterpriseId,'Grupo independiente - Viena',100.00,1,1,'admin','2026-01-30T12:05:03.9894311','admin','2026-01-30T17:21:15.5727590',@EnterpriseGroupingId,'AG008');

INSERT INTO [dbo].[ScaleGroup] (ScaleGroupId, GroupId, Code, [Name], HasSourceData, SortOrder, Weighting, Recommendation, Impact, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate) VALUES
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c001',@GroupId,'EV-1','Diferencias de inventario de almacén',0,1,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c002',@GroupId,'EV-2','Registro de saldos reales de áreas en los formatos establecidos',0,2,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c003',@GroupId,'EV-3','Operatividad de cámaras de video vigilancia',0,3,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c004',@GroupId,'EV-4','Reporte de faltantes por Market',0,4,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c005',@GroupId,'EV-5','Reporte de bajas por vencimientos',0,5,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c006',@GroupId,'EV-6','Liquidación sistema comercial',0,6,5.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c007',@GroupId,'EV-7','Pago de movilidades',0,7,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c008',@GroupId,'EV-8','Sobre stock de insumos',0,8,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c009',@GroupId,'EV-9','Registro de facturas al sistema spring',0,9,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('a1b7f2c4-1c3e-4b4b-9a92-01f9a8a7c00a',@GroupId,'EV-10','Cumplimiento de procedimiento, quiebres de aceite quemado',0,10,10.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE());

SELECT @EnterpriseId = EnterpriseId FROM [dbo].[Enterprise]
WHERE [Code] = '20492979717' AND IsActive = 1;

INSERT INTO [dbo].[Group](GroupId,EnterpriseId,[Name],Weighting,SortOrder,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate,EnterpriseGroupingId,Code)
VALUES('3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18',@EnterpriseId,'Grupo independiente - Turisticas',100.00,1,1,'admin','2026-01-30T12:05:03.9894311','admin','2026-01-30T17:21:15.5727590',@EnterpriseGroupingId,'AG009');

INSERT INTO [dbo].[ScaleGroup] (ScaleGroupId,GroupId,Code,[Name],HasSourceData,SortOrder,Weighting,Recommendation,Impact,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdateDate) VALUES
('a1f2c7d3-0c8e-4e21-9a3b-1d9e4f5a6b01','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-1','Mermas de salad bar',0,1,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('b2e4a9c1-5f6d-4a87-9e02-3c1d8f7a6b12','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-2','Vencimientos',0,2,25.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('c3d5f8a2-6e41-4c9d-8b17-4e2a1d9f0b23','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-3','Operatividad de cámaras de video vigilancia',0,3,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('d4a6e1f3-7b52-4f0c-9d28-5b3c2a1e9f34','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-4','Diferencias de inventario almacén',0,4,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('e5b7c2a4-8c63-4a1d-8e39-6d4b3c2a1f45','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-5','Registro de facturas y/o transferencias a otros locales',0,5,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE()),
('f6c8d3b5-9d74-4b2e-9f40-7e5d4c3b2a56','3e8f4c9a-6b2d-4d8a-9e41-2a7b9f6d5c18','LQ-6','Ventas al crédito',0,6,15.00,NULL,NULL,1,'admin',GETDATE(),'admin',GETDATE());
