namespace TelegramSpyBot.App.Bot.Handlers.Interfaces;

public interface ICallbackHandler
{
    Task HandleAsync(CallbackQuery callback, CancellationToken cancellationToken);
}