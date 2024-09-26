namespace SchoolPortal.Api.Models
{
    public record LookupProfilesRequest
    {
        public int? SchoolYear { get; set; }
        public int? Grade { get; set; }
        public string? Area { get; set; }
        public string? Settlement { get; set; }
        public string? Region { get; set; }
        public string? Neighbourhood { get; set; }
        public GeoLocationRequest? GeoLocationFilter { get; set; }
        public string? ProfileType { get; set; }
        public int? SpecialtyId { get; set; }
        public int? ProfessionId { get; set; }
        public int? ProfessionalDirectionId { get; set; }
        public int? ScienceId { get; set; }
    }

    public record GeoLocationRequest
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Radius { get; set; }
    }
}