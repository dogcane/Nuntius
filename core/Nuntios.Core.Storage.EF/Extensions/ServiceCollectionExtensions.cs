using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Fetching.Infrastructure;
using Nuntius.Core.Messages.Infrastructure;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntius.Core.Templates.Infrastructure;
using Nuntios.Core.Storage.EF.Stores;

namespace Nuntios.Core.Storage.EF.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNuntiusEfStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext)
    {
        services.AddDbContext<NuntiusDbContext>(configureDbContext);
        
        services.AddScoped<ISenderStore, SenderStore>();
        services.AddScoped<IRendererStore, RendererStore>();
        services.AddScoped<ITemplateStore, TemplateStore>();
        services.AddScoped<IDataFetcherStore, DataFetcherStore>();
        services.AddScoped<IMessageStore, MessageStore>();

        return services;
    }

    public static IServiceCollection AddNuntiusEfStorageInMemory(this IServiceCollection services, string? databaseName = null)
    {
        return services.AddNuntiusEfStorage(options =>
            options.UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString()));
    }

    public static IServiceCollection AddNuntiusEfStorageSqlServer(this IServiceCollection services, string connectionString)
    {
        return services.AddNuntiusEfStorage(options =>
            options.UseSqlServer(connectionString));
    }
}