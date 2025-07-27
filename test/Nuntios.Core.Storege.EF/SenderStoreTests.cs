using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Messages.Entities;
using Nuntios.Core.Storage.EF;
using Nuntios.Core.Storage.EF.Extensions;
using Nuntios.Core.Storage.EF.Stores;
using Xunit;

namespace Nuntios.Core.Test.Storage;

public class SenderStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ISenderStore _senderStore;
    private readonly NuntiusDbContext _context;

    public SenderStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusEfStorageInMemory();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _senderStore = _serviceProvider.GetRequiredService<ISenderStore>();

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_NewSender_ShouldSaveSuccessfully()
    {
        // Arrange
        var senderResult = Sender.Create("test-id", "Test Sender", "smtp", "{}", MessageType.Text);
        senderResult.Success.Should().BeTrue();
        var sender = senderResult.Value;

        // Act
        var savedSender = await _senderStore.SaveAsync(sender);

        // Assert
        savedSender.Should().NotBeNull();
        savedSender.Id.Should().Be("test-id");
        savedSender.Name.Should().Be("TEST SENDER");
        savedSender.EngineId.Should().Be("SMTP");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSender_ShouldReturnSender()
    {
        // Arrange
        var senderResult = Sender.Create("test-id-2", "Test Sender 2", "smtp", "{}", MessageType.Text);
        var sender = senderResult.Value;
        await _senderStore.SaveAsync(sender);

        // Act
        var retrievedSender = await _senderStore.GetByIdAsync("test-id-2");

        // Assert
        retrievedSender.Should().NotBeNull();
        retrievedSender!.Id.Should().Be("test-id-2");
        retrievedSender.Name.Should().Be("TEST SENDER 2");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentSender_ShouldReturnNull()
    {
        // Act
        var retrievedSender = await _senderStore.GetByIdAsync("non-existent");

        // Assert
        retrievedSender.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleSenders_ShouldReturnQueryable()
    {
        // Arrange
        var sender1Result = Sender.Create("test-id-3", "Test Sender 3", "smtp", "{}", MessageType.Text);
        var sender2Result = Sender.Create("test-id-4", "Test Sender 4", "sendgrid", "{}", MessageType.Email);
        
        await _senderStore.SaveAsync(sender1Result.Value);
        await _senderStore.SaveAsync(sender2Result.Value);

        // Act
        var sendersQuery = await _senderStore.GetAllAsync();
        var senders = await sendersQuery.ToListAsync();

        // Assert
        senders.Should().HaveCountGreaterOrEqualTo(2);
        senders.Should().Contain(s => s.Id == "test-id-3");
        senders.Should().Contain(s => s.Id == "test-id-4");
    }

    [Fact]
    public async Task DeleteAsync_ExistingSender_ShouldRemoveSender()
    {
        // Arrange
        var senderResult = Sender.Create("test-id-5", "Test Sender 5", "smtp", "{}", MessageType.Text);
        var sender = senderResult.Value;
        await _senderStore.SaveAsync(sender);

        // Act
        await _senderStore.DeleteAsync(sender);

        // Assert
        var retrievedSender = await _senderStore.GetByIdAsync("test-id-5");
        retrievedSender.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_UpdateExistingSender_ShouldUpdateSuccessfully()
    {
        // Arrange
        var senderResult = Sender.Create("test-id-6", "Test Sender 6", "smtp", "{}", MessageType.Text);
        var sender = senderResult.Value;
        await _senderStore.SaveAsync(sender);

        // Update the sender
        var updateResult = sender.Update("Updated Sender", "sendgrid", "{}");
        updateResult.Success.Should().BeTrue();

        // Act
        var updatedSender = await _senderStore.SaveAsync(sender);

        // Assert
        var retrievedSender = await _senderStore.GetByIdAsync("test-id-6");
        retrievedSender.Should().NotBeNull();
        retrievedSender!.Name.Should().Be("UPDATED SENDER");
        retrievedSender.EngineId.Should().Be("SENDGRID");
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}