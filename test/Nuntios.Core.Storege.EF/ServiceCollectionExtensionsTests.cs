using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Fetching.Infrastructure;
using Nuntius.Core.Messages.Infrastructure;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntius.Core.Templates.Infrastructure;
using Nuntios.Core.Storage.EF;
using Nuntios.Core.Storage.EF.Extensions;

namespace Nuntios.Core.Test.Storage;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddNuntiusEfStorageInMemory_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNuntiusEfStorageInMemory();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<NuntiusDbContext>().Should().NotBeNull();
        serviceProvider.GetService<ISenderStore>().Should().NotBeNull();
        serviceProvider.GetService<IRendererStore>().Should().NotBeNull();
        serviceProvider.GetService<ITemplateStore>().Should().NotBeNull();
        serviceProvider.GetService<IDataFetcherStore>().Should().NotBeNull();
        serviceProvider.GetService<IMessageStore>().Should().NotBeNull();
    }

    [Fact]
    public void AddNuntiusEfStorageSqlServer_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Server=localhost;Database=Test;Trusted_Connection=true;";

        // Act
        services.AddNuntiusEfStorageSqlServer(connectionString);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<NuntiusDbContext>().Should().NotBeNull();
        serviceProvider.GetService<ISenderStore>().Should().NotBeNull();
        serviceProvider.GetService<IRendererStore>().Should().NotBeNull();
        serviceProvider.GetService<ITemplateStore>().Should().NotBeNull();
        serviceProvider.GetService<IDataFetcherStore>().Should().NotBeNull();
        serviceProvider.GetService<IMessageStore>().Should().NotBeNull();
    }

    [Fact]
    public void AddNuntiusEfStorage_WithCustomConfiguration_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNuntiusEfStorage(options => options.UseInMemoryDatabase("CustomTest"));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<NuntiusDbContext>().Should().NotBeNull();
        serviceProvider.GetService<ISenderStore>().Should().NotBeNull();
        serviceProvider.GetService<IRendererStore>().Should().NotBeNull();
        serviceProvider.GetService<ITemplateStore>().Should().NotBeNull();
        serviceProvider.GetService<IDataFetcherStore>().Should().NotBeNull();
        serviceProvider.GetService<IMessageStore>().Should().NotBeNull();
        
        // Verify the DbContext is properly configured
        var context = serviceProvider.GetRequiredService<NuntiusDbContext>();
        context.Database.IsInMemory().Should().BeTrue();
    }
}