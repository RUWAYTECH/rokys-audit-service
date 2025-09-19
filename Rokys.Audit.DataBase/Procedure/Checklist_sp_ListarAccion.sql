SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Checklist].[sp_ListarAccion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Checklist].[sp_ListarAccion] AS' 
END
GO
	ALTER PROCEDURE [Checklist].[sp_ListarAccion]
	AS
	BEGIN
		SELECT 
        [Accion],
        [Descripcion],
        [Estado]
    FROM 
        [Checklist].[Accion];
	END
GO