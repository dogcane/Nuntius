using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Templates.Entities;
using Nuntius.Core.Templates.Infrastructure;
using Nuntius.Core.Messages.Entities;
using Nuntios.Core.Storage.EF;
using Nuntios.Core.Storage.EF.Extensions;
using Xunit;

namespace Nuntios.Core.Test.Storage;

public class TemplateStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ITemplateStore _templateStore;
    private readonly NuntiusDbContext _context;

    public TemplateStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusEfStorageInMemory();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _templateStore = _serviceProvider.GetRequiredService<ITemplateStore>();

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_NewTemplate_ShouldSaveSuccessfully()
    {
        // Arrange
        var context = new TemplateContext("en", "test");
        var templateResult = Template.Create("template-1", "Test Template", "liquid", context, "Test Subject", "Test Content", MessageType.Email);
        templateResult.Success.Should().BeTrue();
        var template = templateResult.Value;

        // Act
        var savedTemplate = await _templateStore.SaveAsync(template);

        // Assert
        savedTemplate.Should().NotBeNull();
        savedTemplate.Id.Should().Be("template-1");
        savedTemplate.Name.Should().Be("TEST TEMPLATE");
        savedTemplate.EngineId.Should().Be("LIQUID");
        savedTemplate.Subject.Should().Be("Test Subject");
        savedTemplate.Content.Should().Be("Test Content");
        savedTemplate.MessageType.Should().Be(MessageType.Email);
        savedTemplate.Context.Language.Should().Be("en");
        savedTemplate.Context.Scope.Should().Be("test");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTemplate_ShouldReturnTemplate()
    {
        // Arrange
        var context = new TemplateContext("fr", "scope2");
        var templateResult = Template.Create("template-2", "Test Template 2", "razor", context, "Subject 2", "Content 2", MessageType.Text);
        var template = templateResult.Value;
        await _templateStore.SaveAsync(template);

        // Act
        var retrievedTemplate = await _templateStore.GetByIdAsync("template-2");

        // Assert
        retrievedTemplate.Should().NotBeNull();
        retrievedTemplate!.Id.Should().Be("template-2");
        retrievedTemplate.Name.Should().Be("TEST TEMPLATE 2");
        retrievedTemplate.EngineId.Should().Be("RAZOR");
        retrievedTemplate.Context.Language.Should().Be("fr");
        retrievedTemplate.Context.Scope.Should().Be("scope2");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleTemplates_ShouldReturnQueryable()
    {
        // Arrange
        var context1 = new TemplateContext("en");
        var context2 = new TemplateContext("es", "global");
        var template1Result = Template.Create("template-3", "Test Template 3", "liquid", context1, "Subject 3", "Content 3", MessageType.Text);
        var template2Result = Template.Create("template-4", "Test Template 4", "mustache", context2, "Subject 4", "Content 4", MessageType.Email);
        
        await _templateStore.SaveAsync(template1Result.Value);
        await _templateStore.SaveAsync(template2Result.Value);

        // Act
        var templatesQuery = await _templateStore.GetAllAsync();
        var templates = await templatesQuery.ToListAsync();

        // Assert
        templates.Should().HaveCountGreaterOrEqualTo(2);
        templates.Should().Contain(t => t.Id == "template-3");
        templates.Should().Contain(t => t.Id == "template-4");
    }

    [Fact]
    public async Task DeleteAsync_ExistingTemplate_ShouldRemoveTemplate()
    {
        // Arrange
        var context = new TemplateContext("de");
        var templateResult = Template.Create("template-5", "Test Template 5", "liquid", context, "Subject 5", "Content 5", MessageType.Text);
        var template = templateResult.Value;
        await _templateStore.SaveAsync(template);

        // Act
        await _templateStore.DeleteAsync(template);

        // Assert
        var retrievedTemplate = await _templateStore.GetByIdAsync("template-5");
        retrievedTemplate.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_UpdateExistingTemplate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var context = new TemplateContext("it");
        var templateResult = Template.Create("template-6", "Test Template 6", "liquid", context, "Subject 6", "Content 6", MessageType.Text);
        var template = templateResult.Value;
        await _templateStore.SaveAsync(template);

        // Update the template
        var updateResult = template.Update("mustache", "Updated Subject", "Updated Content");
        updateResult.Success.Should().BeTrue();

        // Act
        var updatedTemplate = await _templateStore.SaveAsync(template);

        // Assert
        var retrievedTemplate = await _templateStore.GetByIdAsync("template-6");
        retrievedTemplate.Should().NotBeNull();
        retrievedTemplate!.EngineId.Should().Be("MUSTACHE");
        retrievedTemplate.Subject.Should().Be("Updated Subject");
        retrievedTemplate.Content.Should().Be("Updated Content");
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}