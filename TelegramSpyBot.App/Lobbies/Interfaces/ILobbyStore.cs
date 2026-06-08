namespace TelegramSpyBot.App.Lobbies.Interfaces;

public interface ILobbyStore
{
    event Func<Lobby, Task>? Expired;

    Lobby? Get(long chatId);

    bool TryAdd(Lobby lobby);

    bool Remove(long chatId);

    void Cancel(Lobby lobby);
}