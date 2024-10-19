CREATE OR ALTER VIEW [Application].[uv_Specialties] AS
SELECT
	[Id],
	[ExternalId],
	[Name],
	[IsProfessional],
	[ProfessionId]
FROM
	[Application].[Specialty];
GO