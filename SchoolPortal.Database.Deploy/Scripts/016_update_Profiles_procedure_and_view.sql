-- Function with Haversine formula to calculate distance
CREATE OR ALTER FUNCTION [Application].[fn_CalculateDistanceHaversine] 
(
    @Latitude       DECIMAL(17, 15),  -- (input)
    @Longitude      DECIMAL(18, 15),  -- (input)
    @GeoLatitude    DECIMAL(17, 15),  -- (table)
    @GeoLongitude   DECIMAL(18, 15)   -- (table)
)
RETURNS DECIMAL(18, 6)
AS
BEGIN
    DECLARE @EarthRadiusKm DECIMAL = 6371.0;
    DECLARE @dLat DECIMAL(18, 15), @dLon DECIMAL(18, 15), @a DECIMAL(18, 15), @c DECIMAL(18, 15);

    SET @Latitude       = RADIANS(CAST(@Latitude AS FLOAT)); 
    SET @Longitude      = RADIANS(CAST(@Longitude AS FLOAT));
    SET @GeoLatitude    = RADIANS(CAST(@GeoLatitude AS FLOAT));
    SET @GeoLongitude   = RADIANS(CAST(@GeoLongitude AS FLOAT));

    SET @dLat = @GeoLatitude - @Latitude;
    SET @dLon = @GeoLongitude - @Longitude;

    SET @a = POWER(SIN(@dLat / 2), 2) + COS(@Latitude) * COS(@GeoLatitude) * POWER(SIN(@dLon / 2), 2);
    SET @c = 2 * ATN2(SQRT(@a), SQRT(1 - @a));

    -- distance in kilometers
    RETURN @EarthRadiusKm * @c;
END;
GO



--Update view
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
GO


--drop the old procedure because of renaming
DROP PROC [Application].[GetFilteredProfiles]
GO

-- Update procedure
CREATE OR ALTER PROC [Application].[usp_GetFilteredProfiles]
    @SchoolYear              INT			= NULL,
    @Grade                   INT			= NULL,
    @Area                    NVARCHAR(300)	= NULL,
    @Settlement              NVARCHAR(300)	= NULL,
    @Region                  NVARCHAR(300)	= NULL,
    @Neighbourhood           NVARCHAR(300)	= NULL,
    @Latitude                DECIMAL(17, 15) = NULL,
    @Longitude               DECIMAL(18, 15) = NULL,
    @Radius                  DECIMAL(6, 3)  = NULL,
    @ProfileType             NVARCHAR(200)	= NULL,
    @SpecialtyId             INT			= NULL,
    @ProfessionId            INT			= NULL,
    @ProfessionalDirectionId INT			= NULL,
    @ScienceId               INT			= NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
		ProfileId,
        ProfileName,
        ProfileType,
        Grade,
        StudyPeriod,
        InstitutionId,
		InstitutionFullName,
        GradingFormulas,
        StudyMethod,
        EducationType,
        ClassesCount,
        FirstForeignLanguage,
        SchoolYear,
        IsPaperOnly,
        ExternalId,
        QuotasTotal,
        QuotasMale,
        QuotasFemale,
		SpecialtyId,
        Specialty,
        SpecialtyExternalId,
        ProfessionalQualificationLevel,
        IsProtected,
        HasExpectedShortage,
		IsProfessional,
        SpecialtyDescription,
		ProfessionId,
        Profession,
        ProfessionExternalId,
		ProfessionalDirectionId,
        ProfessionalDirection,
        ProfessionalDirectionExternalId,
		ScienceId,
        Science,
        ScienceExternalId,
        Area,
        Settlement,
        Region,
        Neighbourhood,
        GeoLatitude,
        GeoLongitude
    FROM [Application].[v_ProfileDetails]
    WHERE
            (@SchoolYear IS NULL                OR SchoolYear = @SchoolYear)
        AND (@Grade IS NULL                     OR Grade = @Grade)
        AND (@ProfileType IS NULL               OR ProfileType = @ProfileType)
        AND (@SpecialtyId IS NULL               OR SpecialtyId = @SpecialtyId)
        AND (@ProfessionId IS NULL              OR ProfessionId = @ProfessionId)
        AND (@ProfessionalDirectionId IS NULL   OR ProfessionalDirectionId = @ProfessionalDirectionId)
        AND (@ScienceId IS NULL                 OR ScienceId = @ScienceId)
        AND (@Area IS NULL                      OR Area = @Area)
        AND (@Settlement IS NULL                OR Settlement = @Settlement)
        AND (@Region IS NULL                    OR Region = @Region)
        AND (@Neighbourhood IS NULL             OR Neighbourhood = @Neighbourhood)
        AND (
                @Latitude   IS NULL  OR
                @Longitude  IS NULL  OR
                ([Application].[fn_CalculateDistanceHaversine] (@Latitude, @Longitude, GeoLatitude, GeoLongitude) <= @Radius)
            );
END;
GO