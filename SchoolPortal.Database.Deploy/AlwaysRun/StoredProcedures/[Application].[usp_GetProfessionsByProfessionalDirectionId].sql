CREATE OR ALTER PROC [Application].[usp_GetProfessionsByProfessionalDirectionId]
	@ProfessionalDirectionId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[Id],
		[ExternalId],
		[Name],
		[ProfessionalDirectionId]
	FROM
		[Application].[uv_Professions]
	WHERE
		[ProfessionalDirectionId] = @ProfessionalDirectionId
END;
GO