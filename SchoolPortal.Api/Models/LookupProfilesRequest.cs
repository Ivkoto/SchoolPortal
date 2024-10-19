namespace SchoolPortal.Api.Models
{
    public record LookupProfilesRequest
    (
        int SchoolYear,
        int? Grade,
        string? Settlement,
        string? Neighbourhood,
        GeoLocationRequest? GeoLocationFilter,
        string? ProfileType,
        int? SpecialtyId,
        int? ProfessionId,
        int? ProfessionalDirectionId,
        int? ScienceId
    );

    public record GeoLocationRequest
    (
        decimal? Latitude,
        decimal? Longitude,
        decimal? Radius
    );
}