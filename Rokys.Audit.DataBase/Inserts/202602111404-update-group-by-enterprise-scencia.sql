declare @EnterpriseGroupingId uniqueidentifier = (SELECT EnterpriseGroupingId FROM dbo.EnterpriseGrouping WHERE Code = 'EG003')

UPDATE dbo.[Group] 
SET Code='AG010'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='DINERO (Caja / Bancos / Ventas) - Copia';

UPDATE dbo.[Group] 
SET Code='AG011'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='GESTION DE INVENTARIOS (Impacto en riesgos, lectura y resultados de P&L) - Copia';

UPDATE dbo.[Group] 
SET Code='AG012'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='CUMPLIMIENTO NORMATIVO (Impacto en Gestión Inventarios) - Copia';

UPDATE dbo.[Group] 
SET Code='AG013'
WHERE EnterpriseGroupingId = @EnterpriseGroupingId and Name='CUMPLIMIENTO NORMATIVO (Impacto en Back Office) - Copia';