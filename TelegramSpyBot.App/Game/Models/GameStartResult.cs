namespace TelegramSpyBot.App.Game.Models;

public class GameStartResult
{
    public required string CharacterName { get; init; }

    public string? CharacterImageUrl { get; init; }

    public required IReadOnlyList<PlayerRole> Roles { get; init; }
}