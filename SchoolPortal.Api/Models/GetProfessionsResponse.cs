namespace SchoolPortal.Api.Models
{
    public record GetProfessionsResponse
    {
        public int ProfessionsCount { get; set; } = 0;
        public List<ProfessionModel> Professions { get; set; } = [];
    }
    public record ProfessionModel
    (
        int Id,
        int ExternalId,
        string Name,
        int ProfessionalDirectionId
    );
}
