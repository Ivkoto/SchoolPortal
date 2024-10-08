-- get specialties view
CREATE OR ALTER VIEW [Application].[v_Specialties] AS
SELECT
	[Id],
	[ExternalId],
	[Name],
	[IsProfessional],
	[ProfessionId]
FROM
	[Application].[Specialty]
GO


-- get Specialties by Professional Direction Id as optional PROC
CREATE OR ALTER PROC [Application].[usp_SpecialtiesByProfessionId]
	@ProfessionId	INT	= NULL,
	@IsProfessional	INT
AS
BEGIN
    SET NOCOUNT ON;

	SELECT
		[Id],
		[ExternalId],
		[Name],
		[IsProfessional],
		[ProfessionId]
	FROM
		[Application].[v_Specialties]
	WHERE
		(@ProfessionId	 IS NULL OR [ProfessionId] = @ProfessionId)
		AND [IsProfessional] = @IsProfessional;
END;
GO


-- Update filteres profiles PROC
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
    @IsProfessional          INT			= NULL,
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
        AND (@IsProfessional IS NULL            OR IsProfessional = @IsProfessional)
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