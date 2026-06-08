using Microsoft.Extensions.Options;
using TelegramSpyBot.App.Lobbies.Interfaces;
using TelegramSpyBot.App.Lobbies.Options;

namespace TelegramSpyBot.App.Lobbies;

public class LobbyManager : ILobbyManager
{
    private readonly ILobbyStore store;
    private readonly LobbyOptions options;

    public LobbyManager(
        ILobbyStore store,
        IOptions<LobbyOptions> options)
    {
        this.store = store;
        this.options = options.Value;
    }

    public Lobby? GetByChat(long chatId)
        => store.Get(chatId);

    public LobbyCreateResult TryCreate(
        long chatId,
        long creatorId,
        int requestedSize,
        int universeId,
        string universeName)
    {
        if (requestedSize < options.MinSize)
        {
            return new LobbyCreateResult(LobbyCreateStatus.SizeTooSmall, null);
        }

        var size = Math.Min(requestedSize, options.MaxSize);

        var lobby = new Lobby
        {
            ChatId = chatId,
            CreatorId = creatorId,
            TargetSize = size,
            UniverseId = universeId,
            UniverseName = universeName,
            LifetimeSeconds = options.LifetimeSeconds
        };

        if (!store.TryAdd(lobby))
        {
            return new LobbyCreateResult(LobbyCreateStatus.AlreadyExists, null);
        }

        return new LobbyCreateResult(LobbyCreateStatus.Created, lobby);
    }

    public LobbyJoinResult TryJoin(long chatId, LobbyPlayer player)
    {
        var lobby = store.Get(chatId);

        if (lobby is null || lobby.State != LobbyState.WaitingForPlayers)
        {
            return new LobbyJoinResult(LobbyJoinStatus.LobbyNotFound, null);
        }

        if (lobby.Players.Any(p => p.TelegramUserId == player.TelegramUserId))
        {
            return new LobbyJoinResult(LobbyJoinStatus.AlreadyJoined, lobby);
        }

        if (lobby.IsFull)
        {
            return new LobbyJoinResult(LobbyJoinStatus.LobbyFull, lobby);
        }

        lobby.TryAddPlayer(player);

        if (lobby.IsFull)
        {
            lobby.State = LobbyState.Starting;
            store.Cancel(lobby);
            return new LobbyJoinResult(LobbyJoinStatus.NowFull, lobby);
        }

        return new LobbyJoinResult(LobbyJoinStatus.Joined, lobby);
    }

    public bool Remove(long chatId)
    {
        var lobby = store.Get(chatId);

        if (lobby is null)
        {
            return false;
        }

        return store.Remove(chatId);
    }
}