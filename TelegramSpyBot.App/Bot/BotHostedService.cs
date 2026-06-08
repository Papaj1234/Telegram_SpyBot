using Telegram.Bot.Polling;
using TelegramSpyBot.App.Bot.Messages;
using TelegramSpyBot.App.Lobbies.Interfaces;

namespace TelegramSpyBot.App.Bot;

public class BotHostedService : IHostedService
{
    private readonly ITelegramBotClient botClient;
    private readonly IUpdateHandler updateHandler;
    private readonly ILobbyStore lobbyStore;

    public BotHostedService(
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler,
        ILobbyStore lobbyStore)
    {
        this.botClient = botClient;
        this.updateHandler = updateHandler;
        this.lobbyStore = lobbyStore;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        lobbyStore.Expired += OnLobbyExpired;

        botClient.StartReceiving(
            updateHandler: updateHandler,
            cancellationToken: cancellationToken);

        Console.WriteLine("Bot started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lobbyStore.Expired -= OnLobbyExpired;
        Console.WriteLine("Bot stopped.");
        return Task.CompletedTask;
    }

    private async Task OnLobbyExpired(Lobby lobby)
    {
        try
        {
            await botClient.EditMessageText(
                chatId: lobby.ChatId,
                messageId: lobby.MessageId,
                text: LobbyMessageBuilder.LobbyCancelled());
        }
        catch
        {
            // the message may be deleted, ignore it
        }
    }
}