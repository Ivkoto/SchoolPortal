CREATE OR ALTER PROCEDURE [Application].[GetFilteredProfiles]
    @SchoolYear INT = NULL,
    @Grade INT = NULL,
    @SpecialtyId INT = NULL,
    @ProfessionId INT = NULL,
    @ProfessionalDirectionId INT = NULL,
    @ScienceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
		ProfileId,
        ProfileName,
        Type,
        Grade,
        StudyPeriod,
        InstitutionId,
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
        SpecialtyDescription,
		ProfessionId,
        Profession,
        ProfessionExternalId,
		ProfessionalDirectionId,
        ProfessionalDirection,
        ProfessionalDirectionExternalId,
		ScienceId,
        Science,
        ScienceExternalId
    FROM [Application].[v_ProfileDetails]
    WHERE
        (@SchoolYear IS NULL OR SchoolYear = @SchoolYear) AND
        (@Grade IS NULL OR Grade = @Grade) AND
        (@SpecialtyId IS NULL OR SpecialtyId = @SpecialtyId) AND
        (@ProfessionId IS NULL OR ProfessionId = @ProfessionId) AND
        (@ProfessionalDirectionId IS NULL OR ProfessionalDirectionId = @ProfessionalDirectionId) AND
        (@ScienceId IS NULL OR ScienceId = @ScienceId)
END;