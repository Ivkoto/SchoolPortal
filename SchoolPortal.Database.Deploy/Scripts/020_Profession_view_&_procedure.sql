-- get professions view
CREATE OR ALTER VIEW [Application].[v_Professions] AS
SELECT 
	[Id],
    [ExternalId],
    [Name],
    [ProfessionalDirectionId]

FROM
	[Application].[Profession]
GO

-- get Professions by Professional Direction Id PROC
CREATE OR ALTER PROC [Application].[usp_ProfessionsByProfessionalDirectionId]
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
		[Application].[v_Professions]
	WHERE
		[ProfessionalDirectionId] = @ProfessionalDirectionId
END;
GO