namespace SchoolPortal.Api.Models
{
    public record LookupSciencesResponse
    {
        public int ScienceCount { get; set; } = 0;
        public List<ScienceModel> Sciences { get; set; } = [];
    }

    public record ScienceModel
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; }
    }
}
