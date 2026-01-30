-- Script para insertar registros en EnterpriseGroup basándose en RUC de Enterprise
-- Fecha: 2026-01-21
DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER = '64106CA9-A15A-4D3A-8EAB-58C3930E3A7D';

IF NOT EXISTS (SELECT 1 FROM EnterpriseGrouping WHERE EnterpriseGroupingId = @EnterpriseGroupingId)
BEGIN
    INSERT INTO EnterpriseGrouping (EnterpriseGroupingId, Code, Name, Description, IsActive, CreatedBy, CreationDate)
    VALUES (@EnterpriseGroupingId, 'EG001', 'Grupo de Empresas por Rokys', 'Agrupación de empresas basada en códigos RUC par Rokys', 1, 'Admin', '2026-01-21 09:18:28.226');
END

-- Declarar variables para almacenar los IDs

DECLARE @CreatedBy NVARCHAR(50) = 'Admin';
DECLARE @CreationDate DATETIME = '2026-01-21 09:18:28.226';

-- Insertar registros en EnterpriseGroup utilizando los RUC de Enterprise
INSERT INTO EnterpriseGroup (EnterpriseGroupId, EnterpriseId, EnterpriseGroupingId, IsActive, CreatedBy, CreationDate)
SELECT 
    NEWID() AS EnterpriseGroupId,
    e.EnterpriseId,
    @EnterpriseGroupingId AS EnterpriseGroupingId,
    1 AS IsActive,
    @CreatedBy AS CreatedBy,
    @CreationDate AS CreationDate
FROM Enterprise e
WHERE e.Code IN (
    '20513427710',  -- GRUPO ROKYS S.A.C.
    '20608029207',  -- BENTEN S.A.C
    '20306689950',  -- PRINCES SAC
    '20432168213',  -- IINVERSIONES HOUSE CHICKEN SAC
    '20476633932',  -- INVERSIONES TOMAS VALLE SAC
    '20537869799',  -- INVERSIONES COMERCIALES NAZCA S.A.C.
    '20418463644',  -- INVERSIONES NOR CHICKEN SAC
    '20517656641',  -- INVERSIONES PLAZA SAN MARTIN SAC
    '20607478695'   -- BISHAMONTEN E.I.R.L.
);
