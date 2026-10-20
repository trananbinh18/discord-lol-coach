namespace DiscordLoLCoach.Tests.Services.Contracts;

using System.Threading;
using System.Threading.Tasks;
using DiscordLoLCoach.Application.Interfaces;
using Xunit;

/// <summary>
/// Abstract contract tests for ICommunicationConnectionService implementations.
/// </summary>
public abstract class ICommunicationConnectionServiceContractTests : IAsyncLifetime
{
    protected abstract Task<ICommunicationConnectionService> CreateSutAsync();

    public virtual Task InitializeAsync() => Task.CompletedTask;
    public virtual Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Contract_IsConnected_Initially_IsFalse()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.False(sut.IsConnected);
    }

    [Fact]
    public async Task Contract_StopAsync_WhenNotConnected_RemainsDisconnectedWithoutThrowing()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Act
        await sut.StopAsync(CancellationToken.None);

        // Assert
        Assert.False(sut.IsConnected);
    }
}

/// <summary>
/// Alias without interface prefix matching alternative naming convention.
/// </summary>
public abstract class CommunicationConnectionServiceContractTests : ICommunicationConnectionServiceContractTests
{
}
