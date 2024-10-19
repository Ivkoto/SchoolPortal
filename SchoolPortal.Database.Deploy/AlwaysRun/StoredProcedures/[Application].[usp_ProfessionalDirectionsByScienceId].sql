CREATE OR ALTER PROC [Application].[usp_ProfessionalDirectionsByScienceId]
	@ScienceId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[Id],
		[ExternalId],
		[Name],
		[ScienceId]
	FROM
		[Application].[uv_ProfessionalDirections]
	WHERE
		[ScienceId] = @ScienceId
END;
GO