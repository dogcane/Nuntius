using FluentAssertions;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Messages;
using Resulz;
using Xunit;

namespace Nuntios.Core.Test.Delivery.Entities;

public class SenderTests
{
    [Theory]
    [InlineData("sender1", "TestSender", "SMTP", "{\"host\":\"smtp.gmail.com\",\"port\":587,\"username\":\"test@gmail.com\"}", 1)] // MessageType.Email
    [InlineData("sender2", "AnotherSender", "SMS", "{\"provider\":\"twilio\",\"accountSid\":\"AC123\",\"authToken\":\"token123\"}", 0)] // MessageType.Text  
    [InlineData("sender3", "NotificationSender", "PUSH", "{\"apiKey\":\"key123\",\"endpoint\":\"https://api.push.com\"}", 2)] // MessageType.Notification
    public void Create_WithValidData_ShouldReturnSuccess(
        string Id,
        string Name,
        string EngineId,
        string Settings,
        int messageTypeValue)
    {
        // Arrange
        var messageType = MessageType.All[messageTypeValue];

        // Act
        var result = Sender.Create(Id, Name, EngineId, Settings, messageType);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(Id);
        result.Value.Name.Should().Be(Name.ToUpper());
        result.Value.EngineId.Should().Be(EngineId.ToUpper());
        result.Value.Settings.Should().Be(Settings);
        result.Value.MessageType.Should().Be(messageType);
        result.Value.Status.Should().Be(ElementStatus.Enabled);
    }

    [Theory]
    [InlineData("", "id")]
    [InlineData(null, "id")]
    [InlineData("   ", "id")]
    [InlineData("invalid@id", "id")]
    [InlineData("id with spaces", "id")]
    [InlineData("very-long-id-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "id")]
    public void Create_WithInvalidId_ShouldReturnFailure(string? id, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Sender.Create(id, "TestSender", "SMTP", "{\"host\":\"smtp.test.com\",\"port\":25}", MessageType.Email);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "name")]
    [InlineData(null, "name")]
    [InlineData("qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890", "name")]
    public void Create_WithInvalidName_ShouldReturnFailure(string? name, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Sender.Create("sender1", name, "SMTP", "{\"host\":\"smtp.test.com\",\"port\":25}", MessageType.Email);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "engineId")]
    [InlineData(null, "engineId")]
    [InlineData("   ", "engineId")]
    [InlineData("invalid@engine", "engineId")]
    [InlineData("engine with spaces", "engineId")]
    [InlineData("very-long-engine-name-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "engineId")]
    public void Create_WithInvalidEngineId_ShouldReturnFailure(string? engineId, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Sender.Create("sender1", "TestSender", engineId, "{\"host\":\"smtp.test.com\",\"port\":25}", MessageType.Email);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "settings")]
    [InlineData(null, "settings")]
    [InlineData("   ", "settings")]
    public void Create_WithInvalidSettings_ShouldReturnFailure(string? settings, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Sender.Create("sender1", "TestSender", "SMTP", settings, MessageType.Email);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("TestSender", "NEWENGINE", "{\"host\":\"new.smtp.com\",\"port\":465,\"ssl\":true}")]
    [InlineData("UpdatedSender", "SMTP", "{\"host\":\"updated.smtp.com\",\"port\":587,\"tls\":true}")]
    [InlineData("AnotherSender", "SMS", "{\"provider\":\"nexmo\",\"apiKey\":\"key456\",\"secret\":\"secret456\"}")]
    [InlineData("DifferentSender", "PUSH", "{\"apiKey\":\"newkey789\",\"endpoint\":\"https://api.newpush.com\",\"timeout\":30}")]
    public void Update_WithValidData_ShouldReturnSuccess(string name, string engineId, string settings)
    {
        // Arrange
        var sender = CreateValidSender();

        // Act
        var result = sender.Update(name, engineId, settings);

        // Assert
        result.Success.Should().BeTrue();
        sender.Name.Should().Be(name.ToUpper());
        sender.EngineId.Should().Be(engineId.ToUpper());
        sender.Settings.Should().Be(settings);
    }

    [Theory]
    [InlineData("", "name")]
    [InlineData(null, "name")]
    [InlineData("qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890", "name")]
    public void Update_WithInvalidName_ShouldReturnFailure(string? name, string expectedErrorKey)
    {
        // Arrange
        var sender = CreateValidSender();

        // Act
#pragma warning disable CS8604
        var result = sender.Update(name, "VALIDENGINE", "{\"host\":\"valid.smtp.com\",\"port\":25}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "engineId")]
    [InlineData(null, "engineId")]
    [InlineData("   ", "engineId")]
    [InlineData("invalid@engine", "engineId")]
    [InlineData("engine with spaces", "engineId")]
    [InlineData("very-long-engine-name-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "engineId")]
    public void Update_WithInvalidEngineId_ShouldReturnFailure(string? engineId, string expectedErrorKey)
    {
        // Arrange
        var sender = CreateValidSender();

        // Act
#pragma warning disable CS8604
        var result = sender.Update("ValidName", engineId, "{\"host\":\"valid.smtp.com\",\"port\":25}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "settings")]
    [InlineData(null, "settings")]
    [InlineData("   ", "settings")]
    public void Update_WithInvalidSettings_ShouldReturnFailure(string? settings, string expectedErrorKey)
    {
        // Arrange
        var sender = CreateValidSender();

        // Act
#pragma warning disable CS8604
        var result = sender.Update("ValidName", "VALIDENGINE", settings);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Fact]
    public void Update_WhenSenderIsDisabled_ShouldReturnFailure()
    {
        // Arrange
        var sender = CreateValidSender();
        sender.Disable();

        // Act
        var result = sender.Update("NewName", "NEWENGINE", "{\"host\":\"new.smtp.com\",\"port\":465,\"ssl\":true}");

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "Status");
    }

    private static Sender CreateValidSender()
    {
        var result = Sender.Create("sender1", "TestSender", "SMTP", "{\"host\":\"smtp.test.com\",\"port\":25}", MessageType.Email);
        return result.Value!;
    }
}
