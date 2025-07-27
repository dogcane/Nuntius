# Nuntius EF Core Storage Layer

This project provides Entity Framework Core 9.x.x storage implementation for the Nuntius messaging system.

## Features

- Full implementation of all store interfaces:
  - `ISenderStore` - Storage for sender configurations
  - `IRendererStore` - Storage for renderer configurations  
  - `ITemplateStore` - Storage for message templates
  - `IDataFetcherStore` - Storage for data fetcher configurations
  - `IMessageStore` - Storage for messages

- Entity Framework Core 9.x.x compatible
- Support for SQL Server, In-Memory Database (for testing), and other EF Core providers
- Proper entity configurations with value conversions for complex types
- Dependency injection setup
- Full test coverage

## Usage

### Basic Setup with SQL Server

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF;

var services = new ServiceCollection();

// Add EF Core storage with SQL Server
services.AddNuntiusEFStorage("Server=localhost;Database=Nuntius;Trusted_Connection=true;");

var serviceProvider = services.BuildServiceProvider();
```

### Custom Configuration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF;

var services = new ServiceCollection();

// Add EF Core storage with custom configuration
services.AddNuntiusEFStorage(options => 
{
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors();
});

var serviceProvider = services.BuildServiceProvider();
```

### In-Memory Database (for Testing)

```csharp
using Microsoft.Extensions.DependencyInjection;
using Nuntios.Core.Storage.EF;

var services = new ServiceCollection();

// Add EF Core storage with in-memory database
services.AddNuntiusInMemoryStorage("TestDatabase");

var serviceProvider = services.BuildServiceProvider();
```

### Using the Stores

```csharp
// Get store instances
var senderStore = serviceProvider.GetRequiredService<ISenderStore>();
var rendererStore = serviceProvider.GetRequiredService<IRendererStore>();
var templateStore = serviceProvider.GetRequiredService<ITemplateStore>();

// Create a sender
var senderResult = Sender.Create("EMAIL_SENDER", "Email Sender", "SMTP", "{}", MessageType.Email);
if (senderResult.Success)
{
    var savedSender = await senderStore.SaveAsync(senderResult.Value!);
}

// Retrieve all senders
var allSenders = await senderStore.GetAllAsync();
var sendersList = await allSenders.ToListAsync();
```

## Database Schema

The storage layer creates the following tables:

- `Senders` - Sender configurations
- `Renderers` - Renderer configurations
- `Templates` - Message templates
- `DataFetchers` - Data fetcher configurations
- `Messages` - Messages

Complex types like `TemplateContext`, `MessageRecipients`, and `RenderedMessage` are stored as JSON in appropriate columns.

## Migrations

To create and apply migrations:

```bash
# Add a migration
dotnet ef migrations add InitialCreate --project Nuntios.Core.Storage.EF

# Update the database
dotnet ef database update --project Nuntios.Core.Storage.EF
```

## Testing

The project includes comprehensive tests that verify:

- Store implementations work correctly
- Entity configurations are properly set up
- Dependency injection registration works
- In-memory database functionality

Run tests with:

```bash
dotnet test
```