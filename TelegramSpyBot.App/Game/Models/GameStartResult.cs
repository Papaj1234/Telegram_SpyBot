namespace TelegramSpyBot.App.Game.Models;

public class GameStartResult
{
    public required string CharacterName { get; init; }
    public required IReadOnlyList<PlayerRole> Roles { get; init; }
}

public class PlayerRole
{
    public long TelegramUserId { get; init; }
    public bool IsSpy { get; init; }
}