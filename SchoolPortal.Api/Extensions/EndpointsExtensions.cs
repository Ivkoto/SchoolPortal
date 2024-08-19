using SchoolPortal.Api.Endpoints;

namespace SchoolPortal.Api.Extensions
{
    public static class EndpointsExtensions
    {
        public static void AddEndpoints(this IServiceCollection services, params Type[] scanMarkers)
        {
            var endpointDefinitions = new List<IEndpoint>();

            foreach (var marker in scanMarkers)
            {
                endpointDefinitions.AddRange(
                    marker.Assembly.ExportedTypes
                        .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                        .Select(Activator.CreateInstance).Cast<IEndpoint>()
                );
            }

            foreach (var endpointDefinition in endpointDefinitions)
            {
                //TODO @IvayloK check for the serviecs duplication
                endpointDefinition.MapServices(services);
            }

            //try use service provider instead
            //services.AddSingleton(sp => Activator.CreateInstance(sp, asds))

            services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpoint>);
        }

        public static void UseEndpoints(this WebApplication app)
        {
            var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpoint>>();

            foreach (var endpointDefinition in definitions)
            {
                endpointDefinition.MapEndpoints(app);
            }
        }
    }
}
