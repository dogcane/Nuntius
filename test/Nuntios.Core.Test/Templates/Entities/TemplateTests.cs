using FluentAssertions;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages.Entities;
using Nuntius.Core.Templates.Entities;
using Resulz;
using Xunit;

namespace Nuntius.Core.Test.Templates.Entities;

public class TemplateTests
{


        [Theory]
        [InlineData("template1", "TestTemplate", "LIQUID", "EN", "Test", "Test Subject", "Test Content", 1)] // MessageType.Email
        [InlineData("template2", "AnotherTemplate", "RAZOR", "IT", null, "Another Subject", "Another Content", 0)] // MessageType.Text  
        [InlineData("template3", "NotificationTemplate", "MUSTACHE", "FR", "Notifications", "Notification Subject", "Notification Content", 2)] // MessageType.Notification
        public void Create_WithValidData_ShouldReturnSuccess(
            string Id,
            string Name,
            string EngineId,
            string Language,
            string? ContextType,
            string Subject,
            string Content,
            int messageTypeValue)
        {
                // Arrange
                var context = new TemplateContext(Language, ContextType);
                var messageType = MessageType.All[messageTypeValue];

                // Act
                var result = Template.Create(Id, Name, EngineId, context, Subject, Content, messageType);

                // Assert
                result.Success.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value!.Id.Should().Be(Id);
                result.Value.Name.Should().Be(Name.ToUpper());
                result.Value.EngineId.Should().Be(EngineId.ToUpper());
                result.Value.Context.Should().Be(context);
                result.Value.Subject.Should().Be(Subject);
                result.Value.Content.Should().Be(Content);
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
                // Arrange
                var context = new TemplateContext("EN", "Test");

                // Act
#pragma warning disable CS8604
                var result = Template.Create(id, "TestTemplate", "LIQUID", context, "Subject", "Content", MessageType.Email);
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
                // Arrange
                var context = new TemplateContext("EN", "Test");

                // Act
#pragma warning disable CS8604
                var result = Template.Create("template1", name, "LIQUID", context, "Subject", "Content", MessageType.Email);
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
                // Arrange
                var context = new TemplateContext("EN", "Test");

                // Act
#pragma warning disable CS8604
                var result = Template.Create("template1", "TestTemplate", engineId, context, "Subject", "Content", MessageType.Email);
#pragma warning restore CS8604

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "subject")]
        [InlineData(null, "subject")]
        [InlineData("qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890", "subject")]
        public void Create_WithInvalidSubject_ShouldReturnFailure(string? subject, string expectedErrorKey)
        {
                // Arrange
                var context = new TemplateContext("EN", "Test");

                // Act
#pragma warning disable CS8604
                var result = Template.Create("template1", "TestTemplate", "LIQUID", context, subject, "Content", MessageType.Email);
#pragma warning restore CS8604

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "content")]
        [InlineData(null, "content")]
        //[InlineData("   ", "content")]
        public void Create_WithInvalidContent_ShouldReturnFailure(string? content, string expectedErrorKey)
        {
                // Arrange
                var context = new TemplateContext("EN", "Test");

                // Act
#pragma warning disable CS8604
                var result = Template.Create("template1", "TestTemplate", "LIQUID", context, "Subject", content, MessageType.Email);
#pragma warning restore CS8604

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Test", "Context.Language")]
        [InlineData("TOOLONG", "Test", "Context.Language")]
        public void Create_WithInvalidContext_ShouldReturnFailure(string language, string? contextType, string expectedErrorKey)
        {
                // Arrange
                var context = new TemplateContext(language, contextType);

                // Act
                var result = Template.Create("template1", "TestTemplate", "LIQUID", context, "Subject", "Content", MessageType.Email);

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("NEWENGINE", "New Subject", "New Content")]
        [InlineData("RAZOR", "Updated Subject", "Updated Content")]
        [InlineData("MUSTACHE", "Another Subject", "Another Content")]
        [InlineData("LIQUID", "Different Subject", "Different Content")]
        public void Update_WithValidData_ShouldReturnSuccess(string engineId, string subject, string content)
        {
                // Arrange
                var template = CreateValidTemplate();

                // Act
                var result = template.Update(engineId, subject, content);

                // Assert
                result.Success.Should().BeTrue();
                template.EngineId.Should().Be(engineId.ToUpper());
                template.Subject.Should().Be(subject);
                template.Content.Should().Be(content);
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
                var template = CreateValidTemplate();

                // Act
#pragma warning disable CS8604
                var result = template.Update(engineId, "Valid Subject", "Valid Content");
#pragma warning restore CS8604

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "subject")]
        [InlineData(null, "subject")]
        [InlineData("qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890qwertyuiopasdfghjklzxcvbnm1234567890", "subject")]
        public void Update_WithInvalidSubject_ShouldReturnFailure(string? subject, string expectedErrorKey)
        {
                // Arrange
                var template = CreateValidTemplate();

                // Act
#pragma warning disable CS8604
                var result = template.Update("VALIDENGINE", subject, "Valid Content");
#pragma warning restore CS8604

                // Assert
                result.Success.Should().BeFalse();
                result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "content")]
        [InlineData(null, "content")]
        //[InlineData("   ", "content")]
        public void Update_WithInvalidContent_ShouldReturnFailure(string? content, string expectedErrorKey)
        {
                // Arrange
                var template = CreateValidTemplate();

                // Act
#pragma warning disable CS8604
                var result = template.Update("VALIDENGINE", "Valid Subject", content);
#pragma warning restore CS8604

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

        private static Template CreateValidTemplate()
        {
                var context = new TemplateContext("EN", "Test");
                var result = Template.Create("template1", "TestTemplate", "LIQUID", context, "Test Subject", "Test Content", MessageType.Email);
                return result.Value!;
        }
}