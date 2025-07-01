CREATE OR ALTER PROC [Application].[usp_GetAllSciences]
	@SchoolYear INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ExternalId],
		[Name],
		[SchoolYear]
	FROM 
		[Application].[uv_Sciences]
	WHERE
		[SchoolYear] = @SchoolYear
END;
GO