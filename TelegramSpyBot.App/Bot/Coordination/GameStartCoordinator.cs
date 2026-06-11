using TelegramSpyBot.App.Bot.Coordination.Interfaces;
using TelegramSpyBot.App.Bot.Messages;
using TelegramSpyBot.App.Game.Interfaces;
using TelegramSpyBot.App.Game.Models;

namespace TelegramSpyBot.App.Bot.Coordination;

public class GameStartCoordinator : IGameStartCoordinator
{
    private readonly ITelegramBotClient botClient;
    private readonly ILobbyManager lobbyManager;
    private readonly IServiceScopeFactory scopeFactory;

    public GameStartCoordinator(
        ITelegramBotClient botClient,
        ILobbyManager lobbyManager,
        IServiceScopeFactory scopeFactory)
    {
        this.botClient = botClient;
        this.lobbyManager = lobbyManager;
        this.scopeFactory = scopeFactory;
    }

    public async Task StartAsync(Lobby lobby, CancellationToken cancellationToken)
    {
        await botClient.EditMessageText(
            chatId: lobby.ChatId,
            messageId: lobby.MessageId,
            text: LobbyMessageBuilder.GameStarting(lobby),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        var result = await RunGameAsync(lobby);

        if (result is null)
        {
            await botClient.SendMessage(
                chatId: lobby.ChatId,
                text: "This universe has no characters. Game cancelled.",
                cancellationToken: cancellationToken);

            lobby.State = LobbyState.Cancelled;
            lobbyManager.Remove(lobby.ChatId);
            return;
        }

        var failed = await DealRolesAsync(lobby, result, cancellationToken);

        await AnnounceResultAsync(lobby, failed, cancellationToken);

        lobby.State = LobbyState.Active;
        lobbyManager.Remove(lobby.ChatId);
    }

    private async Task<GameStartResult?> RunGameAsync(Lobby lobby)
    {
        using var scope = scopeFactory.CreateScope();
        var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
        return await gameService.StartGameAsync(lobby);
    }

    private async Task<List<LobbyPlayer>> DealRolesAsync(
        Lobby lobby,
        GameStartResult result,
        CancellationToken cancellationToken)
    {
        var failed = new List<LobbyPlayer>();
        var roleById = result.Roles.ToDictionary(r => r.TelegramUserId, r => r.IsSpy);

        foreach (var player in lobby.Players)
        {
            var isSpy = roleById[player.TelegramUserId];

            var text = isSpy
                ? "You are the <b>SPY</b>!\n\nYou don't know the character. " +
                  "Listen carefully and blend in. Try not to get caught!"
                : $"Your character: <b>{result.CharacterName}</b>\n\n" +
                  "One of the players is a spy who doesn't know this. Find them!";
            try
            {
                if (!isSpy && !string.IsNullOrEmpty(result.CharacterImageUrl))
                {
                    await botClient.SendPhoto(
                        chatId: player.TelegramUserId,
                        photo: InputFile.FromUri(result.CharacterImageUrl),
                        caption: text,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(
                        chatId: player.TelegramUserId,
                        text: text,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                }
            }
            catch
            {
                try
                {
                    await botClient.SendMessage(
                        chatId: player.TelegramUserId,
                        text: text,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                }
                catch
                {
                    failed.Add(player);
                }
            }
        }
        return failed;
    }

    private async Task AnnounceResultAsync(
        Lobby lobby,
        List<LobbyPlayer> failed,
        CancellationToken cancellationToken)
    {
        if (failed.Count == 0)
        {
            await botClient.SendMessage(
                chatId: lobby.ChatId,
                text: "Roles have been dealt! Check your DMs. Let the game begin!",
                cancellationToken: cancellationToken);
            return;
        }

        var mentions = string.Join(", ", failed.Select(FormatMention));

        await botClient.SendMessage(
            chatId: lobby.ChatId,
            text: $"Couldn't message: {mentions}\n" +
                  "Please start a chat with me (/start in DM) before joining next time.",
            cancellationToken: cancellationToken);
    }

    private static string FormatMention(LobbyPlayer player)
        => string.IsNullOrEmpty(player.Username)
            ? player.FirstName
            : $"@{player.Username}";
}