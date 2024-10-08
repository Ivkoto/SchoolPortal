using Microsoft.OpenApi.Models;

namespace SchoolPortal.Api.Endpoints
{
    public class Swagger : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPortal API v1"))
               .UseHttpsRedirection()
               .UseStaticFiles();
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolPortal API", Version = "v1" });
                s.EnableAnnotations();
            });
        }
    }
}
