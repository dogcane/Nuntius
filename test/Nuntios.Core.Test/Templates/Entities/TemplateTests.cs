using FluentAssertions;
using Moq;
using Nuntius.Core.Common;
using Nuntius.Core.Messages;
using Nuntius.Core.Templates;
using Nuntius.Core.Templates.Entities;
using Resulz;
using Xunit;

namespace Nuntius.Core.Test.Templates.Entities;

public class TemplateTests
{

    [Theory]
    [MemberData(nameof(GetTemplateTestData))]
    public void Create_ShouldReturnExpectedResult(
        string? Id,
        string? Name,
        string? EngineName,
        TemplateContext? Context,
        string? Subject,
        string? Content,
        MessageType? MessageType,
        bool ExpectedSuccess,
        string? ExpectedErrorKey = null)
    {
        // Act
#pragma warning disable CS8604 // Possibile argomento di riferimento Null.
        var result = Template.Create(
            Id,
            Name,
            EngineName,
            Context,
            Subject,
            Content,
            MessageType
            );
#pragma warning restore CS8604 // Possibile argomento di riferimento Null.

        // Assert
        result.Success.Should().Be(ExpectedSuccess);

        if (ExpectedSuccess)
        {
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(Id);
            result.Value.Name.Should().Be(Name.ToUpper());
            result.Value.EngineName.Should().Be(EngineName.ToUpper());
            result.Value.Context.Should().Be(Context);
            result.Value.Subject.Should().Be(Subject);
            result.Value.Content.Should().Be(Content);
            result.Value.MessageType.Should().Be(MessageType);
            result.Value.Status.Should().Be(ElementStatus.Enabled);
        }
        else
        {
            result.HasErrorsByContext(ExpectedErrorKey!).Should().BeTrue();
        }
    }

    public static IEnumerable<object[]> GetTemplateTestData()
    {
        var validContext = new TemplateContext("EN", "Test");

        // Valid data
        yield return new object[] {
                "template1", "TestTemplate", "LIQUID", new TemplateContext("EN", "Test"),
                "Test Subject", "Test Content", MessageType.Email, true
        };
        yield return new object[] {
                "template2", "AnotherTemplate", "RAZOR", new TemplateContext("IT", null),
                "Another Subject", "Another Content", MessageType.Text, true
        };
        yield return new object[] {
                "template3", "NotificationTemplate", "MUSTACHE", new TemplateContext("FR", "Notifications"),
                "Notification Subject", "Notification Content", MessageType.Notification, true
        };

        // Invalid data
        yield return new object[] {
            "", "TestTemplate", "LIQUID", validContext, "Subject", "Content",
                MessageType.Email, false, "id"
        };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        yield return new object[] {
            null, "TestTemplate", "LIQUID", validContext, "Subject", "Content",
                MessageType.Email, false, "id"
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        yield return new object[] {
            "template1", "", "LIQUID", validContext, "Subject", "Content",
                MessageType.Email, false, "name"
        };
        yield return new object[] {
            "template1", "TestTemplate", "", validContext, "Subject", "Content",
                MessageType.Email, false, "engineName"
        };
        yield return new object[] {
            "template1", "TestTemplate", "LIQUID", validContext, "", "Content",
                MessageType.Email, false, "subject"
        };
        yield return new object[] {
            "template1", "TestTemplate", "LIQUID", validContext, "Subject", "",
                MessageType.Email, false, "content"
        };
        yield return new object[] {
            "template1", "TestTemplate", "LIQUID", new TemplateContext("", "Test"),
                "Subject", "Content", MessageType.Email, false, "Context.Language"
        };
        yield return new object[] {
            "template1", "TestTemplate", "LIQUID", new TemplateContext("TOOLONG", "Test"),
                "Subject", "Content", MessageType.Email, false, "Context.Language"
        };
    }

    [Theory]
    [MemberData(nameof(GetValidUpdateData))]
    public void Update_WithValidData_ShouldReturnSuccess(
        string engineName,
        string subject,
        string content)
    {
        // Arrange
        var template = CreateValidTemplate();

        // Act
        var result = template.Update(engineName, subject, content);

        // Assert
        result.Success.Should().BeTrue();
        template.EngineName.Should().Be(engineName.ToUpper());
        template.Subject.Should().Be(subject);
        template.Content.Should().Be(content);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateData))]
    public void Update_WithInvalidData_ShouldReturnFailure(
        string engineName,
        string subject,
        string content,
        string expectedErrorKey)
    {
        // Arrange  
        var template = CreateValidTemplate();

        // Act  
        var result = template.Update(engineName, subject, content);

        // Assert  
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Fact]
    public void Update_WhenTemplateIsDisabled_ShouldReturnFailure()
    {
        // Arrange
        var template = CreateValidTemplate();
        template.Disable();

        // Act
        var result = template.Update("NEWENGINE", "New Subject", "New Content");

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "Status");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNullId_ShouldReturnFailure(string? invalidId)
    {
        // Arrange
        var context = new TemplateContext("EN", "Test");

        // Act
#pragma warning disable CS8604 // Possibile argomento di riferimento Null.
        var result = Template.Create(invalidId, "TestName", "TestEngine", context, "Subject", "Content", MessageType.Email);
#pragma warning restore CS8604 // Possibile argomento di riferimento Null.

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "id");
    }

    [Theory]
    [InlineData("invalid@id")]
    [InlineData("id with spaces")]
    [InlineData("very-long-id-that-exceeds-the-maximum-allowed-length-of-fifty-characters")]
    public void Create_WithInvalidIdFormat_ShouldReturnFailure(string invalidId)
    {
        // Arrange
        var context = new TemplateContext("EN", "Test");

        // Act
        var result = Template.Create(invalidId, "TestName", "TestEngine", context, "Subject", "Content", MessageType.Email);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "id");
    }

    public static IEnumerable<object[]> GetValidUpdateData()
    {
        yield return new object[] { "NEWENGINE", "New Subject", "New Content" };
        yield return new object[] { "RAZOR", "Updated Subject", "Updated Content" };
        yield return new object[] { "MUSTACHE", "Another Subject", "Another Content" };
    }

    public static IEnumerable<object[]> GetInvalidUpdateData()
    {
        yield return new object[] { "", "Valid Subject", "Valid Content", "engineName" };
        yield return new object[] { "VALIDENGINE", "", "Valid Content", "subject" };
        yield return new object[] { "VALIDENGINE", "Valid Subject", "", "content" };
        yield return new object[] { "invalid@engine", "Valid Subject", "Valid Content", "engineName" };
        yield return new object[] { "VALIDENGINE", new string('A', 101), "Valid Content", "subject" };
    }

    private static Template CreateValidTemplate()
    {
        var context = new TemplateContext("EN", "Test");
        var result = Template.Create("template1", "TestTemplate", "LIQUID", context, "Test Subject", "Test Content", MessageType.Email);
        return result.Value!;
    }
}