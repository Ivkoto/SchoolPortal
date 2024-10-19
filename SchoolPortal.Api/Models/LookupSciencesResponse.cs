namespace SchoolPortal.Api.Models
{
    public record LookupSciencesResponse
    {
        public int ScienceCount { get; set; } = 0;
        public List<ScienceModel> Sciences { get; set; } = [];
    }

    public record ScienceModel
    (
        int Id,
        int ExternalId,
        string Name
    );
}
