using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguration();
builder.Services.AddLogger();
builder.Services.AddEndpoints(typeof(IEndpoint));
builder.Services.AddSingleton<IDbConnectionFactory>(
    _ => new DbConnectionFactory(builder.Configuration.GetConnectionString("DatabaseConnection")!));

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseEndpoints();

app.MapGet("/", async context =>
{
    var htmlFilePath = Path.Combine(builder.Environment.WebRootPath, "index.html");
    var htmlContent = await File.ReadAllTextAsync(htmlFilePath);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlContent);
});

app.Run();
