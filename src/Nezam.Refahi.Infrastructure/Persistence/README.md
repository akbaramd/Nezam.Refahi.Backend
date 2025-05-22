# SQLite Database and Migrations Guide

This guide explains how to work with the SQLite database and manage migrations in the Nezam.Refahi.Backend project.

## Creating a Migration

To create a new migration, run the following command from the root directory of the project:

```bash
dotnet ef migrations add <MigrationName> --project src/Nezam.Refahi.Infrastructure --startup-project <path-to-startup-project>
```

For example:

```bash
dotnet ef migrations add InitialCreate --project src/Nezam.Refahi.Infrastructure --startup-project src/Nezam.Refahi.Api
```

## Applying Migrations

Migrations are automatically applied when the application starts, but you can also apply them manually using the following command:

```bash
dotnet ef database update --project src/Nezam.Refahi.Infrastructure --startup-project <path-to-startup-project>
```

## Database Initialization

The database is automatically initialized when the application starts. The initialization process includes:

1. Creating the database if it doesn't exist
2. Applying any pending migrations
3. Seeding initial data if needed

## SQLite Connection String

The default SQLite connection string is:

```
Data Source=nezam_refahi.db
```

You can customize this by setting the `DefaultConnection` connection string in your `appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=custom_database.db"
  }
}
```

## Using the Database in Code

The database context is registered in the DI container and can be injected into your services:

```csharp
public class MyService
{
    private readonly ApplicationDbContext _dbContext;

    public MyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DoSomethingAsync()
    {
        // Use the database context
        var users = await _dbContext.Users.ToListAsync();
    }
}
```

## Testing with In-Memory Database

For testing, you can use an in-memory SQLite database:

```csharp
// In your test setup
services.AddInMemoryDatabase();
```

This will create an in-memory SQLite database that is isolated for each test.
