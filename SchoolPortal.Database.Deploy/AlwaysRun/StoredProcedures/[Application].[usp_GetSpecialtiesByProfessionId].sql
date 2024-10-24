CREATE OR ALTER PROC [Application].[usp_GetSpecialtiesByProfessionId]
	@ProfessionId	INT	= NULL,
	@IsProfessional	INT
AS
BEGIN
    SET NOCOUNT ON;

	SELECT
		[Id],
		[ExternalId],
		[Name],
		[IsProfessional],
		[ProfessionId]
	FROM
		[Application].[uv_Specialties]
	WHERE
		(@ProfessionId	 IS NULL OR [ProfessionId] = @ProfessionId)
		AND [IsProfessional] = @IsProfessional;
END;
GO