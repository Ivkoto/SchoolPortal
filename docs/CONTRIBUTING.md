<!-- Release v1.2.0: Minor updates to contributing guidelines -->

# Contributing to SchoolPortal API

Thank you for considering contributing to SchoolPortal API! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

This project is governed by the Devocean Solutions code of conduct. All contributors are expected to uphold this code.

## Development Process

### Branching Strategy

- `main`: Production-ready code
- `develop`: Integration branch for features
- `feature/*`: Individual feature branches
- `bugfix/*`: Bug fix branches
- `release/*`: Release preparation branches

### Pull Request Process

1. Create a feature or bugfix branch from `develop`
2. Implement your changes
3. Write or update tests
4. Update documentation as needed
5. Submit a pull request to `develop`
6. Address any code review feedback

## Coding Standards

### C# Code Style

- Follow Microsoft's C# coding conventions
- Use meaningful variable and method names
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

We follow semantic versioning:

- MAJOR version for incompatible API changes
- MINOR version for new functionality in a backward compatible manner
- PATCH version for backward compatible bug fixes

## Getting Help

If you need help with your contribution, you can:

- Reach out to the project maintainers
- Create a draft pull request and ask for early feedback
- Comment on the issue you're working on

## License

By contributing to this project, you agree that your contributions will be licensed under the project's license.
