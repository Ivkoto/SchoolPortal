CREATE OR ALTER VIEW [Application].[v_ProfessionalDirections] AS
SELECT 
	[Id],
    [ExternalId],
    [Name],
    [ScienceId]

FROM
	[Application].[ProfessionalDirection]