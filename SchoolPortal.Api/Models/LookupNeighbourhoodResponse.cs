namespace SchoolPortal.Api.Models
{
    public record LookupNeighbourhoodResponse
    {
        public int NeighbourhoodCount { get; set; } = 0;

        public List<NeighbourhoodModel> Neighbourhood { get; set;} = [];
    }

    public record NeighbourhoodModel(string Neighbourhood);
}
