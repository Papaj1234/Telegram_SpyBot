using Microsoft.Extensions.Options;
using TelegramSpyBot.App.Common;
using TelegramSpyBot.App.Lobbies.Interfaces;
using TelegramSpyBot.App.Lobbies.Options;

namespace TelegramSpyBot.App.Lobbies;

public class LobbyStore : ILobbyStore
{
    private readonly ExpiringStore<Lobby> store;

    public LobbyStore(IOptions<LobbyOptions> options)
    {
        var lifetime = TimeSpan.FromSeconds(options.Value.LifetimeSeconds);
        store = new ExpiringStore<Lobby>(lifetime);
        store.Expired += OnExpired;
    }

    public event Func<Lobby, Task>? Expired;

    public Lobby? Get(long chatId)
        => store.Get(chatId);

    public bool TryAdd(Lobby lobby)
        => store.TryAdd(lobby);

    public bool Remove(long chatId)
        => store.Remove(chatId);

    public void Cancel(Lobby lobby)
        => store.Cancel(lobby);

    private async Task OnExpired(Lobby lobby)
    {
        if (lobby.State != LobbyState.WaitingForPlayers)
        {
            return;
        }

        lobby.State = LobbyState.Cancelled;

        if (Expired is { } handler)
        {
            await handler(lobby);
        }
    }
}