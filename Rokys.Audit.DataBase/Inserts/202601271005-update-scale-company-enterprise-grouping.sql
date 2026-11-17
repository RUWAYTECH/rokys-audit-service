update dbo.ScaleCompany
set EnterpriseGroupingId = (select top 1 EnterpriseGroupingId
							from EnterpriseGrouping eg  where eg.Code='EG001' )
where CreationDate < '2025-12-31';