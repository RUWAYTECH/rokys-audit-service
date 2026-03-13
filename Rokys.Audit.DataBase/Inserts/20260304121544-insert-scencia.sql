
INSERT INTO Enterprise 
    (EnterpriseId, Name, Code, Address, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES
    (
        'c0e70cde-5885-4695-a929-783fbb3dcd23',
        'INVERSIONES NOR CHICKEN  S.A.C.',
        '00000000000',
        NULL,
        1,
        'system',
        '2025-09-23 18:07:15.823',
        'system.default.async',
        '2025-12-03 15:01:57.392'
    );



INSERT INTO Stores 
    (StoreId, Name, Code, Address, EnterpriseId, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate, Email)
VALUES
    (
        NEWID(),
        'MOLINA SCENCIA',
        '167',
        NULL,
        'c0e70cde-5885-4695-a929-783fbb3dcd23',
        1,
        'system',
        '2025-09-23 18:07:15.823',
        'system.default.async',
        '2025-12-03 15:01:57.392',
        NULL
    );

INSERT INTO EnterpriseGroup 
    (EnterpriseGroupId, EnterpriseId, EnterpriseGroupingId, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate)
VALUES
    (
        NEWID(),
        'C0E70CDE-5885-4695-A929-783FBB3DCD23',
        'D7E84B21-9F3A-4E6C-8B52-4C9E1A6F2B88',
        1,
        'system',
        '2025-09-23 18:07:15.823',
        'system.default.async',
        '2025-12-03 15:01:57.392'
    );



INSERT INTO ScaleCompany 
(ScaleCompanyId, EnterpriseId, Code, Name, MinValue, MaxValue, ColorCode, 
 SortOrder, IsActive, CreatedBy, CreationDate, UpdatedBy, UpdateDate, 
 EnterpriseGroupingId, NormalizedScore, ExpectedDistribution, LevelOrder)
SELECT 
    newid(),
    EnterpriseId,
    Code,
    Name,
    MinValue,
    MaxValue,
    ColorCode,
    SortOrder,
    IsActive,
    CreatedBy,
    getdate(),
    UpdatedBy,
    UpdateDate,
    'D7E84B21-9F3A-4E6C-8B52-4C9E1A6F2B88',
    NormalizedScore,
    ExpectedDistribution,
    LevelOrder
FROM ScaleCompany
WHERE EnterpriseGroupingId = 'C3A91F4E-2D7B-4C8F-9A6E-1E52D8B4F101'

