using FluentAssertions;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Fetching.Entities;
using Resulz;
using Xunit;

namespace Nuntios.Core.Test.Fetching.Entities;

public class DataFetcherTests
{
    [Theory]
    [InlineData("fetcher1", "TestDataFetcher", "SQL", "{\"connectionString\":\"Server=localhost;Database=test;Trusted_Connection=true;\"}")]
    [InlineData("fetcher2", "AnotherDataFetcher", "REST", "{\"baseUrl\":\"https://api.example.com\",\"apiKey\":\"key123\",\"timeout\":30}")]
    [InlineData("fetcher3", "FileDataFetcher", "FILE", "{\"path\":\"/data/files\",\"pattern\":\"*.csv\",\"encoding\":\"utf-8\"}")]
    public void Create_WithValidData_ShouldReturnSuccess(
        string Id,
        string Name,
        string EngineId,
        string Settings)
    {
        // Act
        var result = DataFetcher.Create(Id, Name, EngineId, Settings);

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
        var result = DataFetcher.Create(id, "TestDataFetcher", "SQL", "{\"connectionString\":\"test\"}");
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
        var result = DataFetcher.Create("fetcher1", name, "SQL", "{\"connectionString\":\"test\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("VeryLongNameThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersForTheNamePropertyWhichShouldCauseAValidationErrorBecauseItIsTooLongAndShouldNotBeAcceptedByTheValidationRulesDefinedInTheDataFetcherValidationsClassThatChecksTheStringLengthOfTheNameProperty", "name")]
    public void Create_WithNameTooLong_ShouldReturnFailure(string longName, string expectedErrorKey)
    {
        // Act
        var result = DataFetcher.Create("fetcher1", longName, "SQL", "{\"connectionString\":\"test\"}");

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
        var result = DataFetcher.Create("fetcher1", "TestDataFetcher", engineId, "{\"connectionString\":\"test\"}");
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
        var result = DataFetcher.Create("fetcher1", "TestDataFetcher", "SQL", settings);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("UpdatedDataFetcher", "NEWSQL", "{\"connectionString\":\"Server=new;Database=updated;Trusted_Connection=true;\"}")]
    [InlineData("AnotherDataFetcher", "REST", "{\"baseUrl\":\"https://api.updated.com\",\"apiKey\":\"newkey456\",\"timeout\":60}")]
    [InlineData("DifferentDataFetcher", "FILE", "{\"path\":\"/new/data/files\",\"pattern\":\"*.json\",\"encoding\":\"utf-8\"}")]
    [InlineData("SimpleDataFetcher", "CACHE", "{\"cacheSize\":1000,\"ttl\":3600}")]
    public void Update_WithValidData_ShouldReturnSuccess(string name, string engineId, string settings)
    {
        // Arrange
        var dataFetcher = CreateValidDataFetcher();

        // Act
        var result = dataFetcher.Update(name, engineId, settings);

        // Assert
        result.Success.Should().BeTrue();
        dataFetcher.Name.Should().Be(name);
        dataFetcher.EngineId.Should().Be(engineId.ToUpper());
        dataFetcher.Settings.Should().Be(settings);
    }

    [Theory]
    [InlineData("", "name")]
    [InlineData(null, "name")]
    public void Update_WithInvalidName_ShouldReturnFailure(string? name, string expectedErrorKey)
    {
        // Arrange
        var dataFetcher = CreateValidDataFetcher();

        // Act
#pragma warning disable CS8604
        var result = dataFetcher.Update(name, "VALIDENGINE", "{\"connectionString\":\"test\"}");
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Theory]
    [InlineData("VeryLongNameThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersForTheNamePropertyWhichShouldCauseAValidationErrorBecauseItIsTooLongAndShouldNotBeAcceptedByTheValidationRulesDefinedInTheDataFetcherValidationsClassThatChecksTheStringLengthOfTheNameProperty", "name")]
    public void Update_WithNameTooLong_ShouldReturnFailure(string longName, string expectedErrorKey)
    {
        // Arrange
        var dataFetcher = CreateValidDataFetcher();

        // Act
        var result = dataFetcher.Update(longName, "VALIDENGINE", "{\"connectionString\":\"test\"}");

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
        var dataFetcher = CreateValidDataFetcher();

        // Act
#pragma warning disable CS8604
        var result = dataFetcher.Update("ValidName", engineId, "{\"connectionString\":\"test\"}");
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
        var dataFetcher = CreateValidDataFetcher();

        // Act
#pragma warning disable CS8604
        var result = dataFetcher.Update("ValidName", "VALIDENGINE", settings);
#pragma warning restore CS8604

        // Assert
        result.Success.Should().BeFalse();
        result.HasErrorsByContext(expectedErrorKey).Should().BeTrue();
    }

    [Fact]
    public void Update_WhenDataFetcherIsDisabled_ShouldReturnFailure()
    {
        // Arrange
        var dataFetcher = CreateValidDataFetcher();
        dataFetcher.Disable();

        // Act
        var result = dataFetcher.Update("NewName", "NEWENGINE", "{\"connectionString\":\"new server\"}");

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(err => err.Context == "Status");
    }

    private static DataFetcher CreateValidDataFetcher()
    {
        var result = DataFetcher.Create("fetcher1", "TestDataFetcher", "SQL", "{\"connectionString\":\"Server=test;Database=test;Trusted_Connection=true;\"}");
        return result.Value!;
    }
}
