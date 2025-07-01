namespace SchoolPortal.Api.Models;

public record GetNeighbourhoodsResponse
{
    public int NeighbourhoodsCount { get; set; } = 0;

    public IReadOnlyCollection<NeighbourhoodModel> Neighbourhoods { get; set;} = [];
}

public record NeighbourhoodModel(string Neighbourhood);
