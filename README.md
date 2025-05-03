# Nezam.Refahi.Backend

This is the backend solution for the Nezam.Refahi project.

## Solution Structure

The solution is organized using a clean architecture approach:

- **src/** - Contains all source code projects
  - **Nezam.Refahi.Domain/** - Contains domain entities, value objects, and domain logic
  
- **test/** - Contains all test projects
  - **Nezam.Refahi.Domain.Tests/** - Tests for the domain project

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or later / Visual Studio Code with C# extensions

### Building the Solution

```bash
dotnet restore
dotnet build
```

### Running Tests

```bash
dotnet test
```

## Development Guidelines

- Follow Domain-Driven Design principles
- Write unit tests for all domain logic
- Keep the domain model clean and focused on business rules
- Use value objects for concepts with no identity
- Implement rich domain models with behavior
