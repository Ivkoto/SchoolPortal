namespace SchoolPortal.Api.Models
{
    public record LookupSpecialtyResponse
    {
        public int SpecialtyCount { get; set; } = 0;
        public List<SpecialtyModel> Specialties { get; set; } = [];
    }

    public record SpecialtyModel
    (
        int Id,
        int ExternalId,
        string Name,
        bool IsProfessional,
        int ProfessionId
    );
}
