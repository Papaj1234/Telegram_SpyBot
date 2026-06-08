using TelegramSpyBot.App.Game.Models;

namespace TelegramSpyBot.App.Game.Interfaces;

public interface IPendingGameStore
{
    event Func<PendingGameRequest, Task>? TimedOut;

    bool TryAdd(PendingGameRequest request);

    PendingGameRequest? Get(long chatId);

    bool Remove(long chatId);
}