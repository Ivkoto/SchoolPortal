CREATE OR ALTER VIEW [Application].[uv_Professions] AS
SELECT 
	[Id],
    [ExternalId],
    [Name],
    [ProfessionalDirectionId]

FROM
	[Application].[Profession];
GO