namespace SchoolPortal.Api.Models;

public record GetProfessionalDirectionsResponse
{
    public int ProfessionalDirectionsCount { get; set; } = 0;
    public IReadOnlyCollection<ProfessionalDirectionModel> ProfessionalDirections { get; set; } = [];
}

public record ProfessionalDirectionModel
(
    int Id,
    int ExternalId,
    string Name,
    int ScienceId
);
