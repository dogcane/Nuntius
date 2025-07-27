using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Fetching.Infrastructure;
using Nuntius.Core.Messages.Infrastructure;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntius.Core.Templates.Infrastructure;
using Nuntios.Core.Storage.EF;
using Xunit;

namespace Nuntios.Core.Storage.EF.Tests;

public class ServiceCollectionExtensionsTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ServiceCollectionExtensionsTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusInMemoryStorage($"TestDb_{Guid.NewGuid()}");
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void AddNuntiusInMemoryStorage_ShouldRegisterAllStores()
    {
        // Act & Assert - Verify all stores are registered
        var senderStore = _serviceProvider.GetService<ISenderStore>();
        var rendererStore = _serviceProvider.GetService<IRendererStore>();
        var templateStore = _serviceProvider.GetService<ITemplateStore>();
        var dataFetcherStore = _serviceProvider.GetService<IDataFetcherStore>();
        var messageStore = _serviceProvider.GetService<IMessageStore>();
        var dbContext = _serviceProvider.GetService<NuntiusDbContext>();

        Assert.NotNull(senderStore);
        Assert.NotNull(rendererStore);
        Assert.NotNull(templateStore);
        Assert.NotNull(dataFetcherStore);
        Assert.NotNull(messageStore);
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddNuntiusInMemoryStorage_ShouldRegisterStoresAsScoped()
    {
        // Act - Create two scopes
        using var scope1 = _serviceProvider.CreateScope();
        using var scope2 = _serviceProvider.CreateScope();

        var senderStore1 = scope1.ServiceProvider.GetService<ISenderStore>();
        var senderStore2 = scope2.ServiceProvider.GetService<ISenderStore>();

        // Assert - Should be different instances (scoped)
        Assert.NotNull(senderStore1);
        Assert.NotNull(senderStore2);
        Assert.NotSame(senderStore1, senderStore2);
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}