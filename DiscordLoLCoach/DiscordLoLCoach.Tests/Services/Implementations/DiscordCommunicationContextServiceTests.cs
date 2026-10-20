namespace DiscordLoLCoach.Tests.Services.Implementations;

using System;
using System.Threading.Tasks;
using DiscordLoLCoach.API.Services;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.Tests.Services.Contracts;
using Xunit;

public class DiscordCommunicationContextServiceTests : ICommunicationContextServiceContractTests
{
    private DiscordCommunicationContextService? _currentSut;

    protected override Task<ICommunicationContextService> CreateSutAsync()
    {
        _currentSut = new DiscordCommunicationContextService();
        return Task.FromResult<ICommunicationContextService>(_currentSut);
    }

    public override Task DisposeAsync()
    {
        _currentSut?.Dispose();
        _currentSut = null;
        return Task.CompletedTask;
    }

    [Fact]
    public void Specific_InitializeForMessage_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        using var sut = new DiscordCommunicationContextService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.InitializeForMessage(null!));
    }

    [Fact]
    public void Specific_InitializeForInteraction_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange
        using var sut = new DiscordCommunicationContextService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.InitializeForInteraction(null!));
    }
}
