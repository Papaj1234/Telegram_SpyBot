namespace TelegramSpyBot.App.Lobbies.Interfaces;

public interface ILobbyTimeoutScheduler
{
    event Func<Lobby, Task>? TimedOut;

    void Schedule(Lobby lobby);
    void Cancel(Lobby lobby);
}