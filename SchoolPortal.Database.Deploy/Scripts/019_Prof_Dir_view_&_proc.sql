-- get Professional Directions VIEW
CREATE OR ALTER VIEW [Application].[v_ProfessionalDirections] AS
SELECT 
	[Id],
    [ExternalId],
    [Name],
    [ScienceId]

FROM
	[Application].[ProfessionalDirection]
GO

-- get Professional Directions by Science Id PROC
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
		[Application].[v_ProfessionalDirections]
	WHERE
		[ScienceId] = @ScienceId
END;
GO
