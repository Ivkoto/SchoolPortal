namespace SchoolPortal.Api.Models
{
    public record GetNeighbourhoodsResponse
    {
        public int NeighbourhoodsCount { get; set; } = 0;

        public List<NeighbourhoodModel> Neighbourhoods { get; set;} = [];
    }

    public record NeighbourhoodModel(string Neighbourhood);
}
