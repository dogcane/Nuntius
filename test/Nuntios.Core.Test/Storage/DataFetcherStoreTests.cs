using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Fetching.Infrastructure;
using Nuntios.Core.Storage.EF;
using Nuntios.Core.Storage.EF.Extensions;
using Xunit;

namespace Nuntios.Core.Test.Storage;

public class DataFetcherStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IDataFetcherStore _dataFetcherStore;
    private readonly NuntiusDbContext _context;

    public DataFetcherStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusEfStorageInMemory();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _dataFetcherStore = _serviceProvider.GetRequiredService<IDataFetcherStore>();

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_NewDataFetcher_ShouldSaveSuccessfully()
    {
        // Arrange
        var dataFetcherResult = DataFetcher.Create("fetcher-1", "Test DataFetcher", "sql", "{}");
        dataFetcherResult.Success.Should().BeTrue();
        var dataFetcher = dataFetcherResult.Value;

        // Act
        var savedDataFetcher = await _dataFetcherStore.SaveAsync(dataFetcher);

        // Assert
        savedDataFetcher.Should().NotBeNull();
        savedDataFetcher.Id.Should().Be("fetcher-1");
        savedDataFetcher.Name.Should().Be("Test DataFetcher");
        savedDataFetcher.EngineId.Should().Be("SQL");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingDataFetcher_ShouldReturnDataFetcher()
    {
        // Arrange
        var dataFetcherResult = DataFetcher.Create("fetcher-2", "Test DataFetcher 2", "rest", "{}");
        var dataFetcher = dataFetcherResult.Value;
        await _dataFetcherStore.SaveAsync(dataFetcher);

        // Act
        var retrievedDataFetcher = await _dataFetcherStore.GetByIdAsync("fetcher-2");

        // Assert
        retrievedDataFetcher.Should().NotBeNull();
        retrievedDataFetcher!.Id.Should().Be("fetcher-2");
        retrievedDataFetcher.Name.Should().Be("Test DataFetcher 2");
        retrievedDataFetcher.EngineId.Should().Be("REST");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleDataFetchers_ShouldReturnQueryable()
    {
        // Arrange
        var dataFetcher1Result = DataFetcher.Create("fetcher-3", "Test DataFetcher 3", "graphql", "{}");
        var dataFetcher2Result = DataFetcher.Create("fetcher-4", "Test DataFetcher 4", "mongodb", "{}");
        
        await _dataFetcherStore.SaveAsync(dataFetcher1Result.Value);
        await _dataFetcherStore.SaveAsync(dataFetcher2Result.Value);

        // Act
        var dataFetchersQuery = await _dataFetcherStore.GetAllAsync();
        var dataFetchers = await dataFetchersQuery.ToListAsync();

        // Assert
        dataFetchers.Should().HaveCountGreaterOrEqualTo(2);
        dataFetchers.Should().Contain(d => d.Id == "fetcher-3");
        dataFetchers.Should().Contain(d => d.Id == "fetcher-4");
    }

    [Fact]
    public async Task DeleteAsync_ExistingDataFetcher_ShouldRemoveDataFetcher()
    {
        // Arrange
        var dataFetcherResult = DataFetcher.Create("fetcher-5", "Test DataFetcher 5", "sql", "{}");
        var dataFetcher = dataFetcherResult.Value;
        await _dataFetcherStore.SaveAsync(dataFetcher);

        // Act
        await _dataFetcherStore.DeleteAsync(dataFetcher);

        // Assert
        var retrievedDataFetcher = await _dataFetcherStore.GetByIdAsync("fetcher-5");
        retrievedDataFetcher.Should().BeNull();
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}