CREATE OR ALTER PROC [Application].[usp_GetAllSciences]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ExternalId],
		[Name]
	FROM 
		[Application].[uv_Sciences]
END;
GO