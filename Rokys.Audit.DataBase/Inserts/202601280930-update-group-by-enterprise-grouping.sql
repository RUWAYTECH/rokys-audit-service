declare @EnterpriseId uniqueidentifier = (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20513427710')
declare @EnterpriseGroupingId uniqueidentifier = (SELECT EnterpriseGroupingId FROM dbo.EnterpriseGrouping WHERE Code = 'EG001')

UPDATE dbo.[Group]
SET IsActive = 0
WHERE EnterpriseId != @EnterpriseId;

UPDATE dbo.[Group] 
SET EnterpriseGroupingId = @EnterpriseGroupingId, EnterpriseId = null
WHERE EnterpriseId = @EnterpriseId ;
