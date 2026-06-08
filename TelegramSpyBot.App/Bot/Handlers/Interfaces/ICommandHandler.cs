namespace TelegramSpyBot.App.Bot.Handlers.Interfaces;

public interface ICommandHandler
{
    Task HandleAsync(Message message, CancellationToken cancellationToken);
}