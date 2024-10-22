namespace SchoolPortal.Api.Models
{
    public record GetFilteredProfilesRequest
    (
        int SchoolYear,
        int Grade,
        string? Settlement,
        string? Neighbourhood,
        GeoLocationModel? GeoLocationFilter,
        string? ProfileType,
        int? SpecialtyId,
        int? ProfessionId,
        int? ProfessionalDirectionId,
        int? ScienceId
    );

    public record GeoLocationModel
    (
        decimal? Latitude,
        decimal? Longitude,
        decimal? Radius
    );
}