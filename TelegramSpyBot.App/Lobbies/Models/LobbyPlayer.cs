namespace TelegramSpyBot.App.Lobbies.Models;

public class LobbyPlayer
{
    public long TelegramUserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
}