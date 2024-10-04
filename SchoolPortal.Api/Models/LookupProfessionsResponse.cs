namespace SchoolPortal.Api.Models
{
    public record LookupProfessionsResponse
    {
        public int ProfessionCount { get; set; } = 0;
        public List<ProfessionModel> Professions { get; set; } = [];
    }
    public record ProfessionModel
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public int ProfessionalDirectionId { get; set; }
    }
}
