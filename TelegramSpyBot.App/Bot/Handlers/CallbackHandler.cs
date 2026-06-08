using TelegramSpyBot.App.Bot.Coordination.Interfaces;
using TelegramSpyBot.App.Bot.Handlers.Interfaces;
using TelegramSpyBot.App.Bot.Keyboards;
using TelegramSpyBot.App.Bot.Messaging.Interfaces;
using TelegramSpyBot.App.Bot.Messages;

namespace TelegramSpyBot.App.Bot.Handlers;

public class CallbackHandler : ICallbackHandler
{
    private readonly ITelegramBotClient botClient;
    private readonly ILobbyManager lobbyManager;
    private readonly IGameStartCoordinator gameStartCoordinator;
    private readonly IPrivateChatProbe privateChatProbe;

    public CallbackHandler(
        ITelegramBotClient botClient,
        ILobbyManager lobbyManager,
        IGameStartCoordinator gameStartCoordinator,
        IPrivateChatProbe privateChatProbe)
    {
        this.botClient = botClient;
        this.lobbyManager = lobbyManager;
        this.gameStartCoordinator = gameStartCoordinator;
        this.privateChatProbe = privateChatProbe;
    }

    public async Task HandleAsync(
        CallbackQuery callback,
        CancellationToken cancellationToken)
    {
        if (callback.Data != LobbyKeyboard.JoinCallback)
        {
            await AnswerAsync(callback.Id, cancellationToken: cancellationToken);
            return;
        }

        if (callback.Message is not { } message)
        {
            await AnswerAsync(callback.Id, cancellationToken: cancellationToken);
            return;
        }

        var chatId = message.Chat.Id;

        //we must check private access when connecting in the lobby

        var canMessage = await privateChatProbe.CanMessageAsync(
            callback.From.Id, cancellationToken);

        if (!canMessage)
        {
            await AnswerAsync(
                callback.Id,
                "Start a chat with me first: open my profile and press Start, then join.",
                cancellationToken,
                showAlert: true);
            return;
        }

        var player = new LobbyPlayer
        {
            TelegramUserId = callback.From.Id,
            FirstName = callback.From.FirstName,
            Username = callback.From.Username ?? string.Empty
        };

        var result = lobbyManager.TryJoin(chatId, player);

        switch (result.Status)
        {
            case LobbyJoinStatus.LobbyNotFound:
                await AnswerAsync(callback.Id, "Lobby is no longer available.", cancellationToken);
                break;

            case LobbyJoinStatus.AlreadyJoined:
                await AnswerAsync(callback.Id, "You're already in the lobby.", cancellationToken);
                break;

            case LobbyJoinStatus.LobbyFull:
                await AnswerAsync(callback.Id, "Lobby is full.", cancellationToken);
                break;

            case LobbyJoinStatus.Joined:
                await AnswerAsync(callback.Id, "You joined the lobby!", cancellationToken);
                await UpdateLobbyMessageAsync(result.Lobby!, cancellationToken);
                break;

            case LobbyJoinStatus.NowFull:
                await AnswerAsync(callback.Id, "You joined! Starting game...", cancellationToken);
                await gameStartCoordinator.StartAsync(result.Lobby!, cancellationToken);
                break;
        }
    }

    private async Task UpdateLobbyMessageAsync(
        Lobby lobby,
        CancellationToken cancellationToken)
        => await botClient.EditMessageText(
            chatId: lobby.ChatId,
            messageId: lobby.MessageId,
            text: LobbyMessageBuilder.Lobby(lobby),
            parseMode: ParseMode.Html,
            replyMarkup: LobbyKeyboard.Build(),
            cancellationToken: cancellationToken);

    private async Task AnswerAsync(
        string callbackId,
        string? text = null,
        CancellationToken cancellationToken = default,
        bool showAlert = false)
        => await botClient.AnswerCallbackQuery(
            callbackQueryId: callbackId,
            text: text,
            showAlert: showAlert,
            cancellationToken: cancellationToken);
}