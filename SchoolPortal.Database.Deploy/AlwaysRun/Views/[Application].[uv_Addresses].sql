CREATE OR ALTER VIEW [Application].[uv_Addresses] AS
SELECT
	[Id],
	[Area],
	[Municipality],
	[Region],
	[Settlement],
	[SettlementType],
	[Neighbourhood],
	[Address],
	[PostalCode],
	[Latitude],
	[Longitude]
FROM
	[Application].[Address];
GO
