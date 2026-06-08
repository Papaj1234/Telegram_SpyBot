using Microsoft.Extensions.Options;
using TelegramSpyBot.App.Common;
using TelegramSpyBot.App.Game.Interfaces;
using TelegramSpyBot.App.Game.Models;
using TelegramSpyBot.App.Lobbies.Options;

namespace TelegramSpyBot.App.Game;

public class PendingGameStore : IPendingGameStore
{
    private readonly ExpiringStore<PendingGameRequest> store;

    public PendingGameStore(IOptions<LobbyOptions> options)
    {
        var lifetime = TimeSpan.FromSeconds(options.Value.LifetimeSeconds);
        store = new ExpiringStore<PendingGameRequest>(lifetime);
        store.Expired += OnExpired;
    }

    public event Func<PendingGameRequest, Task>? TimedOut;

    public bool TryAdd(PendingGameRequest request)
        => store.TryAdd(request);

    public PendingGameRequest? Get(long chatId)
        => store.Get(chatId);

    public bool Remove(long chatId)
        => store.Remove(chatId);

    private async Task OnExpired(PendingGameRequest request)
    {
        if (TimedOut is { } handler)
        {
            await handler(request);
        }
    }
}