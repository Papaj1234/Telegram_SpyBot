namespace TelegramSpyBot.App.Bot.Coordination.Interfaces;

public interface IGameStartCoordinator
{
    Task StartAsync(Lobby lobby, CancellationToken cancellationToken);
}