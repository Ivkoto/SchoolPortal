namespace SchoolPortal.Api.Models
{
    public class SpecialtiesRoot
    {
        public List<SpecialtyModel> Specialties { get; set; }
    }

    public class SpecialtyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
