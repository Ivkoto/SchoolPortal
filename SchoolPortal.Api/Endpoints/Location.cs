using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.Api.Endpoints
{
    public class Location : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/location/neighbourhoods/{settlement}", GetNeighbourhoods)
                .WithName("GetNeighbourhoods")
                .Produces<GetNeighbourhoodsResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<ILocationRepository, LocationRepository>();
        }

        internal async Task<IResult> GetNeighbourhoods(
            string settlement,
            [FromServices] ILocationRepository service)
        {
            var neighbourhoods = await service.GetNeighbourhoodsBySettlement(settlement);

            return Results.Ok(
                new GetNeighbourhoodsResponse
                {
                    NeighbourhoodsCount = neighbourhoods.Count,
                    Neighbourhoods = neighbourhoods
                }
            );
        }
    }
}
