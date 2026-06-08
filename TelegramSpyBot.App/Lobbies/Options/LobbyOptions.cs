namespace TelegramSpyBot.App.Lobbies.Options;

public class LobbyOptions
{
    public const string SectionName = "Lobby";

    public int MinSize { get; init; } = 3;
    public int MaxSize { get; init; } = 10;
    public int LifetimeSeconds { get; init; } = 60;
}