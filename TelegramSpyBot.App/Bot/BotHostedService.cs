using Telegram.Bot.Polling;

namespace TelegramSpyBot.App.Bot;

public class BotHostedService : IHostedService
{
    private readonly ITelegramBotClient botClient;
    private readonly IUpdateHandler updateHandler;

    public BotHostedService(
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler)
    {
        this.botClient = botClient;
        this.updateHandler = updateHandler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        botClient.StartReceiving(
            updateHandler: updateHandler,
            cancellationToken: cancellationToken);

        Console.WriteLine("Bot is running.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Bot is stoped.");
        return Task.CompletedTask;
    }
}