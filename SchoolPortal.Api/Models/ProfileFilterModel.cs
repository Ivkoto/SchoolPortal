namespace SchoolPortal.Api.Models
{
    public class ProfileFilterModel
    {
        public int? SchoolYear { get; set; }
        public int? Grade { get; set; }
        public int? SpecialtyId { get; set; }
        public int?  ProfessionId { get; set; }
        public int? ProfessionalDirectionId { get; set; }
        public int? ScienceId { get; set; }
    }
}