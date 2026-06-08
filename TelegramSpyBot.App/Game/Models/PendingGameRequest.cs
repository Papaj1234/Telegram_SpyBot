using TelegramSpyBot.App.Common.Interfaces;

namespace TelegramSpyBot.App.Game.Models;

public class PendingGameRequest : IExpirable
{
    public long ChatId { get; init; }

    public long CreatorId { get; init; }

    public int Size { get; init; }

    public int MessageId { get; set; }

    public long Key => ChatId;

    public CancellationTokenSource TimeoutCts { get; } = new();

    public int LifetimeSeconds { get; init; }
}