namespace SchoolPortal.Api.Models
{
    public record GetSpecialtiesResponse
    {
        public int SpecialtesCount { get; set; } = 0;
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
