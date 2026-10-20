namespace DiscordLoLCoach.Tests.Services.Implementations;

using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.Application.Services;
using DiscordLoLCoach.Tests.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Xunit;

public class AppSettingTests : IAppSettingContractTests
{
    protected override Task<IAppSetting> CreateSutAsync()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"DiscordSettings:Token", "test-token-123"},
            {"DiscordSettings:AllowServerIds:0", "111222333"},
            {"DiscordSettings:AllowServerIds:1", "444555666"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return Task.FromResult<IAppSetting>(new AppSetting(configuration));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Specific_AppSetting_AllowServerIds_NullOrEmptyOrBlank_AllowsAllServers(string blankEntry)
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"DiscordSettings:Token", "sample-token"},
            {"DiscordSettings:AllowServerIds:0", blankEntry}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        IAppSetting appSetting = new AppSetting(configuration);

        // Assert
        Assert.True(appSetting.IsServerAllowed(123456789));
        Assert.True(appSetting.IsServerAllowed(987654321));
        Assert.True(appSetting.IsServerAllowed(null));
    }

    [Fact]
    public void Specific_AppSetting_AllowServerIds_EmptyList_AllowsAllServers()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"DiscordSettings:Token", "sample-token"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        IAppSetting appSetting = new AppSetting(configuration);

        // Assert
        Assert.True(appSetting.IsServerAllowed(123456789));
        Assert.True(appSetting.IsServerAllowed(null));
    }
}
