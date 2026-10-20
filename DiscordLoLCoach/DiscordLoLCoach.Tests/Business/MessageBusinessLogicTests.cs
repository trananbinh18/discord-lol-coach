namespace DiscordLoLCoach.Tests.Business;

using System.Threading.Tasks;
using DiscordLoLCoach.Application.Business;
using DiscordLoLCoach.Application.Interfaces;
using NSubstitute;
using Xunit;

public class MessageBusinessLogicTests
{
    [Fact]
    public async Task ProcessNormalMessageAsync_SendsReceivedYourMessage()
    {
        // Arrange
        var contextService = Substitute.For<ICommunicationContextService>();
        var businessLogic = new MessageBusinessLogic(contextService);

        // Act
        await businessLogic.ProcessNormalMessageAsync();

        // Assert
        await contextService.Received(1).SendOrUpdateResponseAsync("Received your message.");
    }

    [Fact]
    public async Task ProcessSlashPingAsync_SendsPong()
    {
        // Arrange
        var contextService = Substitute.For<ICommunicationContextService>();
        var businessLogic = new MessageBusinessLogic(contextService);

        // Act
        await businessLogic.ProcessSlashPingAsync();

        // Assert
        await contextService.Received(1).SendOrUpdateResponseAsync("Pong");
    }

    [Fact]
    public async Task ProcessMessageAsync_CompletesSuccessfully()
    {
        // Arrange
        var contextService = Substitute.For<ICommunicationContextService>();
        var businessLogic = new MessageBusinessLogic(contextService);

        // Act & Assert
        await businessLogic.ProcessMessageAsync();
    }
}
