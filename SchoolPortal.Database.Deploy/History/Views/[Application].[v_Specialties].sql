CREATE OR ALTER VIEW [Application].[v_Specialties] AS
SELECT
	[Id],
	[ExternalId],
	[Name],
	[IsProfessional],
	[ProfessionId]
FROM
	[Application].[Specialty]