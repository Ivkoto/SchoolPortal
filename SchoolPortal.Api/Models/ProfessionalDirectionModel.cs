namespace SchoolPortal.Api.Models
{
    public class ProfessionalDirectionsRoot
    {
        public List<ProfessionalDirectionModel> ProfessionalDirections { get; set; }
    }

    public class ProfessionalDirectionModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
