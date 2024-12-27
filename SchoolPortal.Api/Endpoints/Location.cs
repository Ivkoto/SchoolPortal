using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.Api.Endpoints;

public class Location : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/v1/location/neighbourhoods/{settlement}", GetNeighbourhoods)
            .WithName("GetNeighbourhoods")
            .Produces<GetNeighbourhoodsResponse>(StatusCodes.Status200OK)
            .RequireCors("AllowedOriginsPolicy");
    }

    public void MapServices(IServiceCollection services)
    {
        services.TryAddScoped<ILocationRepository, LocationRepository>();
    }

    internal async Task<IResult> GetNeighbourhoods(
        string settlement,
        HttpContext httpContext,
        [FromServices] ILocationRepository locationRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var neighbourhoods = await locationRepository.GetNeighbourhoodsBySettlement(settlement);

        return Results.Ok(
            new GetNeighbourhoodsResponse
            {
                NeighbourhoodsCount = neighbourhoods.Count,
                Neighbourhoods = neighbourhoods
            }
        );
    }
}
