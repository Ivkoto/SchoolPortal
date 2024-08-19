namespace SchoolPortal.Api.Endpoints
{
    public interface IEndpoint
    {
        void MapServices(IServiceCollection services);

        void MapEndpoints(WebApplication app);
    }
}
