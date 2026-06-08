using Microsoft.Extensions.Options;
using TelegramSpyBot.App.Lobbies.Interfaces;
using TelegramSpyBot.App.Lobbies.Options;

namespace TelegramSpyBot.App.Lobbies;

public class LobbyTimeoutScheduler : ILobbyTimeoutScheduler
{
    private readonly TimeSpan lifetime;

    public LobbyTimeoutScheduler(IOptions<LobbyOptions> options)
        => lifetime = TimeSpan.FromSeconds(options.Value.LifetimeSeconds);

    public event Func<Lobby, Task>? TimedOut;

    public void Schedule(Lobby lobby)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(lifetime, lobby.TimerCts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (lobby.State != LobbyState.WaitingForPlayers)
            {
                return;
            }

            lobby.State = LobbyState.Cancelled;

            if (TimedOut is { } handler)
            {
                await handler(lobby);
            }
        });
    }

    public void Cancel(Lobby lobby)
    {
        if (!lobby.TimerCts.IsCancellationRequested)
        {
            lobby.TimerCts.Cancel();
        }
    }
}