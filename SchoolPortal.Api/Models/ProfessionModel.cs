namespace SchoolPortal.Api.Models
{
    public class ProfessionModelRoot
    {
        public List<ProfessionModel> Professions { get; set; }
    }
    public class ProfessionModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
