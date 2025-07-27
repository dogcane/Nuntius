using FluentAssertions;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Rendering.Entities;
using Resulz;
using Xunit;

namespace Nuntios.Core.Test.Rendering.Entities;

public class RendererTests
{
    [Theory]
    [InlineData("renderer1", "TestRenderer", "PDF", "{\"pageSize\":\"A4\",\"orientation\":\"portrait\",\"margins\":\"1cm\"}")]
    [InlineData("renderer2", "AnotherRenderer", "HTML", "{\"template\":\"default\",\"theme\":\"modern\",\"responsive\":true}")]
    [InlineData("renderer3", "ExcelRenderer", "EXCEL", "{\"sheetName\":\"Report\",\"format\":\"xlsx\",\"autoFit\":true}")]
    public void Create_WithValidData_ShouldReturnSuccess(
        string Id,
        string Name,
        string EngineId,
        string Settings)
    {
        // Act
        var result = Renderer.Create(Id, Name, EngineId, Settings);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(Id);
        result.Value.Name.Should().Be(Name);
        result.Value.EngineId.Should().Be(EngineId.ToUpper());
        result.Value.Settings.Should().Be(Settings);
        result.Value.Status.Should().Be(ElementStatus.Enabled);
    }

    [Theory]
    [InlineData("", "id")]
    [InlineData(null, "id")]
    [InlineData("invalid@id", "id")]
    [InlineData("id with spaces", "id")]
    [InlineData("very-long-id-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "id")]
    public void Create_WithInvalidId_ShouldReturnFailure(string? id, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Renderer.Create(id, "TestRenderer", "PDF", "{\"pageSize\":\"A4\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "name")]
    [InlineData(null, "name")]
    public void Create_WithInvalidName_ShouldReturnFailure(string? name, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Renderer.Create("renderer1", name, "PDF", "{\"pageSize\":\"A4\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("VeryLongNameThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersForTheNamePropertyWhichShouldCauseAValidationErrorBecauseItIsTooLongAndShouldNotBeAcceptedByTheValidationRulesDefinedInTheRendererValidationsClassThatChecksTheStringLengthOfTheNameProperty", "name")]
    public void Create_WithNameTooLong_ShouldReturnFailure(string longName, string expectedErrorKey)
    {
        // Act
        var result = Renderer.Create("renderer1", longName, "PDF", "{\"pageSize\":\"A4\"}");

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "engineId")]
    [InlineData(null, "engineId")]
    [InlineData("invalid@engine", "engineId")]
    [InlineData("engine with spaces", "engineId")]
    [InlineData("very-long-engine-name-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "engineId")]
    public void Create_WithInvalidEngineId_ShouldReturnFailure(string? engineId, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Renderer.Create("renderer1", "TestRenderer", engineId, "{\"pageSize\":\"A4\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "settings")]
    [InlineData(null, "settings")]
    [InlineData("invalid json", "settings")]
    [InlineData("{invalid: json}", "settings")]
    [InlineData("{'singleQuotes': 'invalid'}", "settings")]
    [InlineData("{\"unclosed\": \"json\"", "settings")]
    public void Create_WithInvalidSettings_ShouldReturnFailure(string? settings, string expectedErrorKey)
    {
        // Act
#pragma warning disable CS8604
        var result = Renderer.Create("renderer1", "TestRenderer", "PDF", settings);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("UpdatedRenderer", "NEWPDF", "{\"pageSize\":\"A3\",\"orientation\":\"landscape\",\"margins\":\"2cm\"}")]
    [InlineData("AnotherRenderer", "HTML", "{\"template\":\"updated\",\"theme\":\"dark\",\"responsive\":false}")]
    [InlineData("DifferentRenderer", "EXCEL", "{\"sheetName\":\"UpdatedReport\",\"format\":\"xls\",\"autoFit\":false}")]
    [InlineData("SimpleRenderer", "WORD", "{\"docFormat\":\"docx\",\"fontSize\":12,\"fontFamily\":\"Arial\"}")]
    public void Update_WithValidData_ShouldReturnSuccess(string name, string engineId, string settings)
    {
        // Arrange
        var renderer = CreateValidRenderer();

        // Act
        var result = renderer.Update(name, engineId, settings);

        // Assert
        result.Success.Should().BeTrue();
        renderer.Name.Should().Be(name);
        renderer.EngineId.Should().Be(engineId.ToUpper());
        renderer.Settings.Should().Be(settings);
    }

    [Theory]
    [InlineData("", "name")]
    [InlineData(null, "name")]
    public void Update_WithInvalidName_ShouldReturnFailure(string? name, string expectedErrorKey)
    {
        // Arrange
        var renderer = CreateValidRenderer();

        // Act
#pragma warning disable CS8604
        var result = renderer.Update(name, "VALIDENGINE", "{\"pageSize\":\"A4\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("VeryLongNameThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersForTheNamePropertyWhichShouldCauseAValidationErrorBecauseItIsTooLongAndShouldNotBeAcceptedByTheValidationRulesDefinedInTheRendererValidationsClassThatChecksTheStringLengthOfTheNameProperty", "name")]
    public void Update_WithNameTooLong_ShouldReturnFailure(string longName, string expectedErrorKey)
    {
        // Arrange
        var renderer = CreateValidRenderer();

        // Act
        var result = renderer.Update(longName, "VALIDENGINE", "{\"pageSize\":\"A4\"}");

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "engineId")]
    [InlineData(null, "engineId")]
    [InlineData("invalid@engine", "engineId")]
    [InlineData("engine with spaces", "engineId")]
    [InlineData("very-long-engine-name-that-exceeds-the-maximum-allowed-length-of-fifty-characters", "engineId")]
    public void Update_WithInvalidEngineId_ShouldReturnFailure(string? engineId, string expectedErrorKey)
    {
        // Arrange
        var renderer = CreateValidRenderer();

        // Act
#pragma warning disable CS8604
        var result = renderer.Update("ValidName", engineId, "{\"pageSize\":\"A4\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("", "settings")]
    [InlineData(null, "settings")]
    [InlineData("invalid json", "settings")]
    [InlineData("{invalid: json}", "settings")]
    [InlineData("{'singleQuotes': 'invalid'}", "settings")]
    [InlineData("{\"unclosed\": \"json\"", "settings")]
    public void Update_WithInvalidSettings_ShouldReturnFailure(string? settings, string expectedErrorKey)
    {
        // Arrange
        var renderer = CreateValidRenderer();

        // Act
#pragma warning disable CS8604
        var result = renderer.Update("ValidName", "VALIDENGINE", settings);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Fact]
    public void Update_WhenRendererIsDisabled_ShouldReturnFailure()
    {
        // Arrange
        var renderer = CreateValidRenderer();
        renderer.Disable();

        // Act
        var result = renderer.Update("NewName", "NEWENGINE", "{\"pageSize\":\"A4\"}");

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "Status");
    }

    private static Renderer CreateValidRenderer()
    {
        var result = Renderer.Create("renderer1", "TestRenderer", "PDF", "{\"pageSize\":\"A4\",\"orientation\":\"portrait\",\"margins\":\"1cm\"}");
        return result.Value!;
    }
}