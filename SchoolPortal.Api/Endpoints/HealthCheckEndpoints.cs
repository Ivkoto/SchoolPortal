using SchoolPortal.Api.Services;

namespace SchoolPortal.Api.Endpoints
{
    public class HealthCheckEndpoints : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/ping", Ping);
            app.MapGet("/ping/details", PingDetails);
        }

        internal async Task Ping(IApiHealthCheckRepository service)
        {
            await service.Ping();
        }

        internal async Task<IResult> PingDetails(IApiHealthCheckRepository service, CancellationToken cancellationToken)
        {
            return await service.PingDetails(cancellationToken);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<IApiHealthCheckRepository, ApiHealthCheckRepository>();
            services.AddHealthChecks();
        }
    }
}
