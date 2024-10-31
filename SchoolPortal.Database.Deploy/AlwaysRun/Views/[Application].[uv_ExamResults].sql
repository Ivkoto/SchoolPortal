
CREATE OR ALTER VIEW [Application].[uv_ExamResults] AS
SELECT
	schy.[Year]				as [SchoolYear],
	er.[SubInstitutionId]	as [InstitutionId],
	exabr.[Abbreviation]	as [ExamAbbreviation],
	suabr.[FullName]		as [SubjectName],
	suabr.[ShortName]		as [SubjectAbbreviation],
	er.[PreparationType]	as [PreparationType],
	er.[CandidateCount]		as [CandidateCount],
	er.[AverageSuccess]		as [AverageScore],
	er.[Grade]				as [Grade],
	er.[LevelType]			as [LevelType]

FROM		[Application].[ExamResult]			as er
LEFT JOIN	[Application].[SubInstitution]		as sinst	ON er.[SubInstitutionId] = sinst.[Id]
LEFT JOIN	[Application].[SchoolYear]			as schy		ON er.[SchoolYearId] = schy.[Id]
LEFT JOIN	[Application].[ExamAbbreviation]	as exabr	ON er.[ExamAbbreviationId] = exabr.[Id]
LEFT JOIN	[Application].[SubjectAbbreviation] as suabr	ON er.[SubjectAbbreviationId] = suabr.[Id]
GO
