using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Rendering.Infrastructure;
using Nuntios.Core.Storage.EF;
using Nuntios.Core.Storage.EF.Extensions;
using Xunit;

namespace Nuntios.Core.Test.Storage;

public class RendererStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IRendererStore _rendererStore;
    private readonly NuntiusDbContext _context;

    public RendererStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusEfStorageInMemory();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _rendererStore = _serviceProvider.GetRequiredService<IRendererStore>();

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_NewRenderer_ShouldSaveSuccessfully()
    {
        // Arrange
        var rendererResult = Renderer.Create("renderer-1", "Test Renderer", "liquid", "{}");
        rendererResult.Success.Should().BeTrue();
        var renderer = rendererResult.Value;

        // Act
        var savedRenderer = await _rendererStore.SaveAsync(renderer);

        // Assert
        savedRenderer.Should().NotBeNull();
        savedRenderer.Id.Should().Be("renderer-1");
        savedRenderer.Name.Should().Be("Test Renderer");
        savedRenderer.EngineId.Should().Be("LIQUID");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingRenderer_ShouldReturnRenderer()
    {
        // Arrange
        var rendererResult = Renderer.Create("renderer-2", "Test Renderer 2", "razor", "{}");
        var renderer = rendererResult.Value;
        await _rendererStore.SaveAsync(renderer);

        // Act
        var retrievedRenderer = await _rendererStore.GetByIdAsync("renderer-2");

        // Assert
        retrievedRenderer.Should().NotBeNull();
        retrievedRenderer!.Id.Should().Be("renderer-2");
        retrievedRenderer.Name.Should().Be("Test Renderer 2");
        retrievedRenderer.EngineId.Should().Be("RAZOR");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleRenderers_ShouldReturnQueryable()
    {
        // Arrange
        var renderer1Result = Renderer.Create("renderer-3", "Test Renderer 3", "liquid", "{}");
        var renderer2Result = Renderer.Create("renderer-4", "Test Renderer 4", "mustache", "{}");
        
        await _rendererStore.SaveAsync(renderer1Result.Value);
        await _rendererStore.SaveAsync(renderer2Result.Value);

        // Act
        var renderersQuery = await _rendererStore.GetAllAsync();
        var renderers = await renderersQuery.ToListAsync();

        // Assert
        renderers.Should().HaveCountGreaterOrEqualTo(2);
        renderers.Should().Contain(r => r.Id == "renderer-3");
        renderers.Should().Contain(r => r.Id == "renderer-4");
    }

    [Fact]
    public async Task DeleteAsync_ExistingRenderer_ShouldRemoveRenderer()
    {
        // Arrange
        var rendererResult = Renderer.Create("renderer-5", "Test Renderer 5", "liquid", "{}");
        var renderer = rendererResult.Value;
        await _rendererStore.SaveAsync(renderer);

        // Act
        await _rendererStore.DeleteAsync(renderer);

        // Assert
        var retrievedRenderer = await _rendererStore.GetByIdAsync("renderer-5");
        retrievedRenderer.Should().BeNull();
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}