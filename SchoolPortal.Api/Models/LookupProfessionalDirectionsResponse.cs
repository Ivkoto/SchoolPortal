namespace SchoolPortal.Api.Models
{
    public record LookupProfessionalDirectionsResponse
    {
        public int ProfessionalDirectionCount { get; set; } = 0;
        public List<ProfessionalDirectionModel> ProfessionalDirections { get; set; } = [];
    }

    public record ProfessionalDirectionModel
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public int ScienceId { get; set; }
    }
}
