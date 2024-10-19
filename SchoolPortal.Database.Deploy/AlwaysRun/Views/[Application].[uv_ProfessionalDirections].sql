CREATE OR ALTER VIEW [Application].[uv_ProfessionalDirections] AS
SELECT 
	[Id],
    [ExternalId],
    [Name],
    [ScienceId]

FROM
	[Application].[ProfessionalDirection];
GO