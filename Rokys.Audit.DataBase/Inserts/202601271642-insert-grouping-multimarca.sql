INSERT INTO EnterpriseGrouping
(
    EnterpriseGroupingId,
    Code,
    Name,
    Description,
    IsActive,
    CreatedBy,
    CreationDate
)
VALUES
(
    'B8F3A2D1-6C4E-4F2B-9C71-1D4A8E9F0A21',
    'EG002',
    'Grupo de Empresas Multimarca',
    'Agrupación de empresas bajo el esquema Multimarca',
    1,
    'Admin',
    '2026-01-26 16:36:26.116'
);


INSERT INTO EnterpriseGrouping
(
    EnterpriseGroupingId,
    Code,
    Name,
    Description,
    IsActive,
    CreatedBy,
    CreationDate
)
VALUES
(
    'C3A91F4E-2D7B-4C8F-9A6E-1E52D8B4F101',
    'EG003',
    'Grupo de Empresas Brass',
    'Agrupaci�n de empresas bajo el esquema Brass',
    1,
    'Admin',
    '2026-01-26 16:36:26.116'
);

INSERT INTO EnterpriseGrouping
(
    EnterpriseGroupingId,
    Code,
    Name,
    Description,
    IsActive,
    CreatedBy,
    CreationDate
)
VALUES
(
    'D7E84B21-9F3A-4E6C-8B52-4C9E1A6F2B88',
    'EG004',
    'Grupo de Empresas Scencia',
    'Agrupaci�n de empresas bajo el esquema Scencia',
    1,
    'Admin',
    '2026-01-26 16:36:26.116'
);

declare @EnterpriseGroupingId uniqueidentifier = (SELECT EnterpriseGroupingId FROM dbo.EnterpriseGrouping WHERE Code = 'EG002')

INSERT INTO [dbo].[EnterpriseGroup]
(
    EnterpriseGroupId,
    EnterpriseId,
    EnterpriseGroupingId,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
)
VALUES
(
    NEWID(),                -- EnterpriseGroupId
    (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20612441198'),
    @EnterpriseGroupingId,  -- EnterpriseGroupingId (GUID)
    1,                      -- IsActive
    'Admin',                -- CreatedBy
    GETDATE(),              -- CreationDate
    NULL,                   -- UpdatedBy
    NULL                    -- UpdateDate
);



INSERT INTO [dbo].[EnterpriseGroup]
(
    EnterpriseGroupId,
    EnterpriseId,
    EnterpriseGroupingId,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
)
VALUES
(
    NEWID(),                -- EnterpriseGroupId
    (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20601824265'),
    @EnterpriseGroupingId,  -- EnterpriseGroupingId (GUID)
    1,                      -- IsActive
    'Admin',                -- CreatedBy
    GETDATE(),              -- CreationDate
    NULL,                   -- UpdatedBy
    NULL                    -- UpdateDate
);

INSERT INTO [dbo].[EnterpriseGroup]
(
    EnterpriseGroupId,
    EnterpriseId,
    EnterpriseGroupingId,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
)
VALUES
(
    NEWID(),                -- EnterpriseGroupId
    (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20517656217'),
    @EnterpriseGroupingId,  -- EnterpriseGroupingId (GUID)
    1,                      -- IsActive
    'Admin',                -- CreatedBy
    GETDATE(),              -- CreationDate
    NULL,                   -- UpdatedBy
    NULL                    -- UpdateDate
);

INSERT INTO [dbo].[EnterpriseGroup]
(
    EnterpriseGroupId,
    EnterpriseId,
    EnterpriseGroupingId,
    IsActive,
    CreatedBy,
    CreationDate,
    UpdatedBy,
    UpdateDate
)
VALUES
(
    NEWID(),                -- EnterpriseGroupId
    (SELECT EnterpriseId FROM dbo.Enterprise WHERE Code = '20492979717'),
    @EnterpriseGroupingId,  -- EnterpriseGroupingId (GUID)
    1,                      -- IsActive
    'Admin',                -- CreatedBy
    GETDATE(),              -- CreationDate
    NULL,                   -- UpdatedBy
    NULL                    -- UpdateDate
);