-- =====================================================================
-- PASO 1: MIGRACIÓN DE USUARIOS Y DATOS BÁSICOS
-- De: DBMemoQAV2 → A: DBSecurityQA
-- =====================================================================

--CREATE DATABASE DBSecurityQAV2

USE [DBSecurityQA]
GO


update DBMemoQA.dbo.Roles
set Code='APPADMIN'
where Code='R005'
--select * from [dbo].[Users]

-- Script para migrar usuarios de DBMemoQAV2 a DBSecurityQA
-- IMPORTANTE: Ejecutar este script después de verificar que las tablas estén vacías

INSERT INTO [dbo].[Users]
    ([UserId]
    ,[Username]
    ,[PasswordHash]
    ,[Status]
    ,[EmployeeId]
    ,[LastLoginAt]
    ,[PasswordResetToken]
    ,[PasswordResetTokenExpires]
    ,[CreatedAt]
    ,[UpdatedAt]
    ,[DateOfBirth]
    ,[Email]
    ,[FirstName]
    ,[LastName]
    ,[PhoneNumber])
SELECT 
    u.[Id] AS [UserId]                                    -- Mantener el mismo ID
    ,u.[Username] AS [Username]                           -- Username igual
    ,'wr9WdEU1ipQrYLrBioxSjnsa9qFBmwcBMHiqRpVX8bxglbDO5W7ndtX63ORWxPVz' AS [PasswordHash]  -- Password por defecto
    ,CASE 
        WHEN u.[IsActive] = 1 THEN 1                      -- Status: 1=Activo, 0=Inactivo
        ELSE 0 
    END AS [Status]
    ,u.[EmployeeId] AS [EmployeeId]                       -- Mantener EmployeeId
    ,u.[LastLogin] AS [LastLoginAt]                       -- Mapear LastLogin
    ,NULL AS [PasswordResetToken]                         -- Inicializar como NULL
    ,NULL AS [PasswordResetTokenExpires]                  -- Inicializar como NULL
    ,ISNULL(u.[CreationDate], GETDATE()) AS [CreatedAt]  -- CreationDate o fecha actual
    ,ISNULL(u.[UpdateDate], u.[CreationDate]) AS [UpdatedAt]  -- UpdateDate o CreationDate
    ,NULL AS [DateOfBirth]                                -- No disponible en origen, NULL
    ,e.[Email] AS [Email]                                 -- Email del empleado
    ,e.[FirstName] AS [FirstName]                         -- FirstName del empleado
    ,e.[LastName] AS [LastName]                           -- LastName del empleado
    ,e.[Phone] AS [PhoneNumber]                           -- Phone del empleado
FROM [DBMemoQA].[dbo].[Users] u
INNER JOIN [DBMemoQA].[dbo].[Employees] e ON u.[EmployeeId] = e.[Id]
WHERE u.[IsActive] = 1                                    -- Solo usuarios activos
    AND e.[IsActive] = 1   and u.Id!='eeeeeeee-1111-1111-1111-111111111111'                               -- Solo empleados activos
GO



update [DBMemoQAV2].[dbo].[Users] 
set SecurityUserId = Id

-- =====================================================================
-- VERIFICACIÓN DE LA MIGRACIÓN
-- =====================================================================

-- Contar registros migrados
SELECT 
    'Usuarios migrados' as Concepto,
    COUNT(*) as Cantidad
FROM [DBSecurityQAV2].[dbo].[Users]

UNION ALL

SELECT 
    'Usuarios origen (activos)' as Concepto,
    COUNT(*) as Cantidad
FROM [DBMemoQA].[dbo].[Users] u
INNER JOIN [DBMemoQA].[dbo].[Employees] e ON u.[EmployeeId] = e.[Id]
WHERE u.[IsActive] = 1 AND e.[IsActive] = 1
GO

-- =====================================================================
-- VALIDACIÓN DE DATOS MIGRADOS
-- =====================================================================

-- Verificar que no hay datos faltantes críticos
SELECT 
    'Sin Username' as Problema,
    COUNT(*) as Cantidad
FROM [DBSecurityQA].[dbo].[Users]
WHERE [Username] IS NULL OR [Username] = ''

UNION ALL

SELECT 
    'Sin Email' as Problema,
    COUNT(*) as Cantidad
FROM [DBSecurityQA].[dbo].[Users]
WHERE [Email] IS NULL OR [Email] = ''

UNION ALL

SELECT 
    'Sin FirstName' as Problema,
    COUNT(*) as Cantidad
FROM [DBSecurityQA].[dbo].[Users]
WHERE [FirstName] IS NULL OR [FirstName] = ''

UNION ALL

SELECT 
    'Sin LastName' as Problema,
    COUNT(*) as Cantidad
FROM [DBSecurityQA].[dbo].[Users]
WHERE [LastName] IS NULL OR [LastName] = ''
GO

-- =====================================================================
-- CONSULTA PARA REVISAR LOS DATOS MIGRADOS
-- =====================================================================

 -- Insertar todos los usuarios en la aplicación SECURITY
/*INSERT INTO [DBSecurityQA].[dbo].[UserApplications] 
([UserId], [ApplicationId], [IsActive], [AssignedAt], [CreatedAt], [UpdatedAt])
SELECT 
    u.[UserId],
    '0DAF922E-C54B-448F-90F2-31A70E378565' AS ApplicationId, -- SECURITY
    1 AS IsActive,
    GETDATE() AS AssignedAt,
    GETDATE() AS CreatedAt,
    GETDATE() AS UpdatedAt
FROM [DBSecurityQA].[dbo].[Users] u
WHERE NOT EXISTS (
    SELECT 1 FROM [DBSecurityQA].[dbo].[UserApplications] ua 
    WHERE ua.UserId = u.UserId 
    AND ua.ApplicationId = '0DAF922E-C54B-448F-90F2-31A70E378565'
); */



--select * from [DBSecurityQAV2].[dbo].[Users]
-- Insertar todos los usuarios en la aplicación MEMOS
INSERT INTO [dbo].[UserApplications] 
([UserId], [ApplicationId], [IsActive], [AssignedAt], [CreatedAt], [UpdatedAt])
SELECT 
    u.[UserId],
   (select top 1 ApplicationId from Applications where code ='MEMOS') AS ApplicationId, -- MEMO
    1 AS IsActive,
    GETDATE() AS AssignedAt,
    GETDATE() AS CreatedAt,
    GETDATE() AS UpdatedAt
FROM [dbo].[Users] u
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[UserApplications] ua 
    WHERE ua.UserId = u.UserId 
    AND ua.ApplicationId = (select top 1 ApplicationId from Applications where code ='MEMOS') 
); 

-- Insertar todos los usuarios en la aplicación MEMOS


-- PASO 2: Migrar roles masivamente usando el RoleId original
-- Ahora sí migrar los roles (tu script original)
INSERT INTO [dbo].[UserRoles]
([UserId], [RoleId], [AssignedAt], [CreatedAt], [UpdatedAt])
SELECT 
    u_dest.[UserId],
    (SELECT RoleId FROM [Roles] WHERE Code=r.Code 
     AND ApplicationId=(SELECT TOP 1 ApplicationId FROM [Applications] WHERE Code = 'MEMOS')) AS RoleId,
    GETDATE() AS AssignedAt,
    GETDATE() AS CreatedAt,
    GETDATE() AS UpdatedAt
FROM [DBMemoQA].[dbo].[Users] u_orig
INNER JOIN [DBMemoQA].[dbo].[Roles] r ON u_orig.RoleId = r.Id
INNER JOIN [dbo].[Users] u_dest ON u_orig.[Id] = u_dest.[UserId]
WHERE u_orig.[IsActive] = 1
    AND u_orig.Id != 'eeeeeeee-1111-1111-1111-111111111111'
    AND u_orig.[RoleId] IS NOT NULL
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[UserRoles] ur 
        WHERE ur.UserId = u_dest.UserId
    );



USE DBAuditQA
--select * from Roles
--where ApplicationId= (select top 1 ApplicationId from Applications where code ='MEMOS') 
-- SCRIPT FOR AUDIT
INSERT INTO [dbo].[Enterprise]
(
    [EnterpriseId],
    [Name],
    [Code],
    [Address],
    [IsActive],
    [CreatedBy],
    [CreationDate],
    [UpdatedBy],
    [UpdateDate]
)
SELECT
    a.[Id] AS [EnterpriseId],
    a.[Name],
    a.[Code],
    a.[Address],
    a.[IsActive],
    a.[CreatedBy],
    a.[CreationDate],
    a.[UpdatedBy],
    a.[UpdateDate]
FROM [DBMemoQA].[dbo].[Enterprise] a
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[Enterprise] b
    WHERE b.[Code] = a.[Code]
);


INSERT INTO [dbo].[Stores]
(
    [StoreId],
    [Name],
    [Code],
    [Address],
    [EnterpriseId],
    [IsActive],
    [CreatedBy],
    [CreationDate],
    [UpdatedBy],
    [UpdateDate]
)
SELECT
    a.[Id] AS [StoreId],
    a.[Name],
    a.[Code],
    a.[Address],
    a.[EnterpriseId],
    a.[IsActive],
    a.[CreatedBy],
    a.[CreationDate],
    a.[UpdatedBy],
    a.[UpdateDate]
FROM [DBMemoQA].[dbo].[Stores] a
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[Stores] b
    WHERE b.[Code] = a.[Code]
);
