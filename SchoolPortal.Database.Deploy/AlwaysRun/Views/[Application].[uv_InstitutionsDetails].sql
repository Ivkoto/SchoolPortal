CREATE OR ALTER VIEW [Application].[uv_InstitutionsDetails] AS
SELECT
	sinst.[Id]					as InstitutionId,
	sinst.[Kind]				as Kind,
	sinst.[Director]			as Director,
	sinst.[Websites]			as Websites,
	sinst.[Emails]				as Emails,
	sinst.[PhoneNumbers]		as PhoneNumbers,
	adr.[Address]				as AddressOfActivity,
	adr.[Area]					as Area,
	adr.[Municipality]			as Municipality,
	adr.[Region]				as Region,
	adr.[Settlement]			as Settlement,
	adr.[SettlementType]		as SettlementType,
	adr.[Neighbourhood]			as Neighbourhood,
	adr.[PostalCode]			as PostalCode,
	adr.[Latitude]              as GeoLatitude,
	adr.[Longitude]				as GeoLongitide,
	inst.[ExternalId]			as ExternalId,
	inst.[IsActive]				as IsActive,
	inst.[EIK]					as EIK,
	inst.[FullName]				as FullName,
	inst.[ShortName]			as ShortName,
	instprep.[PreparationType]	as PreparationType,
	inststat.[Status]			as InstStatus,
	instfin.[FinancingType]		as FinancingType,
	instown.[Ownership]			as InstOwnership

FROM 		[Application].[SubInstitution]				as sinst
INNER JOIN	[Application].[Institution]					as inst		ON sinst.[InstitutionId] = inst.[Id]
LEFT JOIN	[Application].[InstitutionPreparationType]	as instprep	ON sinst.[PreparationTypeId] = instprep.[Id]
LEFT JOIN	[Application].[InstitutionFinancingType]	as instfin	ON inst.[FinancingTypeId] = instfin.[Id]
LEFT JOIN	[Application].[InstitutionStatus]			as inststat	ON inst.[StatusId] = inststat.[Id]
LEFT JOIN	[Application].[InstitutionOwnership]		as instown	ON inst.[OwnershipId] = instown.[Id]
LEFT JOIN	[Application].[Address]					 	as adr		ON sinst.[AddressOfActivityId] = adr.[Id];
GO
