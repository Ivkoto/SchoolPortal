CREATE OR ALTER PROC [Application].[usp_GetFilteredProfiles]
    @SchoolYear              INT,
    @Grade                   INT,
    @Settlement              NVARCHAR(300)    = NULL,
    @Neighbourhood           NVARCHAR(300)    = NULL,
    @Latitude                DECIMAL(17, 15)  = NULL,
    @Longitude               DECIMAL(18, 15)  = NULL,
    @Radius                  DECIMAL(6, 3)    = NULL,
    @IsProfessional          INT              = NULL,
    @SpecialtyId             INT              = NULL,
    @ProfessionId            INT              = NULL,
    @ProfessionalDirectionId INT              = NULL,
    @ScienceId               INT              = NULL,
    @InstitutionId           INT              = NULL,
    @PageNumber              INT              = NULL,
    @PageSize                INT              = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- total items
    DECLARE @TotalItems INT;
    SELECT @TotalItems = COUNT(*)
    FROM [Application].[uv_ProfileDetails]
    WHERE
            (SchoolYear = @SchoolYear)
        AND (Grade = @Grade)
        AND (@IsProfessional IS NULL            OR IsProfessional = @IsProfessional)
        AND (@SpecialtyId IS NULL               OR SpecialtyId = @SpecialtyId)
        AND (@ProfessionId IS NULL              OR ProfessionId = @ProfessionId)
        AND (@ProfessionalDirectionId IS NULL   OR ProfessionalDirectionId = @ProfessionalDirectionId)
        AND (@ScienceId IS NULL                 OR ScienceId = @ScienceId)
        AND (@Settlement IS NULL                OR Settlement = @Settlement)
        AND (@Neighbourhood IS NULL             OR Neighbourhood = @Neighbourhood)
        AND (
                @Latitude   IS NULL  OR
                @Longitude  IS NULL  OR
                ([Application].[ufn_CalculateDistanceHaversine] (@Latitude, @Longitude, GeoLatitude, GeoLongitude) <= @Radius)
            )
        AND (@InstitutionId IS NULL             OR InstitutionId = @InstitutionId);

    -- total pages
    DECLARE @TotalPages INT = 
		CASE 
          WHEN @PageSize IS NOT NULL AND @PageSize > 0 
          THEN CEILING(@TotalItems * 1.0 / @PageSize) 
          ELSE 1 
        END;

    -- pagination offset
    DECLARE @Offset INT = 
        CASE 
            WHEN @PageNumber IS NOT NULL AND @PageSize IS NOT NULL 
            THEN (@PageNumber - 1) * @PageSize 
            ELSE 0 
        END;

    -- paginated rows select
    SELECT 
        ProfileId,
        ProfileName,
        ProfileType,
        Grade,
        StudyPeriod,
        InstitutionId,
        InstitutionFullName,
        InstitutionShortName,
        GradingFormulas,
        StudyMethod,
        EducationType,
        ClassesCount,
        FirstForeignLanguage,
        SchoolYear,
        IsPaperOnly,
		IsClosed,
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
    FROM [Application].[uv_ProfileDetails]
    WHERE
            (SchoolYear = @SchoolYear)
        AND (Grade		= @Grade)
        AND (@IsProfessional IS NULL            OR IsProfessional = @IsProfessional)
        AND (@SpecialtyId IS NULL               OR SpecialtyId = @SpecialtyId)
        AND (@ProfessionId IS NULL              OR ProfessionId = @ProfessionId)
        AND (@ProfessionalDirectionId IS NULL   OR ProfessionalDirectionId = @ProfessionalDirectionId)
        AND (@ScienceId IS NULL                 OR ScienceId = @ScienceId)
        AND (@Settlement IS NULL                OR Settlement = @Settlement)
        AND (@Neighbourhood IS NULL             OR Neighbourhood = @Neighbourhood)
        AND (
                @Latitude   IS NULL  OR
                @Longitude  IS NULL  OR
                ([Application].[ufn_CalculateDistanceHaversine] (@Latitude, @Longitude, GeoLatitude, GeoLongitude) <= @Radius)
            )
        AND (@InstitutionId IS NULL             OR InstitutionId = @InstitutionId)
    ORDER BY ProfileId
    OFFSET @Offset ROWS
    FETCH NEXT CASE WHEN @PageSize IS NOT NULL THEN @PageSize ELSE 1000000 END ROWS ONLY;

    -- total pages
    SELECT @TotalPages AS TotalPages;
END;
GO
