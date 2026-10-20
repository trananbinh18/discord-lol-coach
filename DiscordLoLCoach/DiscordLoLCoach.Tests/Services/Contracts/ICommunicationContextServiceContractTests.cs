namespace DiscordLoLCoach.Tests.Services.Contracts;

using System;
using System.Threading.Tasks;
using DiscordLoLCoach.Application.Interfaces;
using Xunit;

/// <summary>
/// Abstract contract tests for ICommunicationContextService implementations.
/// </summary>
public abstract class ICommunicationContextServiceContractTests : IAsyncLifetime
{
    protected abstract Task<ICommunicationContextService> CreateSutAsync();

    public virtual Task InitializeAsync() => Task.CompletedTask;
    public virtual Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Contract_InitialState_PropertiesAreNullOrZero()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Assert
        Assert.Null(sut.UserMessage);
        Assert.Equal(0UL, sut.UserMessageId);
        Assert.Equal(0UL, sut.ChannelId);
        Assert.Null(sut.AckMessage);
        Assert.Equal(0UL, sut.AckMessageId);
    }

    [Fact]
    public async Task Contract_SendAckAsync_SetsAckMessageProperty()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Act
        await sut.SendAckAsync("Thinking...");

        // Assert
        Assert.Equal("Thinking...", sut.AckMessage);
    }

    [Fact]
    public async Task Contract_SendOrUpdateResponseAsync_WithoutInitialization_ThrowsInvalidOperationException()
    {
        // Arrange
        var sut = await CreateSutAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SendOrUpdateResponseAsync("Test Response"));
    }
}

/// <summary>
/// Alias without interface prefix matching alternative naming convention.
/// </summary>
public abstract class CommunicationContextServiceContractTests : ICommunicationContextServiceContractTests
{
}
