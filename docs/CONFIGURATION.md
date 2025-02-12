## Configuration
## <span style="color: red; font-weight: bold;">OUT OF DATE. NEEDS UPDATING AND SUPPLEMENTATION. This is planned in a future task.</span> ##
### Services Configuration

- **AddConfiguration()**: Configures the necessary application settings.
- **AddLogger()**: Sets up logging for the application.
- **AddEndpoints(typeof(IEndpoint))**: Registers all endpoint classes implementing the `IEndpoint` interface.

### Middleware

- **app.UseEndpoints()**: Registers all configured endpoints for the application.
- **Default Route**: The API serves an `index.html` file as the default route.

## Additional Information

- **Swagger UI**: Swagger is enabled for interactive API documentation.
- **Static Files**: The application serves static files from the `wwwroot` directory.