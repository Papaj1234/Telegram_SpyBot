namespace TelegramSpyBot.App.Common.Interfaces;

public interface IExpirable
{
    long Key { get; }

    CancellationTokenSource TimeoutCts { get; }
}