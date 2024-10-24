﻿using SchoolPortal.Api.Services;

namespace SchoolPortal.Api.Endpoints
{
    public class HealthChecks : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/ping", Ping);
            app.MapGet("/ping/details", PingDetails);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<IApiHealthCheckRepository, ApiHealthCheckRepository>();
            services.AddHealthChecks();
        }

        internal async Task<IResult> Ping(IApiHealthCheckRepository service, CancellationToken cancellationToken)
        {
            return await service.Ping(cancellationToken);
        }

        internal async Task<IResult> PingDetails(IApiHealthCheckRepository service, CancellationToken cancellationToken)
        {
            return await service.PingDetails(cancellationToken);
        }
    }
}
