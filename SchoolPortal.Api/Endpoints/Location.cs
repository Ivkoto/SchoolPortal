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
                .Produces<LookupNeighbourhoodResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<ILocationRepository, LocationRepository>();
        }

        private async Task<IResult> GetNeighbourhoods(
            string settlement, [FromServices] ILocationRepository service)
        {
            var neighbourhoods = await service.GetNeighbourhoodsBySettlement(settlement);

            return Results.Ok(new LookupNeighbourhoodResponse
            {
                NeighbourhoodCount = neighbourhoods.Count,
                Neighbourhood = neighbourhoods
            });
        }
    }
}
