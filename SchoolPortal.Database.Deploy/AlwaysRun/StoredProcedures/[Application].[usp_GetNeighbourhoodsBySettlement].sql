CREATE OR ALTER PROC [Application].[usp_GetNeighbourhoodsBySettlement]
	@Settlement NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT
		[Neighbourhood]
	FROM
		[Application].[uv_Addresses]
	WHERE
		[Settlement] = @Settlement AND [Neighbourhood] IS NOT NULL;
END;
GO
