-- Script para insertar registros en EnterpriseGroup basándose en RUC de Enterprise
-- Fecha: 2026-01-21

-- Declarar variables para almacenar los IDs
DECLARE @EnterpriseGroupingId UNIQUEIDENTIFIER = '64106CA9-A15A-4D3A-8EAB-58C3930E3A7D';
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
    '20517656217',  -- INVERSIONES INDEPENDENCIA SAC
    '20608029207',  -- BENTEN S.A.C
    '20306689950',  -- PRINCES SAC
    '20492979717',  -- INVERSIONES TURISTICAS KILLA SAC
    '20432168213',  -- IINVERSIONES HOUSE CHICKEN SAC
    '20476633932',  -- INVERSIONES TOMAS VALLE SAC
    '20537869799',  -- INVERSIONES COMERCIALES NAZCA S.A.C.
    '20612441198',  -- INVERSIONES DEKIRU S.A.C
    '20418463644',  -- INVERSIONES NOR CHICKEN SAC
    '20601824265',  -- INVERSIONES VIENA DEL PERU S.A.C
    '20517656641',  -- INVERSIONES PLAZA SAN MARTIN SAC
    '20607478695'   -- BISHAMONTEN E.I.R.L.
);
