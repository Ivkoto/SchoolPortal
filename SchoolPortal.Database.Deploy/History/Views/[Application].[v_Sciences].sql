-- get all Sciences VIEW
CREATE OR ALTER VIEW [Application].[v_Sciences] AS
SELECT
	[Id],
	[ExternalId],
	[Name]

FROM [Application].[Science]
GO