namespace SchoolPortal.Api.Models
{
    public record GetFilteredProfilesResponse
    {
        public int ProfilesCount { get; set; } = 0;
        public List<ProfileModel> Profiles { get; set; } = [];
    }    
}
