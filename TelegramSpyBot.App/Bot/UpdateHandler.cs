using Telegram.Bot.Polling;
using TelegramSpyBot.App.Bot.Handlers.Interfaces;

namespace TelegramSpyBot.App.Bot;

public class UpdateHandler : IUpdateHandler
{
    private readonly ICommandHandler commandHandler;
    private readonly ICallbackHandler callbackHandler;

    public UpdateHandler(
        ICommandHandler commandHandler,
        ICallbackHandler callbackHandler)
    {
        this.commandHandler = commandHandler;
        this.callbackHandler = callbackHandler;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        var task = update switch
        {
            { Message: { } message } => commandHandler.HandleAsync(message, cancellationToken),
            { CallbackQuery: { } callback } => callbackHandler.HandleAsync(callback, cancellationToken),
            _ => Task.CompletedTask
        };

        await task;
    }

    public Task HandleErrorAsync(
        ITelegramBotClient client,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}