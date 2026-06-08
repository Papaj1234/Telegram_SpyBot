using System.Collections.Concurrent;
using TelegramSpyBot.App.Lobbies.Interfaces;

namespace TelegramSpyBot.App.Lobbies;

public class LobbyStore : ILobbyStore
{
    private readonly ConcurrentDictionary<long, Lobby> lobbies = new();

    public Lobby? Get(long chatId)
        => lobbies.TryGetValue(chatId, out var lobby) ? lobby : null;

    public bool TryAdd(Lobby lobby)
        => lobbies.TryAdd(lobby.ChatId, lobby);

    public bool Remove(long chatId)
        => lobbies.TryRemove(chatId, out _);
}