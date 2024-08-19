namespace SchoolPortal.Api.Models
{
    public class ProfilesRoot
    {
        public List<ProfileModel> Profiles { get; set; }
    }
        
    public class ProfileModel
    {
        public int ProfileId { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public int StudyPeriod { get; set; }
        public string GradingFormula { get; set; }
        public string StudyMethod { get; set; }
        public string EducationType { get; set; }
        public double ClassesCount { get; set; }
        public string FirstForeignLanguage { get; set; }
        public string Specialty { get; set; }
        public int ProfessionalQualificationLevel { get; set; }
        public bool IsProtected { get; set; }
        public bool HasExpectedShortage { get; set; }
        public string Description { get; set; }
        public string Profession { get; set; }
        public string ProfessionalDirection { get; set; }
        public string Science { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; } 
    }
}
