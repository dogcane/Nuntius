using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Fetching.Infrastructure;
using Nuntius.Core.Messages.Infrastructure;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntius.Core.Templates.Infrastructure;
using Nuntios.Core.Storage.EF.Stores;

namespace Nuntios.Core.Storage.EF;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework storage services to the DI container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="connectionString">The database connection string</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddNuntiusEFStorage(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        // Add DbContext
        services.AddDbContext<NuntiusDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register store implementations
        services.AddScoped<ISenderStore, SenderStore>();
        services.AddScoped<IRendererStore, RendererStore>();
        services.AddScoped<ITemplateStore, TemplateStore>();
        services.AddScoped<IDataFetcherStore, DataFetcherStore>();
        services.AddScoped<IMessageStore, MessageStore>();

        return services;
    }

    /// <summary>
    /// Adds Entity Framework storage services with a custom DbContext configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureContext">Action to configure the DbContext</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddNuntiusEFStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> configureContext)
    {
        if (configureContext == null)
            throw new ArgumentNullException(nameof(configureContext));

        // Add DbContext with custom configuration
        services.AddDbContext<NuntiusDbContext>(configureContext);

        // Register store implementations
        services.AddScoped<ISenderStore, SenderStore>();
        services.AddScoped<IRendererStore, RendererStore>();
        services.AddScoped<ITemplateStore, TemplateStore>();
        services.AddScoped<IDataFetcherStore, DataFetcherStore>();
        services.AddScoped<IMessageStore, MessageStore>();

        return services;
    }

    /// <summary>
    /// Adds Entity Framework storage services with in-memory database (useful for testing)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="databaseName">The in-memory database name</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddNuntiusInMemoryStorage(this IServiceCollection services, string databaseName = "NuntiusTestDb")
    {
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentNullException(nameof(databaseName));

        // Add DbContext with in-memory database
        services.AddDbContext<NuntiusDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        // Register store implementations
        services.AddScoped<ISenderStore, SenderStore>();
        services.AddScoped<IRendererStore, RendererStore>();
        services.AddScoped<ITemplateStore, TemplateStore>();
        services.AddScoped<IDataFetcherStore, DataFetcherStore>();
        services.AddScoped<IMessageStore, MessageStore>();

        return services;
    }
}