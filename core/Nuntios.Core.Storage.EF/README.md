# Nuntius.Core.Storage.EF

Entity Framework Core storage implementation for the Nuntius messaging framework.

## Overview

This package provides Entity Framework Core implementations for all Nuntius storage interfaces:

- `ISenderStore` - Store for managing message senders
- `IRendererStore` - Store for managing content renderers  
- `ITemplateStore` - Store for managing message templates
- `IDataFetcherStore` - Store for managing data fetchers
- `IMessageStore` - Store for managing messages (currently empty interface)

## Supported Database Providers

- **SQL Server** - via `UseSqlServer()`
- **In-Memory** - via `UseInMemoryDatabase()` (primarily for testing)
- **Any EF Core supported provider** - via custom configuration

## Usage

### Basic Setup with SQL Server

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF.Extensions;

// Add EF Core storage with SQL Server
services.AddNuntiusEfStorageSqlServer("Server=localhost;Database=NuntiusDb;Trusted_Connection=true;");
```

### Basic Setup with In-Memory Database

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF.Extensions;

// Add EF Core storage with in-memory database (for testing)
services.AddNuntiusEfStorageInMemory();
```

### Custom Database Provider Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF.Extensions;

// Add EF Core storage with custom configuration
services.AddNuntiusEfStorage(options =>
{
    options.UseNpgsql(connectionString); // PostgreSQL
    // or any other EF Core supported provider
});
```

### Using the Stores

```csharp
// Inject the stores you need
public class MyService
{
    private readonly ISenderStore _senderStore;
    private readonly ITemplateStore _templateStore;

    public MyService(ISenderStore senderStore, ITemplateStore templateStore)
    {
        _senderStore = senderStore;
        _templateStore = templateStore;
    }

    public async Task Example()
    {
        // Create a sender
        var senderResult = Sender.Create("email-sender", "Email Sender", "smtp", "{\"host\":\"smtp.gmail.com\"}", MessageType.Email);
        if (senderResult.Success)
        {
            await _senderStore.SaveAsync(senderResult.Value);
        }

        // Get all senders
        var sendersQuery = await _senderStore.GetAllAsync();
        var senders = await sendersQuery.ToListAsync();

        // Get a specific sender
        var sender = await _senderStore.GetByIdAsync("email-sender");
    }
}
```

## Database Schema

The EF Core implementation creates the following tables:

- **Senders** - Message senders with engine configuration
- **Renderers** - Content renderers with engine configuration  
- **Templates** - Message templates with content and context
- **DataFetchers** - Data fetchers with engine configuration

All entities include:
- `Id` (string, Primary Key)
- `Name` (string, Unique Index)
- `EngineId` (string, Index)
- `Status` (int) - Enabled/Disabled/Archived
- Entity-specific properties

## Entity Configurations

The package includes proper EF Core configurations for:

- Value conversions for `MessageType` and `ElementStatus` enums
- Unique constraints on entity names
- Indexes on commonly queried fields
- Owned entity configuration for `TemplateContext`
- Proper column types and lengths

## Migrations

To create and apply database migrations:

```bash
# Add migration
dotnet ef migrations add InitialCreate --project Nuntios.Core.Storage.EF

# Update database
dotnet ef database update --project Nuntios.Core.Storage.EF
```

## Testing

The package includes comprehensive tests covering:

- CRUD operations for all store implementations
- Dependency injection registration
- Entity mapping and conversions
- In-memory database scenarios

Run tests with:
```bash
dotnet test --filter "Storage"
```

## Dependencies

- .NET 8.0
- Entity Framework Core 8.0.x
- Nuntius.Core