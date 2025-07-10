# Contributing to SchoolPortal API

Thank you for considering contributing to SchoolPortal API! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

This project is governed by the Devocean Solutions code of conduct. All contributors are expected to uphold this code.

## Development Process

### Local Development Setup

#### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

#### Getting Started

1. Clone the repository
2. Configure the database connection in `appsettings.Development.json`
3. Run database deployment: `dotnet run --project SchoolPortal.Database.Deploy`
4. Start the API:
   - **Visual Studio**: Press F5 (defaults to HTTPS)
   - **Command Line HTTP**: `dotnet run --project SchoolPortal.Api`
   - **Command Line HTTPS**: `dotnet run --project SchoolPortal.Api --urls=https://localhost:7154`

#### API Documentation

- **Swagger UI**: Available at `/swagger` endpoint
- **Local URLs**:
  - HTTP: http://localhost:5141/swagger
  - HTTPS: https://localhost:7154/swagger
- **Development Certificate**: Run `dotnet dev-certs https --trust` if needed for HTTPS

#### Troubleshooting

- **Swagger rendering issues**: Clear browser cache, especially in Microsoft Edge
- **HTTPS certificate errors**: Ensure development certificate is trusted
- **Database connection**: Verify connection string and SQL Server availability

### Branching Strategy

- `main`: Production-ready code
- `feature/*`: Individual feature branches
- `db/*`: Database realated changes branch
- `bug/*`: Bug fix branches
- `fix/*`: Small fixes and refactoring branches

### Pull Request Process

1. Create a new branch from `main`
2. Implement your changes
3. Write or update tests
4. Update documentation as needed
5. Submit a pull request in Azure
6. Address any code review feedback

## Coding Standards

### C# Code Style

- Follow Microsoft's C# coding conventions: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- Use meaningful variable and method names _(e.g. use "userCount" instead of "x", use "calculateTotal()" instead of "doIt()")_
- Add XML documentation comments for public APIs
- Limit line length to 120 characters

### Database

- Use stored procedures for database operations
- Include documentation comments for stored procedures
- Follow consistent naming conventions for database objects

### Testing

All code contributions should include appropriate tests:

- **Unit tests** for individual components
- **Integration tests** for API endpoints
- Aim for high test coverage, especially for business logic

## Documentation

Update documentation when making changes:

- Update API.md when modifying endpoints
- Update CONFIGURATION.md when changing configuration options
- Update README.md for significant features or changes
- Include code comments explaining complex logic

## Commit Guidelines

- Use clear, descriptive commit messages
- Reference issue numbers in commit messages
- Keep commits focused on a single change
- Use present tense ("Add feature" not "Added feature")

## Versioning

We follow semantic versioning (see https://semver.org for details):

- MAJOR version for incompatible API changes
- MINOR version for new functionality in a backward compatible manner
- PATCH version for backward compatible bug fixes

## Getting Help

If you need help with your contribution, you can:

- Reach out to the project maintainers
- Create a draft pull request and ask for early feedback
- Comment on the issue you're working on

## License

By contributing to this project, you agree that your contributions will be licensed under the [project's license](LICENSE.md).
