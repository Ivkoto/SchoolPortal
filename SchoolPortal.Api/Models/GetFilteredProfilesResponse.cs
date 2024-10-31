namespace SchoolPortal.Api.Models
{
    public record GetFilteredProfilesResponse
    {
        public int ProfilesCount { get; set; } = 0;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<ProfileModel> Profiles { get; set; } = [];
    }
}
