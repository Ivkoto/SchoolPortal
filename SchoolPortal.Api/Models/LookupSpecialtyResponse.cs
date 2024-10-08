namespace SchoolPortal.Api.Models
{
    public record LookupSpecialtyResponse
    {
        public int SpecialtyCount { get; set; } = 0;
        public List<SpecialtyModel> Specialties { get; set; } = [];
    }

    public record SpecialtyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
