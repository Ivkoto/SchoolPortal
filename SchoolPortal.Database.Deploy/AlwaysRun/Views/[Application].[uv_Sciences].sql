CREATE OR ALTER VIEW [Application].[uv_Sciences] AS
SELECT
	sci.[Id],
	sci.[ExternalId],
	sci.[Name],
	syr.[Year] as SchoolYear
FROM
			[Application].[Science]		as sci
INNER JOIN	[Application].[SchoolYear]	as syr ON sci.SchoolYearId = syr.Id;
GO