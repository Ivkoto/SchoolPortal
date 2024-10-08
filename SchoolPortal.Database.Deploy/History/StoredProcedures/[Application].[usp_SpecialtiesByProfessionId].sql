CREATE OR ALTER PROC [Application].[usp_SpecialtiesByProfessionId]
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
		[Application].[v_Specialties]
	WHERE
		(@ProfessionId	 IS NULL OR [ProfessionId] = @ProfessionId)
		AND [IsProfessional] = @IsProfessional;
END;