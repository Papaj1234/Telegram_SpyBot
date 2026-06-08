using TelegramSpyBot.App.Common.Interfaces;

namespace TelegramSpyBot.App.Lobbies.Models;

public class Lobby : IExpirable
{
    private readonly List<LobbyPlayer> players = new();

    public long ChatId { get; init; }
    public int TargetSize { get; init; }
    public int UniverseId { get; init; }
    public string UniverseName { get; init; } = string.Empty;
    public long CreatorId { get; init; }

    public long Key => ChatId;
    public CancellationTokenSource TimeoutCts => TimerCts;

    public int MessageId { get; set; }
    public LobbyState State { get; set; } = LobbyState.WaitingForPlayers;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public int LifetimeSeconds { get; init; }

    public CancellationTokenSource TimerCts { get; } = new();

    public IReadOnlyList<LobbyPlayer> Players => players;
    public int Count => players.Count;
    public bool IsFull => players.Count >= TargetSize;

    public bool TryAddPlayer(LobbyPlayer player)
    {
        if (players.Any(p => p.TelegramUserId == player.TelegramUserId))
        {
            return false;
        }

        if (IsFull)
        {
            return false;
        }

        players.Add(player);
        return true;
    }
}