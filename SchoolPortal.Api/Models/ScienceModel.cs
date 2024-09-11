namespace SchoolPortal.Api.Models
{
    public class SciencesRoot
    {
        public List<ScienceModel> Sciences { get; set; }
    }

    public class ScienceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
