using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntios.Core.Storage.EF;
using Xunit;

namespace Nuntios.Core.Storage.EF.Tests;

public class RendererStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly NuntiusDbContext _context;
    private readonly IRendererStore _rendererStore;

    public RendererStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusInMemoryStorage($"TestDb_{Guid.NewGuid()}");
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _rendererStore = _serviceProvider.GetRequiredService<IRendererStore>();
        
        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistNewRenderer()
    {
        // Arrange
        var rendererResult = Renderer.Create("TEST_RENDERER", "Test Renderer", "MUSTACHE_ENGINE", "{}");
        Assert.True(rendererResult.Success);
        var renderer = rendererResult.Value!;

        // Act
        var savedRenderer = await _rendererStore.SaveAsync(renderer);

        // Assert
        Assert.NotNull(savedRenderer);
        Assert.Equal("TEST_RENDERER", savedRenderer.Id);
        Assert.Equal("Test Renderer", savedRenderer.Name);
        Assert.Equal("MUSTACHE_ENGINE", savedRenderer.EngineId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPersistedRenderer()
    {
        // Arrange
        var rendererResult = Renderer.Create("GET_TEST", "Get Test Renderer", "RAZOR_ENGINE", "{}");
        var renderer = rendererResult.Value!;
        await _rendererStore.SaveAsync(renderer);

        // Act
        var retrievedRenderer = await _rendererStore.GetByIdAsync("GET_TEST");

        // Assert
        Assert.NotNull(retrievedRenderer);
        Assert.Equal("GET_TEST", retrievedRenderer.Id);
        Assert.Equal("Get Test Renderer", retrievedRenderer.Name);
        Assert.Equal("RAZOR_ENGINE", retrievedRenderer.EngineId);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRenderers()
    {
        // Arrange
        var renderer1Result = Renderer.Create("RENDERER_1", "Renderer One", "ENGINE_1", "{}");
        var renderer2Result = Renderer.Create("RENDERER_2", "Renderer Two", "ENGINE_2", "{}");
        
        await _rendererStore.SaveAsync(renderer1Result.Value!);
        await _rendererStore.SaveAsync(renderer2Result.Value!);

        // Act
        var allRenderers = await _rendererStore.GetAllAsync();
        var renderersList = await allRenderers.ToListAsync();

        // Assert
        Assert.Equal(2, renderersList.Count);
        Assert.Contains(renderersList, r => r.Id == "RENDERER_1");
        Assert.Contains(renderersList, r => r.Id == "RENDERER_2");
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}