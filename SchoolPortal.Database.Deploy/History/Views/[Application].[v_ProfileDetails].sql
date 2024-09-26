CREATE OR ALTER VIEW [Application].[v_ProfileDetails] AS
SELECT
	p.[Id]								as ProfileId,
	p.[Name]							as ProfileName,
	p.[Type]							as ProfileType,
	p.[Grade]							as Grade,
	p.[StudyPeriod] 					as StudyPeriod,
	p.[SubInstitutionId]				as InstitutionId,
	inst.[FullName]						as InstitutionFullName,
	pd.[GradingFormulas]				as GradingFormulas,
	pd.[StudyMethod]					as StudyMethod,
	pd.[EducatingType]					as EducationType,
	pd.[ClassesCount]					as ClassesCount,
	pd.[FirstForeignLanguage]			as FirstForeignLanguage,
	scy.[Year]							as SchoolYear,
	pd.[IsPaperOnly]					as IsPaperOnly,
	pd.[ExternalId]						as ExternalId,
	pd.[Quotas_Total]					as QuotasTotal,
	pd.[Quotas_Male]					as QuotasMale,
	pd.[Quotas_Female]					as QuotasFemale,
	sty.[Id]							as SpecialtyId,
	sty.[Name]							as Specialty,
	sty.[ExternalId]					as SpecialtyExternalId,
	sty.[ProfessionalQualificationLevel]as ProfessionalQualificationLevel,
	sty.[IsProtected]					as IsProtected,
	sty.[HasExpectedShortage]			as HasExpectedShortage,
	sty.[IsProfessional]				as IsProfessional,
	sty.[Description]					as SpecialtyDescription,
	prof.[Id]							as ProfessionId,
	prof.[Name]							as Profession,
	prof.[ExternalId]					as ProfessionExternalId,
	profd.[Id]							as ProfessionalDirectionId,
	profd.[Name]						as ProfessionalDirection,
	profd.[ExternalId]					as ProfessionalDirectionExternalId,
	sci.[Id]							as ScienceId,
	sci.[Name]							as Science,
	sci.[ExternalId]					as ScienceExternalId,
	adr.[Area]							as Area,
	adr.[Settlement]					as Settlement,
	adr.[Region]						as Region,
	adr.[Neighbourhood]					as Neighbourhood,
	adr.[Latitude]						as GeoLatitude,
	adr.[Longitude]						as GeoLongitude

FROM	  [Application].[Profile]				as p
LEFT JOIN [Application].[ProfileDetails]		as pd		ON pd.[ProfileId] = p.[Id]
LEFT JOIN [Application].[Specialty]				as sty		ON pd.[SpecialtyId] = sty.[Id]
LEFT JOIN [Application].[Profession]			as prof		ON sty.[ProfessionId] = prof.[Id]
LEFT JOIN [Application].[ProfessionalDirection] as profd	ON prof.[ProfessionalDirectionId] = profd.[Id]
LEFT JOIN [Application].[Science]				as sci		ON profd.[ScienceId] = sci.[Id]
LEFT JOIN [Application].[SchoolYear]			as scy		ON pd.[SchoolYearId] = scy.[Id]
INNER JOIN [Application].[SubInstitution]		as sinst	ON p.[SubInstitutionId] = sinst.[Id]
INNER JOIN [Application].[Institution]			as inst		ON sinst.[InstitutionId] = inst.[Id]
LEFT JOIN [Application].[Address]				as adr		ON sinst.[AddressOfActivityId] = adr.[Id]