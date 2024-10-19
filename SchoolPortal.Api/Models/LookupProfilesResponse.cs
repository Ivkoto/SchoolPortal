namespace SchoolPortal.Api.Models
{
    public record LookupProfilesResponse
    {
        public int ProfileCount { get; set; } = 0;
        public List<ProfileModel> Profiles { get; set; } = [];
    }    
}
