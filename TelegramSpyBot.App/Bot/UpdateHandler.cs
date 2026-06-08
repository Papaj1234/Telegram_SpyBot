using Telegram.Bot.Polling;

namespace TelegramSpyBot.App.Bot;

public class UpdateHandler : IUpdateHandler
{
    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
        {
            return;
        }

        if (message.Text is not { } text)
        {
            return;
        }    

        var command = text.Split(' ')[0].ToLowerInvariant();

        switch (command)
        {
            case "/start":
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Привет! Я Михаил Исаеви",
                    cancellationToken: cancellationToken);
                break;

            case "/ping":
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "pong",
                    cancellationToken: cancellationToken);
                break;
        }
    }

    public Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}