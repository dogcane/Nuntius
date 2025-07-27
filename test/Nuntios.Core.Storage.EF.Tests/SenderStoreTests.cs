using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Delivery.Infrastructure;
using Nuntius.Core.Messages.Entities;
using Nuntios.Core.Storage.EF;
using Xunit;

namespace Nuntios.Core.Storage.EF.Tests;

public class SenderStoreTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly NuntiusDbContext _context;
    private readonly ISenderStore _senderStore;

    public SenderStoreTests()
    {
        var services = new ServiceCollection();
        services.AddNuntiusInMemoryStorage($"TestDb_{Guid.NewGuid()}");
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<NuntiusDbContext>();
        _senderStore = _serviceProvider.GetRequiredService<ISenderStore>();
        
        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistNewSender()
    {
        // Arrange
        var senderResult = Sender.Create("TEST_SENDER", "Test Sender", "EMAIL_ENGINE", "{}", MessageType.Email);
        Assert.True(senderResult.Success);
        var sender = senderResult.Value!;

        // Act
        var savedSender = await _senderStore.SaveAsync(sender);

        // Assert
        Assert.NotNull(savedSender);
        Assert.Equal("TEST_SENDER", savedSender.Id);
        Assert.Equal("TEST SENDER", savedSender.Name); // Should be uppercase
        Assert.Equal("EMAIL_ENGINE", savedSender.EngineId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPersistedSender()
    {
        // Arrange
        var senderResult = Sender.Create("GET_TEST", "Get Test Sender", "EMAIL_ENGINE", "{}", MessageType.Email);
        var sender = senderResult.Value!;
        await _senderStore.SaveAsync(sender);

        // Act
        var retrievedSender = await _senderStore.GetByIdAsync("GET_TEST");

        // Assert
        Assert.NotNull(retrievedSender);
        Assert.Equal("GET_TEST", retrievedSender.Id);
        Assert.Equal("GET TEST SENDER", retrievedSender.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSenders()
    {
        // Arrange
        var sender1Result = Sender.Create("SENDER_1", "Sender One", "EMAIL_ENGINE", "{}", MessageType.Email);
        var sender2Result = Sender.Create("SENDER_2", "Sender Two", "SMS_ENGINE", "{}", MessageType.Text);
        
        await _senderStore.SaveAsync(sender1Result.Value!);
        await _senderStore.SaveAsync(sender2Result.Value!);

        // Act
        var allSenders = await _senderStore.GetAllAsync();
        var sendersList = await allSenders.ToListAsync();

        // Assert
        Assert.Equal(2, sendersList.Count);
        Assert.Contains(sendersList, s => s.Id == "SENDER_1");
        Assert.Contains(sendersList, s => s.Id == "SENDER_2");
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingSender()
    {
        // Arrange
        var senderResult = Sender.Create("UPDATE_TEST", "Original Name", "EMAIL_ENGINE", "{}", MessageType.Email);
        var sender = senderResult.Value!;
        await _senderStore.SaveAsync(sender);

        // Update the sender
        var updateResult = sender.Update("Updated Name", "NEW_ENGINE", "{}");
        Assert.True(updateResult.Success);

        // Act
        var updatedSender = await _senderStore.SaveAsync(sender);

        // Assert
        var retrievedSender = await _senderStore.GetByIdAsync("UPDATE_TEST");
        Assert.NotNull(retrievedSender);
        Assert.Equal("UPDATED NAME", retrievedSender.Name);
        Assert.Equal("NEW_ENGINE", retrievedSender.EngineId);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSender()
    {
        // Arrange
        var senderResult = Sender.Create("DELETE_TEST", "Delete Test", "EMAIL_ENGINE", "{}", MessageType.Email);
        var sender = senderResult.Value!;
        await _senderStore.SaveAsync(sender);

        // Act
        await _senderStore.DeleteAsync(sender);

        // Assert
        var deletedSender = await _senderStore.GetByIdAsync("DELETE_TEST");
        Assert.Null(deletedSender);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}