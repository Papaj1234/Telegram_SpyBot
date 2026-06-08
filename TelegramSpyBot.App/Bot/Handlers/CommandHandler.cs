using TelegramSpyBot.App.Bot.Handlers.Interfaces;
using TelegramSpyBot.App.Bot.Keyboards;
using TelegramSpyBot.App.Bot.Messages;
using TelegramSpyBot.Data.Models;
using TelegramSpyBot.Data.UnitOfWork;

namespace TelegramSpyBot.App.Bot.Handlers;

public class CommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient botClient;
    private readonly ILobbyManager lobbyManager;
    private readonly IServiceScopeFactory scopeFactory;

    public CommandHandler(
        ITelegramBotClient botClient,
        ILobbyManager lobbyManager,
        IServiceScopeFactory scopeFactory)
    {
        this.botClient = botClient;
        this.lobbyManager = lobbyManager;
        this.scopeFactory = scopeFactory;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } text)
        {
            return;
        }

        var command = text.Split(' ')[0].Split('@')[0].ToLowerInvariant();

        switch (command)
        {
            case "/start":
            case "/help":
                await SendWelcomeAsync(message.Chat.Id, cancellationToken);
                break;

            case "/newgame":
                await HandleNewGameAsync(message, text, cancellationToken);
                break;
        }
    }

    private async Task SendWelcomeAsync(long chatId, CancellationToken cancellationToken)
        => await botClient.SendMessage(
            chatId: chatId,
            text: LobbyMessageBuilder.Welcome(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

    private async Task HandleNewGameAsync(
        Message message,
        string text,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || !int.TryParse(parts[1], out var size))
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Specify lobby size, e.g.: /newgame 5",
                cancellationToken: cancellationToken);
            return;
        }

        var universe = await GetFirstUniverseAsync();

        if (universe is null)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "No universes available. Please seed the database first.",
                cancellationToken: cancellationToken);
            return;
        }

        var result = lobbyManager.TryCreate(
            chatId: chatId,
            creatorId: message.From!.Id,
            requestedSize: size,
            universeId: universe.Id,
            universeName: universe.Name);

        switch (result.Status)
        {
            case LobbyCreateStatus.SizeTooSmall:
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Minimum 3 players. Try: /newgame 3",
                    cancellationToken: cancellationToken);
                return;

            case LobbyCreateStatus.AlreadyExists:
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "There is already an active lobby in this chat.",
                    cancellationToken: cancellationToken);
                return;
        }

        var lobby = result.Lobby!;

        var sent = await botClient.SendMessage(
            chatId: chatId,
            text: LobbyMessageBuilder.Lobby(lobby),
            parseMode: ParseMode.Html,
            replyMarkup: LobbyKeyboard.Build(),
            cancellationToken: cancellationToken);

        lobby.MessageId = sent.MessageId;
    }

    private async Task<Universe?> GetFirstUniverseAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var universes = await unitOfWork.Universes.GetAllAsync();
        return universes.Count > 0 ? universes[0] : null;
    }
}