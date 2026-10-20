namespace DiscordLoLCoach.Tests.Services.Contracts;

using System.Threading.Tasks;
using DiscordLoLCoach.Application.Interfaces;
using Xunit;

/// <summary>
/// Abstract contract tests for IAppSetting implementations.
/// </summary>
public abstract class IAppSettingContractTests : IAsyncLifetime
{
    protected abstract Task<IAppSetting> CreateSutAsync();

    public virtual Task InitializeAsync() => Task.CompletedTask;
    public virtual Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Contract_DiscordSettings_IsNotNull()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.NotNull(sut.DiscordSettings);
    }

    [Fact]
    public async Task Contract_DiscordSettings_Token_IsPopulated()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(sut.DiscordSettings.Token));
    }

    [Fact]
    public async Task Contract_IsServerAllowed_WhenConfiguredAllowedServer_ReturnsTrue()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.True(sut.IsServerAllowed(111222333));
    }

    [Fact]
    public async Task Contract_IsServerAllowed_WhenUnconfiguredServer_ReturnsFalse()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.False(sut.IsServerAllowed(999999999));
    }

    [Fact]
    public async Task Contract_IsServerAllowed_WhenNullServerId_ReturnsFalse()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.False(sut.IsServerAllowed(null));
    }
}

/// <summary>
/// Alias without interface prefix matching alternative naming convention.
/// </summary>
public abstract class AppSettingContractTests : IAppSettingContractTests
{
}
