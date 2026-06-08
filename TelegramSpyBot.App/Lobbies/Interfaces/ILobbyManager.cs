namespace TelegramSpyBot.App.Lobbies.Interfaces;

public interface ILobbyManager
{
    Lobby? GetByChat(long chatId);

    LobbyCreateResult TryCreate(
        long chatId,
        long creatorId,
        int requestedSize,
        int universeId,
        string universeName);

    LobbyJoinResult TryJoin(long chatId, LobbyPlayer player);

    bool Remove(long chatId);
}