namespace SchoolPortal.Api.Endpoints;

/// <summary>
/// Represents a contract for defining API endpoint modules in the SchoolPortal application.
/// This interface follows the Vertical Slice Architecture pattern, allowing each endpoint module
/// to encapsulate both its service registrations and route mappings in a cohesive unit.
/// </summary>
/// <remarks>
/// <para>
/// The IEndpoint interface enables a modular approach to API development where each feature area
/// (such as Institutions, Profiles, Location) can be implemented as a self-contained endpoint module.
/// This promotes separation of concerns and makes the codebase more maintainable and testable.
/// </para>
/// <para>
/// Implementations are automatically discovered and registered during application startup through
/// the <see cref="Extensions.EndpointsExtensions.AddEndpoints"/> method, which scans assemblies
/// for classes implementing this interface.
/// </para>
/// </remarks>
public interface IEndpoint
{
    /// <summary>
    /// Registers services required by this endpoint module into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register dependencies into.</param>
    /// <remarks>
    /// This method is called during application startup and should register all services
    /// that the endpoint's methods depend on, such as:
    /// <list type="bullet">
    /// <item><description>Repository interfaces and implementations</description></item>
    /// <item><description>Validators for request models</description></item>
    /// <item><description>Any other services specific to this endpoint</description></item>
    /// </list>
    /// Use TryAdd* methods when possible to avoid duplicate registrations when multiple
    /// endpoints might register the same services.
    /// </remarks>
    void MapServices(IServiceCollection services);

    /// <summary>
    /// Configures the HTTP routes and endpoints for this module.
    /// </summary>
    /// <param name="app">The web application builder to configure routes on.</param>
    /// <remarks>
    /// This method is called during application startup and should define all HTTP routes
    /// handled by this endpoint module. Each route should typically:
    /// <list type="bullet">
    /// <item><description>Specify the HTTP method and route pattern</description></item>
    /// <item><description>Assign a unique name using WithName()</description></item>
    /// <item><description>Define expected response types using Produces&lt;T&gt;()</description></item>
    /// <item><description>Apply CORS policies using RequireCors()</description></item>
    /// </list>
    /// Routes should follow the API versioning pattern: /api/v{version}/{resource}
    /// </remarks>
    void MapEndpoints(WebApplication app);
}
