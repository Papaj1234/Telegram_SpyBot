namespace TelegramSpyBot.App.Lobbies.Interfaces;

public interface ILobbyStore
{
    Lobby? Get(long chatId);

    bool TryAdd(Lobby lobby);

    bool Remove(long chatId);
}