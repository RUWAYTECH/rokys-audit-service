declare @EnterpriseId uniqueidentifier = (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20513427710')
declare @EnterpriseGroupingId uniqueidentifier = (SELECT EnterpriseGroupingId FROM dbo.EnterpriseGrouping WHERE Code = 'EG001')

UPDATE dbo.[Group]
SET IsActive = 0
WHERE EnterpriseId != @EnterpriseId;

UPDATE dbo.[Group] 
SET EnterpriseGroupingId = @EnterpriseGroupingId, EnterpriseId = null
WHERE EnterpriseId = @EnterpriseId ;

UPDATE dbo.[Group] 
SET Code='AG001'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='DINERO (Caja / Bancos / Ventas)';


UPDATE dbo.[Group] 
SET Code='AG002'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='GESTION DE INVENTARIOS (Impacto en riesgos, lectura y resultados de P&L)';

UPDATE dbo.[Group] 
SET Code='AG003'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='CUMPLIMIENTO NORMATIVO (Impacto en Gestión Inventarios)';

UPDATE dbo.[Group] 
SET Code='AG004'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='CUMPLIMIENTO NORMATIVO (Impacto en Back Office)';