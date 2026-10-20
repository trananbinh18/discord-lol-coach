namespace DiscordLoLCoach.Tests.Services.Implementations;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DiscordLoLCoach.API.Services;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.Application.Models;
using DiscordLoLCoach.Tests.Services.Contracts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class DiscordConnectionServiceTests : ICommunicationConnectionServiceContractTests
{
    private DiscordConnectionService? _currentSut;

    protected override Task<ICommunicationConnectionService> CreateSutAsync()
    {
        var appSetting = Substitute.For<IAppSetting>();
        appSetting.DiscordSettings.Returns(new DiscordSettings
        {
            Token = "test-token-value",
            AllowServerIds = new List<string>()
        });

        var logger = Substitute.For<ILogger<DiscordConnectionService>>();
        _currentSut = new DiscordConnectionService(appSetting, logger);
        return Task.FromResult<ICommunicationConnectionService>(_currentSut);
    }

    public override async Task DisposeAsync()
    {
        if (_currentSut != null)
        {
            await _currentSut.DisposeAsync();
            _currentSut = null;
        }
    }

    [Fact]
    public async Task Specific_StartAsync_WhenTokenIsEmpty_LogsWarningAndDoesNotConnect()
    {
        // Arrange
        var appSetting = Substitute.For<IAppSetting>();
        appSetting.DiscordSettings.Returns(new DiscordSettings { Token = "" });
        var logger = Substitute.For<ILogger<DiscordConnectionService>>();
        await using var sut = new DiscordConnectionService(appSetting, logger);

        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        Assert.False(sut.IsConnected);
    }

    [Fact]
    public async Task Specific_DiscordConnectionService_InitializesClientAndInteractionService()
    {
        // Arrange
        var appSetting = Substitute.For<IAppSetting>();
        appSetting.DiscordSettings.Returns(new DiscordSettings { Token = "test-token" });
        var logger = Substitute.For<ILogger<DiscordConnectionService>>();
        await using var sut = new DiscordConnectionService(appSetting, logger);

        // Assert
        Assert.NotNull(sut.Client);
        Assert.NotNull(sut.InteractionService);
    }
}
