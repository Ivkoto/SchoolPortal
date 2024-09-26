namespace SchoolPortal.Api.Models
{
    public record LookupProfilesResponse
    {
        public int ProfileCount { get; set; } = 0;
        public List<ProfileModel> Profiles { get; set; } = [];
    }

    public record ProfileModel
    {
        public required int ProfileId { get; set; }
        public required string ProfileName { get; set; }
        public string? ProfileType { get; set; }
        public int? Grade { get; set; }
        public string? StudyPeriod { get; set; }
        public required int InstitutionId { get; set; }
        public required string InstitutionFullName { get; set; }
        public string? GradingFormulas { get; set; }
        public string? StudyMethod { get; set; }
        public string? EducationType { get; set; }
        public decimal? ClassesCount { get; set; }
        public string? FirstForeignLanguage { get; set; }
        public int SchoolYear { get; set; }
        public bool IsPaperOnly { get; set; }
        public int? ExternalId { get; set; }
        public int? QuotasTotal { get; set; }
        public int? QuotasMale { get; set; }
        public int? QuotasFemale { get; set; }
        public int? SpecialtyId { get; set; }
        public string? Specialty { get; set; }
        public int? SpecialtyExternalId { get; set; }
        public int? ProfessionalQualificationLevel { get; set; }
        public bool IsProtected { get; set; }
        public bool HasExpectedShortage { get; set; }
        public bool IsProfessional { get; set; }
        public string? SpecialtyDescription { get; set; }
        public int? ProfessionId { get; set; }
        public string? Profession { get; set; }
        public int? ProfessionExternalId { get; set; }
        public int? ProfessionalDirectionId { get; set; }
        public string? ProfessionalDirection { get; set; }
        public int? ProfessionalDirectionExternalId { get; set; }
        public int? ScienceId { get; set; }
        public string? Science { get; set; }
        public int? ScienceExternalId { get; set; }
    }
}
