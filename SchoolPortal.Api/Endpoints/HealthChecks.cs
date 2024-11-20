using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolPortal.Api.Services;

namespace SchoolPortal.Api.Endpoints
{
    public class HealthChecks : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/v1/ping", Ping);
            app.MapGet("/api/v1/ping/details", PingDetails);
        }

        public void MapServices(IServiceCollection services)
        {
            services.TryAddScoped<IApiHealthCheckRepository, ApiHealthCheckRepository>();
            services.AddHealthChecks();
        }

        internal async Task<IResult> Ping(IApiHealthCheckRepository repository, CancellationToken cancellationToken)
        {
            return await repository.Ping(cancellationToken);
        }

        internal async Task<IResult> PingDetails(IApiHealthCheckRepository repository, CancellationToken cancellationToken)
        {
            return await repository.PingDetails(cancellationToken);
        }
    }
}
